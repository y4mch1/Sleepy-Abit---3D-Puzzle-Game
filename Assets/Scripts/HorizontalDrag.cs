using System.Collections.Generic;
using UnityEngine;

public class HorizontalDrag : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private bool isAutoSliding = false;
    private float fixedX;
    private float fixedY;
    private Vector3 targetPosition;
    private Vector3 slideDirection;
    private Vector3 initialDragPosition;
    [Range(1f, 50f)]
    public float autoSlideSpeed = 20f;
    [Range(0.1f, 2f)]
    public float dragThreshold = 0.5f;
    public List<Collider> stopColliders = new List<Collider>();
    private Collider selfCollider;
    public AudioClip slideAudio; 
    [Range(0f, 1f)]
    public float slideVolume = 1f; 
    private AudioSource audioSource;

    void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position;
        selfCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = slideVolume;
    }

    void OnMouseDown()
    {
        isDragging = true;
        isAutoSliding = false;
        fixedX = transform.position.x;
        fixedY = transform.position.y;
        initialDragPosition = GetMouseWorldPositionOnPlane();
    }

    void OnMouseDrag()
    {
        if (!isDragging || isAutoSliding) return;

        Vector3 mouseWorld = GetMouseWorldPositionOnPlane();
        float dragDistance = mouseWorld.z - initialDragPosition.z;

        if (Mathf.Abs(dragDistance) > dragThreshold)
        {
            slideDirection = dragDistance > 0 ? Vector3.forward : Vector3.back;
            isAutoSliding = true;
            isDragging = false;

          
            if (slideAudio != null)
            {
                audioSource.clip = slideAudio;
                audioSource.volume = slideVolume;
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        if (isAutoSliding)
        {
            Vector3 nextPosition = transform.position + slideDirection * autoSlideSpeed * Time.deltaTime;
            nextPosition.x = fixedX;
            nextPosition.y = fixedY;

            if (!IsBlockedAt(nextPosition))
            {
                transform.position = nextPosition;
            }
            else
            {
                isAutoSliding = false;
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    Vector3 GetMouseWorldPositionOnPlane()
    {
        Plane plane = new Plane(Vector3.right, transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
            return ray.GetPoint(distance);

        return transform.position;
    }

    bool IsBlockedAt(Vector3 nextPos)
    {
        if (selfCollider == null) return false;

        Bounds futureBounds = selfCollider.bounds;
        Vector3 delta = nextPos - transform.position;
        futureBounds.center += delta;

        foreach (var col in stopColliders)
        {
            if (col == null) continue;
            if (futureBounds.Intersects(col.bounds))
                return true;
        }

        return false;
    }
}

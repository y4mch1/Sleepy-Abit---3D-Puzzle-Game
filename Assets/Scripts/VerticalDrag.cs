using System.Collections.Generic;
using UnityEngine;

public class VerticalDrag : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private bool isAutoSliding = false;
    private float fixedY;
    private float fixedZ;
    private Vector3 slideDirection;
    private Vector3 initialMouseScreenPos;

    [Range(1f, 50f)]
    public float autoSlideSpeed = 20f;

    [Range(10f, 100f)]
    public float dragThreshold = 30f;

    public List<Collider> stopColliders = new List<Collider>();
    private Collider selfCollider;

    public AudioClip slideAudio; 
    [Range(0f, 1f)]
    public float slideVolume = 1f; 
    private AudioSource audioSource;

    void Start()
    {
        cam = Camera.main;
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
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
        initialMouseScreenPos = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        if (!isDragging || isAutoSliding) return;

        float deltaY = Input.mousePosition.y - initialMouseScreenPos.y;

        if (Mathf.Abs(deltaY) > dragThreshold)
        {
            // Drag ke atas -> sliding ke kiri, drag ke bawah -> sliding ke kanan
            slideDirection = deltaY > 0 ? Vector3.left : Vector3.right;

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
            nextPosition.y = fixedY;
            nextPosition.z = fixedZ;

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

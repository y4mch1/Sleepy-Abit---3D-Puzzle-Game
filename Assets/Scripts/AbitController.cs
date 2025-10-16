using UnityEngine;
public class AbitController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public LayerMask obstacleLayer;
    public bool enableJumpAnimation = true;
    public float jumpHeight = 0.3f;
    public float jumpSpeed = 5f;
    public AudioClip[] moveSounds; 
    public AudioClip[] collisionSounds; 
    [Range(0f, 1f)]
    public float audioVolume = 1f;
    private AudioSource audioSource;
    private Vector3 moveDirection = Vector3.zero;
    private bool isMoving = false;
    private bool blockedUp = false;
    private bool blockedDown = false;
    private bool blockedLeft = false;
    private bool blockedRight = false;
    private Rigidbody rb;
    private Camera cam;
    private Quaternion targetRotation = Quaternion.identity;
    private float jumpProgress = 0f;
    private float originalY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 
        audioSource.volume = audioVolume;
        float yRot = transform.eulerAngles.y;
        float snappedY = Mathf.Round(yRot / 90f) * 90f;
        transform.rotation = Quaternion.Euler(0, snappedY, 0);
        targetRotation = transform.rotation;
        
        originalY = transform.position.y;
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
        
        if (isMoving && enableJumpAnimation)
        {
            jumpProgress += Time.deltaTime * jumpSpeed;
        }
        else if (!isMoving)
        {
            jumpProgress = 0f;
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            
            if (Mathf.Abs(moveDirection.x) > 0.5f)
            {
                newPos.z = Mathf.Round(rb.position.z * 10f) / 10f;
            }
            else if (Mathf.Abs(moveDirection.z) > 0.5f)
            {
                newPos.x = Mathf.Round(rb.position.x * 10f) / 10f;
            }
            
            if (enableJumpAnimation)
            {
                float jumpOffset = Mathf.Abs(Mathf.Sin(jumpProgress)) * jumpHeight;
                newPos.y = originalY + jumpOffset;
            }
            else
            {
                newPos.y = originalY;
            }
            
            rb.MovePosition(newPos);
            rb.MoveRotation(targetRotation);
        }
        else
        {
            Vector3 pos = rb.position;
            pos.y = originalY;
            rb.MovePosition(pos);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MoveRotation(targetRotation);
        }
    }

    void HandleInput()
    {
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 snappedForward = SnapToGrid(camForward);
        Vector3 snappedRight = SnapToGrid(camRight);

        if (Input.GetKeyDown(KeyCode.UpArrow) && !blockedUp)
        {
            TryMove(snappedForward);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !blockedDown)
        {
            TryMove(-snappedForward);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !blockedRight)
        {
            TryMove(snappedRight);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !blockedLeft)
        {
            TryMove(-snappedRight);
        }
    }

    void TryMove(Vector3 direction)
    {
        RaycastHit hit;
        bool hasObstacle = Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            direction,
            out hit,
            0.8f, 
            obstacleLayer
        );
        
        if (hasObstacle)
        {
           
            BlockDirection(direction);
            Debug.Log("Arah " + direction + " ke-block!");
            return;
        }
        
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        targetAngle = Mathf.Round(targetAngle / 90f) * 90f; // snap ke 90 derajat
        targetRotation = Quaternion.Euler(0, targetAngle, 0);
        
        transform.rotation = targetRotation;
        rb.rotation = targetRotation;

        moveDirection = direction;
        isMoving = true;
        jumpProgress = 0f;
    
        PlayRandomSound(moveSounds);
        ResetBlocks();
    }

    void BlockDirection(Vector3 direction)
    {
        // tentuin arah mana yang ke-block berdasarkan direction
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 snappedForward = SnapToGrid(camForward);
        Vector3 snappedRight = SnapToGrid(camRight);
        
        if (Vector3.Dot(direction, snappedForward) > 0.9f)
            blockedUp = true;
        else if (Vector3.Dot(direction, -snappedForward) > 0.9f)
            blockedDown = true;
        else if (Vector3.Dot(direction, snappedRight) > 0.9f)
            blockedRight = true;
        else if (Vector3.Dot(direction, -snappedRight) > 0.9f)
            blockedLeft = true;
    }

    void ResetBlocks()
    {
        blockedUp = false;
        blockedDown = false;
        blockedLeft = false;
        blockedRight = false;
    }

    Vector3 SnapToGrid(Vector3 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absZ = Mathf.Abs(direction.z);

        if (absX > absZ)
        {
            return new Vector3(Mathf.Sign(direction.x), 0, 0);
        }
        else
        {
            return new Vector3(0, 0, Mathf.Sign(direction.z));
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isMoving)
        {
            BlockDirection(moveDirection);
            isMoving = false;
            moveDirection = Vector3.zero;
            
            Vector3 pos = transform.position;
            pos.y = originalY;
            transform.position = pos;
            
            rb.velocity = Vector3.zero;        
            PlayRandomSound(collisionSounds);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && isMoving)
        {
            BlockDirection(moveDirection);
            isMoving = false;
            moveDirection = Vector3.zero;
            
            Vector3 pos = transform.position;
            pos.y = originalY;
            transform.position = pos;
            
            rb.velocity = Vector3.zero;
            
            PlayRandomSound(collisionSounds);
        }
    }
    
    void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        if (audioSource == null) return;
        
        // pilih random dari array
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        if (randomClip != null)
        {
            audioSource.PlayOneShot(randomClip, audioVolume);
            Debug.Log("Playing sound: " + randomClip.name);
        }
        else
        {
            Debug.LogWarning("Audio clip is null!");
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 snappedForward = SnapToGrid(camForward);
        Vector3 snappedRight = SnapToGrid(camRight);
        
        Gizmos.color = blockedUp ? Color.red : Color.green;
        Gizmos.DrawRay(origin, snappedForward * 0.8f);
        
        Gizmos.color = blockedDown ? Color.red : Color.green;
        Gizmos.DrawRay(origin, -snappedForward * 0.8f);
        
        Gizmos.color = blockedRight ? Color.red : Color.green;
        Gizmos.DrawRay(origin, snappedRight * 0.8f);
        
        Gizmos.color = blockedLeft ? Color.red : Color.green;
        Gizmos.DrawRay(origin, -snappedRight * 0.8f);
    }
}
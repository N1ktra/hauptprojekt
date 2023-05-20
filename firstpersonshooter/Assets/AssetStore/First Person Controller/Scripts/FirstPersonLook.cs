using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] private CameraFunctions cameraFunctions;
    [SerializeField] private Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    private Vector2 velocity;
    private Vector2 frameVelocity;


    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        cameraFunctions = transform.parent.gameObject.GetComponent<CameraFunctions>();
    }

    void Update()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;

        float angle = cameraFunctions.getAngle();
        velocity.y = Mathf.Clamp(velocity.y, -90 + angle, 90 + angle);

        // Rotate camera up-down and controller left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}

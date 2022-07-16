using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  [Range(1, 20)]
  [SerializeField]
  float lookSensitivity = 10f;

  CharacterController? controller;
  Transform? cameraTransform;
  Vector2 movementInputVector;
  Vector2 lookInputVector;
  float lookPitch;

  public void LockCursor()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }

  public void Look(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<Vector2>();

    lookInputVector = value;
  }

  public void Move(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<Vector2>();

    movementInputVector = value;
  }

  void HandleLookInput()
  {
    var x = lookInputVector.x * lookSensitivity * Time.deltaTime * 10f;
    var y = lookInputVector.y * lookSensitivity * Time.deltaTime * 10f;

    lookPitch = Mathf.Clamp(lookPitch - y, -90f, 90f);

    if (cameraTransform != null)
    {
      cameraTransform.localRotation = Quaternion.Euler(Vector3.right * lookPitch);
    }

    transform.Rotate(Vector3.up, x);
  }

  void HandleMovementInput()
  {
    var move = movementInputVector.y * transform.forward + movementInputVector.x * transform.right;

    if (controller != null)
    {
      controller.SimpleMove(Vector3.ClampMagnitude(move, 1f) * 6f);
    }
  }

  void Awake()
  {
    controller = GetComponent<CharacterController>();
    cameraTransform = transform.Find("Camera");
  }

  void Start()
  {
    LockCursor();
  }

  void Update()
  {
    HandleLookInput();
    HandleMovementInput();
  }
}

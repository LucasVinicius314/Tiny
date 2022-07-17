using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  [Range(1f, 20f)]
  [SerializeField]
  float lookSensitivity = 10f;

  CharacterController? controller;
  Transform? cameraTransform;
  Vector2 movementInputVector;
  Vector2 lookInputVector;
  Vector2 processedMovementInputVector;
  Vector2 processedLookInputVector;
  float lookPitch;
  [Range(0.001f, 1f)]
  [SerializeField]
  float lookSmoothing = .2f;
  [Range(0.001f, 1f)]
  [SerializeField]
  float movementSmoothing = .05f;

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
    processedLookInputVector = Vector2.Lerp(processedLookInputVector, lookInputVector, lookSmoothing);

    var inputVector = processedLookInputVector;

    var x = inputVector.x * lookSensitivity * Time.deltaTime * 10f;
    var y = inputVector.y * lookSensitivity * Time.deltaTime * 10f;

    lookPitch = Mathf.Clamp(lookPitch - y, -90f, 90f);

    if (cameraTransform != null)
    {
      cameraTransform.localRotation = Quaternion.Euler(Vector3.right * lookPitch);
    }

    transform.Rotate(Vector3.up, x);
  }

  void HandleMovementInput()
  {
    processedMovementInputVector = Vector2.Lerp(processedMovementInputVector, movementInputVector, movementSmoothing);

    var inputVector = processedMovementInputVector;

    var move = inputVector.y * transform.forward + inputVector.x * transform.right;

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

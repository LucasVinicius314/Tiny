using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

// TODO: Add camera sway.
// TODO: Add settings menu.
// TODO: Add turret placement.
// TODO: Add inventory.
// TODO: Add jumping.
// TODO: Add sprinting.
// TODO: Add Poppins as the main font.

public class PlayerScript : MonoBehaviour
{
  [Range(1f, 20f)]
  [SerializeField]
  float lookSensitivity = 10f;
  [Range(0.001f, 1f)]
  [SerializeField]
  float lookSmoothing = .2f;
  [Range(0.001f, 1f)]
  [SerializeField]
  float movementSmoothing = .05f;
  [Range(60f, 120f)]
  [SerializeField]
  float fieldOfView = 120f;
  [Range(60f, 120f)]
  [SerializeField]
  float zoomedInFieldOfView = 30f;

  CharacterController? controller;
  Transform? cameraTransform;
  Vector2 movementInputVector;
  Vector2 lookInputVector;
  Vector2 processedMovementInputVector;
  Vector2 processedLookInputVector;
  MenuScript? menuScript;
  float lookPitch;
  bool isMenuOpen = false;
  bool isZoomed = false;

  void HandleLookInput()
  {
    var lerpFactor = isZoomed ? 0.01f : lookSmoothing;
    var sensitivityMultiplier = isZoomed ? 5f : 10f;

    processedLookInputVector = Vector2.Lerp(processedLookInputVector, lookInputVector, lerpFactor);

    var inputVector = processedLookInputVector;

    var x = inputVector.x * lookSensitivity * Time.deltaTime * sensitivityMultiplier;
    var y = inputVector.y * lookSensitivity * Time.deltaTime * sensitivityMultiplier;

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

  public void OpenMenu(InputAction.CallbackContext context)
  {
    if (!context.performed) return;

    isMenuOpen = !isMenuOpen;

    if (menuScript == null) return;

    if (isMenuOpen)
    {
      menuScript.OpenMenu();
    }
    else
    {
      menuScript.CloseMenu();
    }
  }

  public void Zoom(InputAction.CallbackContext context)
  {
    if (!context.performed) return;

    var value = context.ReadValueAsButton();

    isZoomed = value;

    if (isZoomed)
    {
      Camera.main.fieldOfView = zoomedInFieldOfView;
    }
    else
    {
      Camera.main.fieldOfView = fieldOfView;
    }
  }

  void Awake()
  {
    controller = GetComponent<CharacterController>();
    menuScript = GetComponent<MenuScript>();
    cameraTransform = transform.Find("Camera");

    if (controller == null)
    {
      Debug.LogError("Character controller is null.");
    }

    if (menuScript == null)
    {
      Debug.LogError("Menu script is null.");
    }

    if (cameraTransform == null)
    {
      Debug.LogError("Camera (Transform) script is null.");
    }
  }

  void Start()
  {
    Utils.LockCursor();

    Camera.main.fieldOfView = fieldOfView;
  }

  void Update()
  {
    if (!isMenuOpen)
    {
      HandleLookInput();
      HandleMovementInput();
    }
  }
}

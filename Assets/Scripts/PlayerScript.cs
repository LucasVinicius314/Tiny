using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

// TODO: Add camera sway.
// TODO: Add settings menu.
// TODO: Add inventory.
// TODO: Add jumping.
// TODO: Add sprinting.

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
  [Range(10f, 100f)]
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
  bool isZoomed = false;

  PlayerMenuState playerMenuState = new PlayerMenuState.None();
  PlayerMovementState playerMovementState = new PlayerMovementState.Still();
  PlayerStructurePlacementState playerStructurePlacementState =
    new PlayerStructurePlacementState.None();

  void BeginTurretPlacement()
  {
    if (menuScript != null)
    {
      menuScript.CloseMenu();
    }

    SetPlayerMenuState(new PlayerMenuState.None());
  }

  public void Build(InputAction.CallbackContext context)
  {
    if (!context.performed) return;

    if (playerMenuState is PlayerMenuState.None)
    {
      SetPlayerMenuState(new PlayerMenuState.Build());
    }
    else if (playerMenuState is PlayerMenuState.Build)
    {
      if (menuScript != null)
      {
        menuScript.CloseMenu();
      }

      SetPlayerMenuState(new PlayerMenuState.None());
    }
  }

  public void Escape(InputAction.CallbackContext context)
  {
    if (!context.performed) return;

    if (playerMenuState is PlayerMenuState.None &&
      playerStructurePlacementState is PlayerStructurePlacementState.None)
    {
      // TODO: Settings
    }
    else if (playerMenuState is PlayerMenuState.Build)
    {
      if (menuScript != null)
      {
        menuScript.CloseMenu();
      }

      SetPlayerMenuState(new PlayerMenuState.None());
    }
    else if (
      playerStructurePlacementState is PlayerStructurePlacementState.Placing
    )
    {
      Destroy(
        (
          playerStructurePlacementState as PlayerStructurePlacementState.Placing
        )!.currentInstance
      );

      SetPlayerStructurePlacementState(
        new PlayerStructurePlacementState.None()
      );
    }
  }

  public void Fire(InputAction.CallbackContext context)
  {
    if (!context.performed) return;

    if (playerStructurePlacementState is PlayerStructurePlacementState.None)
    {
      // TODO: Action
    }
    else if (
      playerStructurePlacementState is PlayerStructurePlacementState.Placing
    )
    {
      PlaceTurret(
        (
          playerStructurePlacementState as PlayerStructurePlacementState.Placing
        )!.currentInstance
      );
    }
  }

  public PlayerMenuState GetPlayerMenuState()
  {
    return playerMenuState;
  }

  public PlayerStructurePlacementState GetPlayerStructurePlacementState()
  {
    return playerStructurePlacementState;
  }

  void HandleLookInput()
  {
    var targetInputVector = playerMenuState is PlayerMenuState.None ?
      lookInputVector : Vector2.zero;

    var lerpFactor = isZoomed ? 0.01f : lookSmoothing;
    var sensitivityMultiplier = isZoomed ? 5f : 10f;

    processedLookInputVector = Vector2.Lerp(
      processedLookInputVector,
      targetInputVector,
      lerpFactor
    );

    var inputVector = processedLookInputVector;

    var x = inputVector.x * lookSensitivity * Time.deltaTime *
      sensitivityMultiplier;
    var y = inputVector.y * lookSensitivity * Time.deltaTime *
      sensitivityMultiplier;

    lookPitch = Mathf.Clamp(lookPitch - y, -90f, 90f);

    if (cameraTransform != null)
    {
      cameraTransform.localRotation =
        Quaternion.Euler(Vector3.right * lookPitch);
    }

    transform.Rotate(Vector3.up, x);
  }

  void HandleMovementInput()
  {
    var targetInputVector = playerMenuState is PlayerMenuState.None ?
      movementInputVector : Vector2.zero;

    processedMovementInputVector = Vector2.Lerp(
      processedMovementInputVector,
      targetInputVector,
      movementSmoothing
    );

    var inputVector = processedMovementInputVector;

    var move = inputVector.y * transform.forward + inputVector.x *
      transform.right;

    if (controller != null)
    {
      controller.SimpleMove(Vector3.ClampMagnitude(move, 1f) * 6f);
    }
  }

  void HandleTurretPlacement()
  {
    if (
      !(playerStructurePlacementState
        is PlayerStructurePlacementState.Placing) ||
        cameraTransform == null
      ) return;

    RaycastHit hit;

    if (
      Physics.Raycast(
        cameraTransform.position + cameraTransform.forward,
        cameraTransform.forward,
        out hit,
        10f,
        LayerMasks.all - LayerMasks.ghost
      )
    )
    {
      (playerStructurePlacementState as PlayerStructurePlacementState.Placing)!
        .currentInstance.transform.position = hit.point;
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

  void PlaceTurret(GameObject turret)
  {
    Utils.SetLayerRecursively(turret, Layers.defaultLayer);

    SetPlayerStructurePlacementState(new PlayerStructurePlacementState.None());
  }

  public void SetPlayerMenuState(PlayerMenuState value)
  {
    playerMenuState = value;

    if (value is PlayerMenuState.None)
    {
      Utils.LockCursor();
    }
    else if (value is PlayerMenuState.Build)
    {
      if (menuScript != null)
      {
        menuScript.OpenMenu();

        Utils.UnlockCursor();
      }
    }
  }

  public void SetPlayerStructurePlacementState(
    PlayerStructurePlacementState value
  )
  {
    playerStructurePlacementState = value;

    if (value is PlayerStructurePlacementState.None)
    {

    }
    else if (value is PlayerStructurePlacementState.Placing)
    {
      BeginTurretPlacement();
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

    menuScript?.Configure(this);

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
    Camera.main.fieldOfView = fieldOfView;

    SetPlayerMenuState(new PlayerMenuState.None());
  }

  void Update()
  {
    HandleLookInput();
    HandleMovementInput();

    HandleTurretPlacement();
  }
}

using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class MenuScript : MonoBehaviour
{
  [SerializeField]
  GameObject? canvasGameObject;
  [SerializeField]
  GameObject? uiTilePrefab;
  [SerializeField]
  List<GameObject> turretPrefabs = new List<GameObject>();

  Canvas? canvas;
  GameObject? body;
  List<GameObject> currentUITiles = new List<GameObject>();
  PlayerScript? playerScript;

  void BeginTurretPlacement(GameObject turretPrefab)
  {
    if (playerScript == null)
    {
      Debug.Log("Player script (PlayerScript) is null.");

      return;
    }

    var originalPrefab = turretPrefab;

    var currentInstance = Instantiate(original: originalPrefab);

    Utils.SetLayerRecursively(currentInstance, Layers.ghost);

    playerScript.SetPlayerStructurePlacementState(
      new PlayerStructurePlacementState.Placing
      {
        currentInstance = currentInstance,
        originalPrefab = originalPrefab,
      }
    );
  }

  public void CloseMenu()
  {
    if (canvasGameObject != null)
    {
      canvasGameObject.SetActive(false);

      foreach (var item in currentUITiles)
      {
        Destroy(item);
      }
    }
  }

  public void Configure(PlayerScript script)
  {
    playerScript = script;
  }

  public void OpenMenu()
  {
    if (canvasGameObject != null)
    {
      if (body != null && uiTilePrefab != null)
      {
        foreach (var turretPrefab in turretPrefabs)
        {
          var uiTile = Instantiate(uiTilePrefab, body.transform);
          var script = uiTile.GetComponent<UITileScript>();

          script.Configure("Basic Turret", () =>
          {
            BeginTurretPlacement(turretPrefab: turretPrefab);
          });

          currentUITiles.Add(uiTile);
        }
      }

      canvasGameObject.SetActive(true);
    }
  }

  void Awake()
  {
    canvas = canvasGameObject?.GetComponent<Canvas>();
    body = canvasGameObject?.transform.Find("Panel")?.Find("Body")?.gameObject;

    if (body == null)
    {
      Debug.LogError("Body (GameObject) is null.");
    }

    if (canvas == null)
    {
      Debug.LogError("Canvas is null.");
    }

    if (canvasGameObject == null)
    {
      Debug.LogError("Canvas (GameObject) is null.");
    }

    if (uiTilePrefab == null)
    {
      Debug.LogError("UI tile prefab (GameObject) is null.");
    }
  }
}

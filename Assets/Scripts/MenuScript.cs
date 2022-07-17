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

  public void CloseMenu()
  {
    if (canvasGameObject != null)
    {
      Utils.LockCursor();

      canvasGameObject.SetActive(false);

      foreach (var item in currentUITiles)
      {
        Destroy(item);
      }
    }
  }

  public void OpenMenu()
  {
    if (canvasGameObject != null)
    {
      Utils.UnlockCursor();

      if (body != null && uiTilePrefab != null)
      {
        foreach (var turretPrefab in turretPrefabs)
        {
          var uiTile = Instantiate(uiTilePrefab, body.transform);
          var script = uiTile.GetComponent<UITileScript>();

          script.Configure("Basic Turret", () =>
          {
            Debug.Log("Spawn turret");
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

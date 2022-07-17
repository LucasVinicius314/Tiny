using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class MenuScript : MonoBehaviour
{
  [SerializeField]
  GameObject? canvasGameObject;
  [SerializeField]
  List<GameObject> turretPrefabs = new List<GameObject>();

  Canvas? canvas;

  public void CloseMenu()
  {
    if (canvasGameObject != null)
    {
      Utils.LockCursor();

      canvasGameObject.SetActive(false);
    }
  }

  public void OpenMenu()
  {
    if (canvasGameObject != null)
    {
      Utils.UnlockCursor();

      canvasGameObject.SetActive(true);
    }
  }

  void Awake()
  {
    canvas = canvasGameObject?.GetComponent<Canvas>();

    if (canvas == null)
    {
      Debug.LogError("Canvas is null.");
    }

    if (canvasGameObject == null)
    {
      Debug.LogError("Canvas (GameObject) is null.");
    }
  }
}

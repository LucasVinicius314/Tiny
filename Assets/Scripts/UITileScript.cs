using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class UITileScript : MonoBehaviour
{
  [SerializeField]
  Text? label;
  [SerializeField]
  Button? button;

  public void Configure(string text, Action onClick)
  {
    if (label != null)
    {
      label.text = text;
    }

    if (button != null)
    {
      button.onClick.AddListener(() => onClick());
    }
  }

  void Awake()
  {
    if (button == null)
    {
      Debug.LogError("Button (Button) is null.");
    }

    if (label == null)
    {
      Debug.LogError("Label (Text) is null.");
    }
  }
}

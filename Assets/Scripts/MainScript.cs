using UnityEngine;

#nullable enable

public class MainScript : MonoBehaviour
{
  [SerializeField]
  GameObject? playerPrefab;

  void Awake()
  {
    if (playerPrefab == null)
    {
      Debug.LogError("Player (GameObject) is null.");
    }
  }

  void Start()
  {
    if (playerPrefab != null)
    {
      var player = Instantiate(playerPrefab);
    }
  }
}

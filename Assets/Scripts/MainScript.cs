using UnityEngine;

#nullable enable

public class MainScript : MonoBehaviour
{
  [SerializeField]
  GameObject? playerPrefab;

  void Start()
  {
    if (playerPrefab != null)
    {
      var player = Instantiate(playerPrefab);
    }
  }
}

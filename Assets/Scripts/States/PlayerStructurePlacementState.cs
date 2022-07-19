using UnityEngine;

public abstract class PlayerStructurePlacementState
{
  public class None : PlayerStructurePlacementState { }

  public class Placing : PlayerStructurePlacementState
  {
    public GameObject originalPrefab;
    public GameObject currentInstance;
  }
}

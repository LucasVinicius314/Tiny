using UnityEngine;

public class Utils
{
  public static void LockCursor()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  public static void UnlockCursor()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }
}

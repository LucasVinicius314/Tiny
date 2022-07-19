using System;

public class LayerMasks
{
  public static int all = ~0;
  public static int none = 0;
  public static int defaultLayer = (int)Math.Pow(2, Layers.defaultLayer);
  public static int transparentFX = (int)Math.Pow(2, Layers.transparentFX);
  public static int ignoreRaycast = (int)Math.Pow(2, Layers.ignoreRaycast);
  public static int water = (int)Math.Pow(2, Layers.water);
  public static int ui = (int)Math.Pow(2, Layers.ui);
  public static int ghost = (int)Math.Pow(2, Layers.ghost);
}

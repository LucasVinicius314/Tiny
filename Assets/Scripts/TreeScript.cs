using UnityEngine;

public class TreeScript : MonoBehaviour
{
  [SerializeField]
  GameObject woodChunk;
  [SerializeField]
  GameObject leavesChunk;

  void Start()
  {
    Generate(5, gameObject);
  }

  void Update()
  {

  }

  void Generate(int level, GameObject localParent, float offset = 0, float yaw = 0, float pitch = 0)
  {
    var aspectRatio = 4f;
    var height = aspectRatio * level;

    var branch = new GameObject("Branch");

    branch.transform.SetParent(localParent.transform, false);
    branch.transform.Translate(localParent.transform.up * offset, Space.World);
    branch.transform.Rotate(Vector3.up, yaw);
    branch.transform.Rotate(Vector3.right, pitch);

    if (level == 0)
    {
      var leavesModel = Instantiate(leavesChunk, Vector3.zero, Quaternion.identity);

      leavesModel.name = "Leaves";
      leavesModel.transform.SetParent(branch.transform, false);
      leavesModel.transform.localScale = Vector3.one * Random.Range(6, 10) * 2;
      leavesModel.transform.Translate(Vector3.up * (height / 2), Space.Self);

      return;
    }

    var model = Instantiate(woodChunk, Vector3.zero, Quaternion.identity);

    model.name = "Model";
    model.transform.SetParent(branch.transform, false);
    model.transform.localScale = new Vector3 { x = 1, y = aspectRatio, z = 1 } * level;
    model.transform.Translate(Vector3.up * (height / 2), Space.Self);

    var branches = Random.Range(1, 4);

    var newYaw = Random.rotation.eulerAngles.y;

    var yawStep = 360 / branches;

    for (int index = 0; index <= branches; index++)
    {
      Generate(level - 1, branch, height, newYaw + index * (yawStep), 30);
    }
  }
}

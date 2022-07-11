using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#nullable enable

public class TurretScript : MonoBehaviour
{
  [SerializeField]
  [Min(0f)]
  float viewRange;

  [SerializeField]
  [Range(0f, 180f)]
  float maxPitchAngle;

  [SerializeField]
  [Range(0f, 180f)]
  float maxTurnSpeed;

  List<GameObject> targets = new List<GameObject>();
  GameObject? target;
  bool isTargetOutOfRange = false;
  bool isAimed = false;
  GameObject? head;

  void DebugDrawLineToTargets()
  {
    foreach (var target in targets)
    {
      if (target == null) continue;

      Debug.DrawLine(transform.position, target.transform.position);
    }
  }

  IEnumerator AutoUpdateTargetsCoroutine()
  {
    while (true)
    {
      UpdateTargets();

      yield return new WaitForSeconds(1f);
    }
  }

  IEnumerator AutoShootingCoroutine()
  {
    while (true)
    {
      if (ShouldShoot())
      {
        Shoot(target!);

        yield return new WaitForSeconds(1f);
      }
      else
      {
        yield return new WaitForEndOfFrame();
      }
    }
  }

  void OnTargetDeath()
  {
    UpdateTargets();
  }

  bool ShouldShoot()
  {
    return isAimed && target != null;
  }

  void UpdateTargets()
  {
    var allTargets = GameObject.FindGameObjectsWithTag("Enemy");

    var potentialTargets = allTargets.Where(v => (v.transform.position - transform.position).magnitude < viewRange);

    targets = potentialTargets.ToList();

    if (targets.Count == 0)
    {
      target = null;
    }
    else
    {
      target = targets.First();
    }
  }

  void Shoot(GameObject newTarget)
  {
    newTarget.SendMessage(DamageTakenMessage.message, new DamageTakenMessage { damage = 2f, source = gameObject });
  }

  void Aim()
  {
    if (target == null || head == null) return;

    var targetPoint = target.transform.position;

    var turret = head.transform;
    var hardpoint = turret.parent;

    var direction = targetPoint - turret.position;
    direction = Vector3.ProjectOnPlane(direction, hardpoint.up);
    var signedAngle = Vector3.SignedAngle(hardpoint.forward, direction, hardpoint.up);

    isTargetOutOfRange = false;
    if (Mathf.Abs(signedAngle) > maxPitchAngle)
    {
      isTargetOutOfRange = true;
      direction = hardpoint.rotation * Quaternion.Euler(0, Mathf.Clamp(signedAngle, -maxPitchAngle, maxPitchAngle), 0f) *
                  Vector3.forward;
    }

    var rotation = Quaternion.LookRotation(direction, hardpoint.up);

    isAimed = false;
    if (rotation == transform.rotation && !isTargetOutOfRange)
    {
      isAimed = true;
    }

    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, maxTurnSpeed * Time.deltaTime);
  }

  void OnDrawGizmos()
  {

#if UNITY_EDITOR
    if (head == null) return;

    var range = 20f;
    var dashLineSize = 2f;
    var turret = head.transform;
    var origin = turret.position;
    var hardpoint = turret.parent;

    if (!hardpoint) return;
    var from = Quaternion.AngleAxis(-maxPitchAngle, hardpoint.up) * hardpoint.forward;

    Handles.color = new Color(0, 1, 0, .2f);
    Handles.DrawSolidArc(origin, turret.up, from, maxPitchAngle * 2, range);

    if (target == null) return;

    var projection = Vector3.ProjectOnPlane(target.transform.position - turret.position, hardpoint.up);

    // projection line
    Handles.color = Color.white;
    Handles.DrawDottedLine(target.transform.position, turret.position + projection, dashLineSize);

    // do not draw target indicator when out of angle
    if (Vector3.Angle(hardpoint.forward, projection) > maxPitchAngle) return;

    // target line
    Handles.color = Color.red;
    Handles.DrawLine(turret.position, turret.position + projection);

    // range line
    Handles.color = Color.green;
    Handles.DrawWireArc(origin, turret.up, from, maxPitchAngle * 2, projection.magnitude);
    Handles.DrawSolidDisc(turret.position + projection, turret.up, .5f);
#endif
  }

  void Start()
  {
    head = transform.Find("Head")?.gameObject;

    StartCoroutine(AutoUpdateTargetsCoroutine());
    StartCoroutine(AutoShootingCoroutine());
  }

  void Update()
  {
    DebugDrawLineToTargets();

    Aim();
  }
}

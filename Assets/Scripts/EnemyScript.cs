using UnityEngine;

public class EnemyScript : MonoBehaviour
{
  [SerializeField]
  [Min(1f)]
  float healthPoints;

  void Die(GameObject source)
  {
    Destroy(gameObject);

    source.SendMessage(TargetDeathMessage.message);
  }

  void OnDamageTaken(DamageTakenMessage message)
  {
    var newHealthPoints = healthPoints - message.damage;

    if (newHealthPoints <= 0)
    {
      Die(message.source);

      return;
    }

    healthPoints = newHealthPoints;
  }
}

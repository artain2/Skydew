
using System.Collections.Generic;
using AppBootstrap.Runtime;
using UnityEngine;

namespace _SkyDew.Scripts.Fight
{
    public interface IAttackService
    {
        void Attack(AttackerInfo info, Vector2 attackerPos, Vector2 dir);
    }

    [Injectable]
    public class AttackService : IAttackService
    {
        public void Attack(AttackerInfo info, Vector2 attackerPos, Vector2 dir)
        {
            var difference = dir - attackerPos;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90f;
            var rotation = Quaternion.Euler(0f, 0f, rotationZ);
            var attack = GameObject.Instantiate(info.effect,attackerPos, rotation);
            
            attack.transform.Translate(Vector3.forward * info.attackDistance);
            var hits = new List<Collider2D>();
            var filter = new ContactFilter2D();
            attack.Collider.OverlapCollider(filter, hits);
            attack.Collider.enabled = false;
            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent<IDamageReceiver>(out var damageReceiver))
                    damageReceiver = hit.GetComponentInParent<IDamageReceiver>();
                
                if (damageReceiver == null)
                {
                    continue;
                }

                if (info.sender.Fraction == damageReceiver.Fraction)
                {
                    continue;
                }
                damageReceiver.TakeDamage(info.damage);
            }
        }
    }

    public class AttackerInfo
    {
        public IDamageReceiver sender;
        public FightEffect effect;
        public int damage;
        public float attackDistance;
    }

    public interface IDamageReceiver
    {
        void TakeDamage(int damage);
        string Fraction { get; }

    }
    
    
}

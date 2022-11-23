using UnityEngine;

namespace _SkyDew.Scripts.Fight
{
    public class AttackConfig : ScriptableObject
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _distance = 1f;
        [SerializeField] private FightEffect _effect;

        public int Damage => _damage;

        public float Distance => _distance;

        public FightEffect Effect => _effect;

        public AttackerInfo GetAttackInfo() => new()
        {
            damage = Damage,
            attackDistance = Distance,
            effect = Effect,
        };
    }
}
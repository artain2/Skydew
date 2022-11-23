using System;
using _SkyDew.Scripts.Fight;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Player
{
    public class Player : MonoBehaviour, IDamageReceiver
    {
        public event Action<int> OnDamageTaken;
        
        public void TakeDamage(int damage)
        {
            OnDamageTaken?.Invoke(damage);
        }

        public string Fraction => "Player";
    }
}
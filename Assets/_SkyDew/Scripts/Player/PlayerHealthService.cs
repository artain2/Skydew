using _SkyDew.Scripts.Fight;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Player
{
    [Injectable]
    public class PlayerHealthService
    {
        [Inject] private PlayerViewProvider _playerService;

        private Player _playerDamageObj;

        private int _hp = 100;

        [Init(InitSteps.Postload)]
        private void Init()
        {
            _playerDamageObj = _playerService.View.GetComponent<Player>();
            _playerDamageObj.OnDamageTaken += AtDamage;
        }

        private void AtDamage(int damage)
        {
            _hp -= damage;
            if (_hp <= 0)
            {
                Debug.Log("DEAD");
            }

            Debug.Log($"HP: {_hp}");
        }
    }
}
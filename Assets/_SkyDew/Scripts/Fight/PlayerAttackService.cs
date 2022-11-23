using _SkyDew.Scripts.Common;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UniRx;
using UnityEngine;

namespace _SkyDew.Scripts.Fight
{
    [Injectable]
    public class PlayerAttackService
    {
        [Inject] private IAttackService _attackService;
        [Inject] private AttackConfig _attackConfig;
        [Inject] private IPlayerViewProvider _playerViewProvider;
        [Inject] private ICameraService _cameraService;

        private CompositeDisposable _dispose = new();
        private const long AttackDelay = 100;
        private long _lastAttack;
        private IDamageReceiver _playerDamageReceiver;

        [Init(InitSteps.Postload)]
        private void Init()
        {
            Observable.EveryUpdate().Where(_ => Input.GetMouseButton(0)).Subscribe(AtMouseDown).AddTo(_dispose);
            _playerDamageReceiver = _playerViewProvider.View.GetComponent<IDamageReceiver>();
        }

        private void AtMouseDown(long tick)
        {
            if (_lastAttack + AttackDelay > tick)
            {
                return;
            }

            _lastAttack = tick;
            var attackInfo = _attackConfig.GetAttackInfo();
            attackInfo.sender = _playerDamageReceiver;
            var pos = (Vector2) _playerViewProvider.View.transform.position;
            var dir = (Vector2)_cameraService.Camera.ScreenToWorldPoint(Input.mousePosition);
            _attackService.Attack(attackInfo, pos, dir);
        }

    }
}
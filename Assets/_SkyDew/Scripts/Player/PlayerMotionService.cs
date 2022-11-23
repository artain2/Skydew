using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UniRx;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Player
{
    [Injectable]
    public class PlayerMotionService : IMotionInputSub
    {
        [Inject] private IPlayerViewProvider _playerViewProvider;

        private Vector2Int _currentDirection;
        private CompositeDisposable _dispose = new CompositeDisposable();
        private const float _motionSpeed = 4f;

        [Init(InitSteps.Postload)]
        private void Init()
        {
            Observable.EveryUpdate().Subscribe(MotionUpdate).AddTo(_dispose);
        }
        
        public void AtDirectionChange(Vector2Int direction)
        {
            _currentDirection = direction;
        }

        void MotionUpdate(long _)
        {
            if (_currentDirection == Vector2Int.zero) 
                return;
            var tr = _playerViewProvider.View.transform;
            var trans = new Vector3(_currentDirection.x, _currentDirection.y, 0f) * _motionSpeed * Time.deltaTime;
            tr.Translate(trans);
        }
    }
}
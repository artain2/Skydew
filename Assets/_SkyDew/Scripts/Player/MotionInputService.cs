using System.Collections.Generic;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UniRx;
using UnityEngine;

namespace _SkyDew.Scripts.Player
{
    public interface IMotionInputSub
    {
        public void AtDirectionChange(Vector2Int direction);
    }

    [Injectable]
    public class MotionInputService
    {
        [Inject] private List<IMotionInputSub> _motionInputSubs;

        private CompositeDisposable _dispose = new CompositeDisposable();
        private bool _motionEnable = false;
        private Vector2Int _motionDirection;

        [Init(InitSteps.Postload)]
        private void Init()
        {
            SetMotionEnable(true);
        }

        void KeysUpdate(long _)
        {
            var x = 0;
            var y = 0;
            if (Input.GetKey(KeyCode.A))
                x -= 1;
            if (Input.GetKey(KeyCode.S))
                y -= 1;
            if (Input.GetKey(KeyCode.W))
                y += 1;
            if (Input.GetKey(KeyCode.D))
                x += 1;
            var dir = new Vector2Int(x, y);
            if (dir != _motionDirection)
            {
                _motionDirection = dir;
                _motionInputSubs.ForEach(s=>s.AtDirectionChange(dir));
            }
        }

        public void SetMotionEnable(bool enable)
        {
            if (enable == _motionEnable)
            {
                return;
            }

            if (enable)
            {
                Observable.EveryUpdate().Subscribe(KeysUpdate).AddTo(_dispose);
                _motionEnable = true;
                return;
            }

            _dispose?.Dispose();
            _motionEnable = false;
        }
    }
}
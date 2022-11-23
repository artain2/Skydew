using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UniRx;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Common
{
    public interface ICameraService
    {
        Camera Camera { get; }
    }

    [Injectable]
    public class CameraService : ICameraService, IScreenChangeSub
    {
        [Inject] private IPlayerViewProvider _playerView;

        private Camera _camera;
        
        [Init(InitSteps.Preload)]
        private void Init()
        {
            _camera = Camera.main;
        }
        
        public Camera Camera => _camera;
        
        public void AtScreenActiveChange(string screen, bool active)
        {
            if (screen != Screens.Game)
                return;
            if (active)
                StartFollowCharacter();
        }
        
        private void StartFollowCharacter()
        {
            var camTr = _camera.transform;
            var playerTr = _playerView.View.transform;
            Observable.EveryUpdate().Subscribe(_ => { camTr.position = playerTr.position + Vector3.back * 10; });
        }

    }
}
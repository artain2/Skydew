using _SkyDew.Scripts.Common.UI;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace _SkyDew.Scripts.Common
{
    [Injectable]
    public class GameUiService : IScreenChangeSub
    {
        [Inject] private IUIService _uiService;
        [Inject] private IUIRootService _uiRootService;
        [Inject] private GameObject _gameUiPrefab;

        private GameObject _gameUi;

        [Init(InitSteps.Preload)]
        private void LoadPrefab()
        {
            var root = _uiRootService.UICanvas.transform;
            _gameUi = Object.Instantiate(_gameUiPrefab, root);
            _uiService.AddElement(_gameUi);
            _gameUi.SetActive(false);
        }


        public void AtScreenActiveChange(string screen, bool active)
        {
            if (screen != Screens.Game)
                return;
            _gameUi.SetActive(active);
        }
    }
}
using AppBootstrap.Runtime;
using UnityEngine;

namespace _SkyDew.Scripts.Common.UI
{
    public interface IUIRootService
    {
        Canvas UICanvas { get; }
    }

    [Injectable]
    public class UIRootService : IUIRootService
    {
        [Inject] private ICameraService _cameraService;
        [Inject] private IUIService _uiService;
        [Inject] private Canvas _canvasPrefab;

        public Canvas UICanvas { get; private set; }

        [Init("Preload")]
        private void Init()
        {
            UICanvas = Object.Instantiate(_canvasPrefab);
            UICanvas.worldCamera = _cameraService.Camera;
            _uiService.AddElement(UICanvas.gameObject);
        }
    }
}
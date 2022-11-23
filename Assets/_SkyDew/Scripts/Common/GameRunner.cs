using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;

namespace _SkyDew.Scripts.Common
{
    [Injectable]
    public class GameRunner
    {
        [Inject] private IScreenService _screenService;
        

        [Init(InitSteps.Run)]
        public void Run()
        {
            _screenService.LoadScreen(Screens.Game);
        }
    }
}
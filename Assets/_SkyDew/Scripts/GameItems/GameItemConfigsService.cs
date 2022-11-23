using System.Collections.Generic;
using System.Linq;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;

namespace _SkyDew.Scripts.GameItems
{
    public interface IGameItemConfigsService
    {
        IReadOnlyList<GameItemConfig> AllConfigs { get; }
        GameItemConfig GetConfig(string itemId);
    }

    [Injectable]
    public class GameItemConfigsService : IGameItemConfigsService
    {
        [Inject] private GameItemConfigsContainer _container;

        private Dictionary<string, GameItemConfig> _configsDict;

        [Init(InitSteps.Preload)]
        private void Init()
        {
            _configsDict = _container.Configs.ToDictionary(k => k.ItemID);
        }
        
        // ____ IGameItemConfigsService ______________________________________
        public IReadOnlyList<GameItemConfig> AllConfigs => _container.Configs;

        public GameItemConfig GetConfig(string itemId) => _configsDict[itemId];
    }
}
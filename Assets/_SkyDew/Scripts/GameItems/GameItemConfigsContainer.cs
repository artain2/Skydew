using System.Collections.Generic;
using UnityEngine;

namespace _SkyDew.Scripts.GameItems
{
    public class GameItemConfigsContainer : ScriptableObject
    {
        [SerializeField] private List<GameItemConfig> configs;

        public IReadOnlyList<GameItemConfig> Configs => configs;
    }
}
using System.Collections.Generic;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UnityEngine;

namespace _SkyDew.Scripts.Common
{
    public interface IApplicationQuitSub
    {
        void AtApplicationQuit();
    }

    [Injectable]
    public class ApplicationQuitService
    {
        [Inject] private List<IApplicationQuitSub> _subs;
        [Inject] private ApplicationQuitObserver _obsPrefab;

        [Init(InitSteps.Preload)]
        void Init()
        {
            var obs = Object.Instantiate(_obsPrefab);
            Object.DontDestroyOnLoad(obs);
            obs.OnQuit += () =>
            {
                _subs.ForEach(x=>x.AtApplicationQuit());
            };
        }

    }
}
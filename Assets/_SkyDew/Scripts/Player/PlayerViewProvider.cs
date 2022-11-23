using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using UnityEngine;

public interface IPlayerViewProvider
{
    public GameObject View { get; }
}

[Injectable]
public class PlayerViewProvider : IPlayerViewProvider
{
    [Inject] private GameObject _prefab;
    public GameObject View { get; private set; }

    [Init(InitSteps.Postload)]
    private void Init()
    {
        View = GameObject.Instantiate(_prefab);
        View.transform.localPosition = Vector3.zero;
    }
}
using UnityEngine;
using System;
using FMODUnity;
using FMOD.Studio;

[DefaultExecutionOrder(-1)]
public class Bootstrapper : PersistentSingleton<Bootstrapper>
{
    [SerializeField] SFXManager _sfxManager;
    [SerializeField] StudioEventEmitter _StudioEventEmitter;
    [SerializeField] ItemManager _itemManager;
    [SerializeField] ObjectPooler _objectPooler;
    [SerializeField] SceneTransitionManager _sceneTransitionManager;
   
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        ServiceLocator.Register(_sfxManager);
        ServiceLocator.Register(_StudioEventEmitter);
        ServiceLocator.Register(_itemManager);
        ServiceLocator.Register(_objectPooler);
        ServiceLocator.Register(_sceneTransitionManager);
    }
}


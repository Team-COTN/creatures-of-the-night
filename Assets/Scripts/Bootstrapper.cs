using UnityEngine;
using FMODUnity;

[DefaultExecutionOrder(-1)]
public class Bootstrapper : PersistentSingleton<Bootstrapper>
{
    [SerializeField] SFXManager _sfxManager;
    [SerializeField] StudioEventEmitter _StudioEventEmitter;
    [SerializeField] ItemManager _itemManager;
    [SerializeField] SceneTransitionManager _sceneTransitionManager;
   
    protected override void Awake()
    {
        transform.parent = null;
        base.Awake();
        if (Instance != this) return;

        ServiceLocator.Register(_sfxManager);
        ServiceLocator.Register(_StudioEventEmitter);
        ServiceLocator.Register(_itemManager);
        ServiceLocator.Register(_sceneTransitionManager);
    }
}


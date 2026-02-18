using UnityEngine;
using System;
using FMODUnity;
using FMOD.Studio;

[DefaultExecutionOrder(-1)]
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] SFXManager _sfxManager;
    [SerializeField] StudioEventEmitter _StudioEventEmitter;
    [SerializeField] ItemManager _itemManager;
    [SerializeField] ObjectPooler _objectPooler;
   
    private void Awake()
    {
        ServiceLocator.Register(_sfxManager);
        ServiceLocator.Register(_StudioEventEmitter);
        ServiceLocator.Register(_itemManager);
        ServiceLocator.Register(_objectPooler);
    }
}


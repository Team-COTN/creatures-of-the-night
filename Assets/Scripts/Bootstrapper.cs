using UnityEngine;
using System;
[DefaultExecutionOrder(-1)]
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] SFXManager _sfxManager;
    [SerializeField] ItemManager _itemManager;
    [SerializeField] ObjectPooler _objectPooler;
    
    private void Awake()
    {
        ServiceLocator.Register(_sfxManager);
        ServiceLocator.Register(_itemManager);
        ServiceLocator.Register(_objectPooler);

    }
}

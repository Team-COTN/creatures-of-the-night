using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicEnemy : MonoBehaviour
{
    private StudioEventEmitter studioEventEmitter;
    public string parameterName = "Combat_Intensity"; // Name defined in FMOD Studio
    public float enemyMode = 0.0f;
   
    void Awake()
    {
        studioEventEmitter = ServiceLocator.Get<StudioEventEmitter>();
    }
   
    void Update()
    {
        studioEventEmitter.SetParameter(parameterName, enemyMode);
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<ICharacter>() != null && collision.gameObject.TryGetComponent<IDamagable>(out IDamagable playerDamageable)) ;
            enemyMode++;
    }
}
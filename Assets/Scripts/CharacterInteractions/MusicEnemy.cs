using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicEnemy : MonoBehaviour
{
    FMOD.Studio.EventInstance BG_Music;
    public string parameterName = "Combat_Intensity"; // Name defined in FMOD Studio
    public float enemyMode = 0.0f;
    

    // Example of updating the parameter dynamically (e.g., every frame)
    void Start()
    {
        BG_Music = FMODUnity.RuntimeManager.CreateInstance("event:/Bg_Level_2");
    }
    
    void Update()
    {
        BG_Music.setParameterByName(parameterName, enemyMode);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable)) ;
            enemyMode++;
    }
    
}

using UnityEngine;

public class SafeGroundDamaged : MonoBehaviour
{
    //get character interaction goodies
    public CharacterInteractions CharacterInteractions;
    public SafeGroundCheckpoint SafeGroundCheckpoint;
    public GameObject player;

    private void OnEnable()
    {
        CharacterInteractions.AddCharacterDamagedObserver(TakeDamage);
    }
    //when something idk subscribe and something happen, call take damage
    private void OnDisable()
    {
        CharacterInteractions.RemoveCharacterDamagedObserver(TakeDamage);
    }


    public void TakeDamage(int characterHealth)
    {
        if (player != null)
        {
            WarpPlayer();
        }

    }
    public void WarpPlayer()
    {
        if (player != null && SafeGroundCheckpoint != null)
        {
            player.transform.position = SafeGroundCheckpoint.safeGroundLocation;
        }
        
    }

}

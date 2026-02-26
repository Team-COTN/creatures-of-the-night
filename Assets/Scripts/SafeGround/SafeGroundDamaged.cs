using UnityEngine;

public class SafeGroundDamaged : MonoBehaviour
{
    //get character interaction goodies
    public CharacterInteractions CharacterInteractions;
    public SafeGroundCheckpoint SafeGroundCheckpoint;
    public GameObject player;

    public Animator transition;

    private void OnEnable()
    {
        CharacterInteractions.AddCharacterDamagedObserver(TakeTeleportDamage);
    }
    //when something idk subscribe and something happen, call take damage
    private void OnDisable()
    {
        CharacterInteractions.RemoveCharacterDamagedObserver(TakeTeleportDamage);
    }


    public void TakeTeleportDamage(int characterHealth)
    {
        if (player != null)
        {
            transition.SetTrigger("DeathFade");
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

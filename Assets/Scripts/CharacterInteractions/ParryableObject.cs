using UnityEngine;

//Alex needs to work on this script
public class ParryableObject : MonoBehaviour, IParryable
{
    private Character character;
    public bool parryableNow;

    private void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }

    private void OnEnable()
    {
        parryableNow = true;
        character.AddCharacterStateObserver(Parry);
    }
    private void OnDisable()
    {
        character.AddCharacterStateObserver(Parry);
    }

    public void Parry(string charState)
    {
        if (charState == "JumpParry")
        {
            Debug.Log("Oh man, I got parried!");
            parryableNow = false;
            //make parryableNow false until a certain time has elapsed. 
        }
    }
    
    public bool GetParryableNowState()
    {
        return parryableNow;
    }
    
}

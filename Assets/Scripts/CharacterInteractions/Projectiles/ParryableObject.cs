using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Player;

//Alex needs to work on this script
public class ParryableObject : MonoBehaviour, IParryable
{
    private PlayerCharacterController character;
    public bool parryableNow = true;

    private Color originalColor;

    [SerializeField] private SpriteRenderer mySprite;

    [SerializeField] private VisualEffect burstVFX;

    public Color parryColor = new Color(72f, 75f, 85f);
    

    private void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        originalColor = mySprite.color;
    }
    

    public void Parry()
    {
        Debug.Log("Oh man, I got parried!");
        parryableNow = false;

        StartCoroutine(ParryCooldown());

        //hit --> grey
        //after, smoothly transition from blue --> grey. 
        //make parryableNow false until a certain time has elapsed. 
    }
    
    IEnumerator ParryCooldown()
    {
        Debug.Log("coroutine on");

        float duration = 2f;
        float realTime = 0f;

        mySprite.color = Color.black;
        burstVFX.SendEvent("OnParry");
        while (realTime < duration)
        {
            mySprite.color = Color.Lerp(parryColor, originalColor, realTime / duration);
            realTime += Time.deltaTime;
            yield return null;
        }
        parryableNow = true;
        Debug.Log("I retire");

    }
    
    public bool GetParryableNowState()
    {
        return parryableNow;
    }
    
}

using UnityEngine;
using System;
using System.Collections; //events 
using Player;
using Language.Lua;

public class CharacterInteractions : MonoBehaviour, IPlayerDamagable, IPlayerTeleportable, IPlayerShootable, ICharacter
{
    private Color playerColor;
    public Color hitColor;
    [SerializeField] private SpriteRenderer playerSprite;
    //---teleport variables---
    public Animator transition;
    public SafeGroundCheckpoint SafeGroundCheckpoint;

    [SerializeField] private PlayerCharacterController characterController;
    public int characterHealth = 3;
    public event Action<int> CharacterDamaged;
    public event Action<int> CharacterTeleported;


    public void AddCharacterDamagedObserver(Action<int> observer) { CharacterDamaged += observer; }
    public void RemoveCharacterDamagedObserver(Action<int> observer) { CharacterDamaged -= observer; }

    // public void AddCharacterTeleportedObserver(Action<int> observer) { CharacterTeleported += observer; }
    // public void RemoveCharacterTeleportedObserver(Action<int> observer) { CharacterTeleported -= observer; }

    private void Awake()
    {
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        playerColor = playerSprite.color;
    }


    public void PlayerTakeDamage(int damageAmount)
    {
        Damage(damageAmount);
        CharacterDamaged?.Invoke(characterHealth);
    }

    public void TakeShotDamage(int damageAmount)
    {
        characterHealth -= damageAmount;
        characterController.Damage();
        CharacterDamaged?.Invoke(characterHealth);
    }
    

    public void EnterDamage(Vector2 hazardPosition)
    {
        characterController._hazardPosition = hazardPosition;
    }

    public void PlayerTakeTeleportDamage(int damageAmount)
    {
        // Player Take Damage
        characterHealth -= damageAmount;
        CharacterDamaged?.Invoke(characterHealth);
        characterController.EnterCinematic(new Player.States.Cinematics.CinematicRequest());

        // Fade In
        var sceneTransitionUI = FindFirstObjectByType<SceneTransitionUI>();
        sceneTransitionUI.FadeIn();

        // Move Player
        characterController.SetPosition(SafeGroundCheckpoint.safeGroundLocation);
        StartCoroutine(DamagedColor());
        characterController.ExitCinematic();
    }

    private void Damage(int damage)
    {
        characterHealth -= damage;
        characterController.Damage();
        StartCoroutine(DamagedColor());
    }
    IEnumerator DamagedColor()
    {
        float duration = 2f;
        float realTime = 0f;
        while (realTime < duration)
        {
            playerSprite.color = Color.Lerp(hitColor, playerColor, realTime / duration);
            realTime += Time.deltaTime;
            yield return null;
        }
    }
}

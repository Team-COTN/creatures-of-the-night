using System.Collections;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public delegate void TransitionEvent();
    public event TransitionEvent OnFadeOut;
    private bool fadeOutComplete = false; 
    private bool _isTransitioning = false;

    public void CompleteFadeOut() => fadeOutComplete = true;

    public void TransitionScenes(string scene, SceneTransitionTrigger.DoorNumber doorNumber)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(Transition(scene, doorNumber));
    }

    private IEnumerator Transition(string scene, SceneTransitionTrigger.DoorNumber doorNumber)
    {
        // Invoke Fade Out
        fadeOutComplete = false;
        OnFadeOut?.Invoke();

        // Wait for scene transition
        while (!fadeOutComplete)
            yield return null;

        // Wait for scene load        
        AsyncOperation load = SceneManager.LoadSceneAsync(scene);
        while (!load.isDone)
            yield return null;

        // Get reference to player and door
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        var door = FindObjectsByType<SceneTransitionTrigger>(FindObjectsSortMode.None).FirstOrDefault(door => door.doorNumber == doorNumber);
        
        // Move player to door
        if (door.doorNumber != SceneTransitionTrigger.DoorNumber.None)
            player.transform.position = door.spawnPoint.position;

        _isTransitioning = false;
    }

}

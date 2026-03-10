using System.Collections;
using System.Linq;
using Player;
using UnityEngine;
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
        fadeOutComplete = false;
        OnFadeOut?.Invoke();

        while (!fadeOutComplete)
            yield return null;

        AsyncOperation load = SceneManager.LoadSceneAsync(scene);
        while (!load.isDone)
            yield return null;

        var door = FindObjectsByType<SceneTransitionTrigger>(FindObjectsSortMode.None)
            .FirstOrDefault(d => d.doorNumber == doorNumber);

        if (door != null)
            door.PlayEntranceCinematic();

        _isTransitioning = false;
    }
}

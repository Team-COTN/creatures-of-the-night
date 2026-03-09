using UnityEngine;
using MoreMountains.Feedbacks;

public class SceneTransitionUI : MonoBehaviour
{
    [SerializeField] MMF_Player fadeOutFeedback;
    [SerializeField] MMF_Player fadeInFeedback;
    SceneTransitionManager sceneTransitionManager;

    private void OnEnable()
    {
        sceneTransitionManager = ServiceLocator.Get<SceneTransitionManager>();
        sceneTransitionManager.OnFadeOut += FadeOut;
        fadeOutFeedback.Events.OnComplete.AddListener(sceneTransitionManager.CompleteFadeOut);
    }

    private void OnDisable()
    {
        sceneTransitionManager.OnFadeOut -= FadeOut;
        fadeOutFeedback.Events.OnComplete.RemoveListener(sceneTransitionManager.CompleteFadeOut);
    }

    public void FadeOut()
    {
        fadeOutFeedback.PlayFeedbacks();
    }
}
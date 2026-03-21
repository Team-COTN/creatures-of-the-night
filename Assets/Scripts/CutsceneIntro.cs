using UnityEngine;
using UnityEngine.Video;
using NaughtyAttributes;

public class CutsceneIntro : CutsceneManager
{
    SceneTransitionManager sceneTransitionManager;
    [Scene] [SerializeField] private string targetScene;
    [SerializeField] private SceneTransitionTrigger.DoorNumber targetDoor;

    protected override void Start()
    {
        base.Start();
        sceneTransitionManager = ServiceLocator.Get<SceneTransitionManager>();
    }

    protected override void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video Finished!");
        base.OnVideoFinished(vp);
        sceneTransitionManager.TransitionScenes(targetScene, targetDoor);
    }
}
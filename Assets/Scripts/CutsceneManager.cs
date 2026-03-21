using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using FMODUnity;
using NaughtyAttributes;

[RequireComponent(typeof(VideoPlayer))]
public class CutsceneManager : MonoBehaviour
{
    public enum CutscenePlayTrigger { None, OnStart, OnFadeIn, OnEnable, OnTriggerEnter2D }

    private VideoPlayer videoPlayer;
    [SerializeField] private EventReference audioEvent;

    [Header("Scene Transition")]
    [SerializeField] private bool transitionOnFinish = false;
    [Scene] [SerializeField] [ShowIf("transitionOnFinish")] private string targetScene;
    [SerializeField] [ShowIf("transitionOnFinish")] private SceneTransitionTrigger.DoorNumber targetDoor;

    [Header("Events")]
    public CutscenePlayTrigger PlayTrigger = CutscenePlayTrigger.None;
    [SerializeField] [ShowIf("isTrigger2D")] private string triggerTag = "Player";
    public UnityEvent OnCutsceneStarted;
    public UnityEvent OnCutsceneFinished;

    private bool isTrigger2D => PlayTrigger == CutscenePlayTrigger.OnTriggerEnter2D;

    private FMOD.Studio.EventInstance _audioInstance;
    private SceneTransitionManager _sceneTransitionManager;

    protected virtual void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (transitionOnFinish || PlayTrigger == CutscenePlayTrigger.OnFadeIn)
            _sceneTransitionManager = ServiceLocator.Get<SceneTransitionManager>();

        if (PlayTrigger == CutscenePlayTrigger.OnFadeIn)
            _sceneTransitionManager.OnFadeIn += Play;

        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoFinished;

        if (PlayTrigger == CutscenePlayTrigger.OnStart) Play();
    }

    protected virtual void OnEnable()
    {
        if (PlayTrigger == CutscenePlayTrigger.OnEnable) Play();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayTrigger == CutscenePlayTrigger.OnTriggerEnter2D && other.CompareTag(triggerTag)) Play();
    }

    protected virtual void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;

        if (PlayTrigger == CutscenePlayTrigger.OnFadeIn && _sceneTransitionManager != null)
            _sceneTransitionManager.OnFadeIn -= Play;

        if (_audioInstance.isValid())
        {
            _audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _audioInstance.release();
        }
    }

    [Button]
    public void Play()
    {
        _audioInstance = RuntimeManager.CreateInstance(audioEvent);
        _audioInstance.start();
        videoPlayer.Play();
        OnCutsceneStarted?.Invoke();
    }

    [Button]
    public void Stop()
    {
        videoPlayer.Stop();
        _audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _audioInstance.release();
    }

    protected virtual void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video Finished!");
        _audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _audioInstance.release();

        if (transitionOnFinish)
            _sceneTransitionManager.TransitionScenes(targetScene, targetDoor);

        OnCutsceneFinished?.Invoke();
    }
}
using UnityEditor.Media;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject cutscene;
    bool play = true;
    public bool playOnAwake;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.prepareCompleted += (player) =>
            {
                if (playOnAwake) PlayCutscene();
            };

            videoPlayer.Prepare();
        }
  
    }
    public void PlayCutscene()
    {
        cutscene.SetActive(true);
        videoPlayer.Play();
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            if (play)
            {
                PlayCutscene();
                play = false;
            }
        }

    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        vp.Stop();
        cutscene.SetActive(false);
    }
}

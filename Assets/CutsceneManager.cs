using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject cutscene;
    bool play = true;

    void Awake()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }


        cutscene.SetActive(false); 
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
        cutscene.SetActive(false);
    }
}

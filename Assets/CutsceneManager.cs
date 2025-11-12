using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    bool play = true;

    void Start()
    {

    }
    public void PlayCutscene()
    {
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
}

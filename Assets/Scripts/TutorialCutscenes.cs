using UnityEngine;
using UnityEngine.Playables;

public class TutorialCutscenes : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject tutorialGroup;
    private bool playing = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            tutorialGroup.SetActive(true);
            timeline.Play();
            Debug.Log("tutorial cutscene triggered entere giiiioo");
            
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            timeline.Stop();
            tutorialGroup.SetActive(false);
            //https://www.youtube.com/watch?v=Mn3veUb4hA0
        }
    }
}

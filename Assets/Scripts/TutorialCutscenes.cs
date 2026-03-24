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
        tutorialGroup.SetActive(false);
        //rese
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered with: " + other.gameObject.name);
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            Debug.Log("guy");
            
            
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("hey");
            tutorialGroup.SetActive(true);
            timeline.Play();
            
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            //timeline.Stop();
            //tutorialGroup.SetActive(false);
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("whatTheHeck");
            timeline.Stop();
            tutorialGroup.SetActive(false);

        }
    }
}

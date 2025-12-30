using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class NPC : MonoBehaviour
{
    [SerializeField] private NPC_SO_Base _npcSO;
    
    //Talk indicators
    private Renderer npcRenderer;
    public TextMeshProUGUI talkText;
    public float padding = 0.15f;

    //triggering Dialogue
    private bool inNpcRange = false;

    private bool DialogueWasPressedThisFrame => InputManager.GetDialogueWasPressedThisFrame();

    private DialogueManager _dialogueManager;
    
    void Awake()
    {
        _dialogueManager = ServiceLocator.Get<DialogueManager>();
        talkText.gameObject.SetActive(false);
        
        //this NPC's sprite
        npcRenderer = GetComponent<Renderer>();
        
        //Talk appears above head
        PositionText();
    }

    void PositionText()
    {
        Bounds bounds = npcRenderer.bounds;
        Vector3 topOfHead = new Vector3(
            transform.position.x,
            bounds.max.y + padding,
            transform.position.z
        );

        talkText.transform.position = topOfHead;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            talkText.gameObject.SetActive(true);
            inNpcRange = true;
        }
    }
    void Update()
    {
        if (inNpcRange)
        {
            if (DialogueWasPressedThisFrame)
            {
                if (FindFirstObjectByType<DialogueManager>().isTalking)
                {   
                    FindFirstObjectByType<DialogueManager>().DisplayNextSentence();
                }
                else
                {
                    FindFirstObjectByType<DialogueManager>().StartDialogue(_npcSO);
                }
            }

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            talkText.gameObject.SetActive(false);
            inNpcRange = false;
        }
    }
}

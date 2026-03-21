using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using Player;
using Player.States.Cinematics;

public class SceneTransitionTrigger : MonoBehaviour
{
    public enum DoorNumber { None, One, Two, Three, Four, Five, Six }

    public DoorNumber doorNumber;

    [Header("Transition Target")]
    [SerializeField] private bool transitionToScene = true;
    [Scene] [SerializeField] [ShowIf("transitionToScene")] private string targetScene;
    [SerializeField] [ShowIf("transitionToScene")] private DoorNumber targetDoor;

    [Header("Cinematic")]
    [SerializeField] Transform enterPoint;
    [SerializeField] Transform exitPoint;

    private SceneTransitionManager sceneTransitionManager;
    private PlayerCharacterController player;
    private Collider2D col;

    void Start()
    {
        if (transitionToScene)
            sceneTransitionManager = ServiceLocator.Get<SceneTransitionManager>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!transitionToScene) return;
        if (other.gameObject != player.gameObject) return;
        PlayExitCinematic();
    }

    public void PlayEntranceCinematic()
    {
        col.enabled = false;
        player.SetPosition(exitPoint.position);
        player.EnterCinematic(new CinematicRequest {
            MoveTarget = enterPoint.position,
            FaceRight = enterPoint.position.x > exitPoint.position.x,
            OnComplete = () => col.enabled = true
        });
    }

    public void PlayExitCinematic()
    {
        col.enabled = false;
        player.EnterCinematic(new CinematicRequest {
            MoveTarget = exitPoint.position,
            FaceRight = enterPoint.position.x < exitPoint.position.x,
        });

        if (transitionToScene)
            sceneTransitionManager.TransitionScenes(targetScene, targetDoor);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle labelStyle = GUI.skin.GetStyle("Label");
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position + Vector3.up * 1, doorNumber.ToString(), labelStyle);
    }
#endif
}
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using Player;

public class SceneTransitionTrigger : MonoBehaviour
{
    public enum DoorNumber
    {
        None,
        One,
        Two,
        Three,
        Four
    }

    public DoorNumber doorNumber;
    [Header("Transition Target")]
    [Scene] [SerializeField] private string targetScene;
    [SerializeField] private DoorNumber targetDoor;
    public Transform spawnPoint;
    private SceneTransitionManager sceneTransitionManager;
    private PlayerCharacterController player;

    void Start()
    {
        sceneTransitionManager = ServiceLocator.Get<SceneTransitionManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
    }

    [Button]
    public void Transition()
    {
        sceneTransitionManager.TransitionScenes(targetScene, targetDoor);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player.gameObject)
            Transition();
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

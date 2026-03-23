using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class DeveloperDebugCanvas : MonoBehaviour
{
    Canvas canvas;
    bool canvasToggle;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    void Start()
    {        
        HideDeveloperCanvas();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (canvas.enabled) HideDeveloperCanvas();
            else ShowDeveloperCanvas();
        }
    }

    public void HideDeveloperCanvas()
    {
        canvas.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ShowDeveloperCanvas()
    {
        canvas.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;        
    }

    public void JumpToScene(string scene, SceneTransitionTrigger.DoorNumber door) => ServiceLocator.Get<SceneTransitionManager>().TransitionScenes(scene, door);
    public void JumpToIntroCutscene() => JumpToScene("IntroCutscene", SceneTransitionTrigger.DoorNumber.None);
    public void JumpToDreamSequence() => JumpToScene("IntroSequence", SceneTransitionTrigger.DoorNumber.One);
    public void JumpToIntroEndCutscene() => JumpToScene("IntroEndCutscene", SceneTransitionTrigger.DoorNumber.None);
    public void JumpToIntroToStar() => JumpToScene("IntroToStar", SceneTransitionTrigger.DoorNumber.Four);
    public void JumpToIRLevel() => JumpToScene("IR_Level", SceneTransitionTrigger.DoorNumber.One);
    public void JumpToSkyLevel() => JumpToScene("SkyLevel", SceneTransitionTrigger.DoorNumber.One);
    public void JumpToStarLevel() => JumpToScene("Star_Level", SceneTransitionTrigger.DoorNumber.One);
}

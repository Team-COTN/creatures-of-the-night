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

}

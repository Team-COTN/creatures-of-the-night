using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleSaveLoad : MonoBehaviour
{
    public GameObject character;

    public void Save()
    {
        ES3.Save("Player Position", character.transform.position);
    }

    public void Load()
    {
        character.transform.position = ES3.Load("Player Position", Vector3.zero);
    }

    void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            Save();
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Load();
        }
    }
}

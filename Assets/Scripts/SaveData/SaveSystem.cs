using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class SaveSystem : MonoBehaviour
{
    private Character character;
    public CharacterInteractions characterInteractions;
    private int characterHealth;
    private void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        characterHealth = characterInteractions.characterHealth;
        /*SaveData saveData = new SaveData
        {
            health = 3,
        };
        string json = JsonUtility.ToJson(saveData);
        Debug.Log(json);

        SaveData loadedSave = JsonUtility.FromJson<SaveData>(json);
        Debug.Log(loadedSave.health);
        */
    }

//change to UI save button
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Load();
        }
    } 

    private void Save()
    {
        SaveData saveData = new SaveData
        {
            health = characterInteractions.characterHealth,
            position = character.transform.position,
        };

        string json = JsonUtility.ToJson(saveData);
        Debug.Log(json);
        File.WriteAllText(Application.persistentDataPath + "/save.txt", json);


    }
    private void Load()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string saveString = File.ReadAllText(Application.persistentDataPath + "/save.txt");
            Debug.Log("Loaded:" + saveString);

            SaveData saveData = JsonUtility.FromJson<SaveData>(saveString);

            //set player
            character.transform.position = saveData.position;

            //is it okay to have the health data re-written when saving?
            characterInteractions.characterHealth = saveData.health;
            
        }
    }

    private class SaveData
    {
        public int health;
        public Vector2 position;
    }

}


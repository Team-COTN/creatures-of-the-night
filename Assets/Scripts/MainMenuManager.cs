using System.Collections; //for "IEnumerator"
using System.Collections.Generic; //for "List"
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject loadingBar;
    [SerializeField] private GameObject[] objectsToHide;
    private Slider slider;
    
    [Header("Scenes to load")]
    [SerializeField] private SceneField persist;
    [SerializeField] private SceneField scene1;

    private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    
    private void Awake()
    {
        loadingBar.SetActive(false);
        slider = loadingBar.GetComponent<Slider>(); 
    }
    
    public void StartGame()
    {
        HideMenu();
        
        loadingBar.SetActive(true);

        scenesToLoad.Add(SceneManager.LoadSceneAsync(persist));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(scene1, LoadSceneMode.Additive));

        StartCoroutine(ProgressBar());
    }

    private void HideMenu()
    {
        Debug.Log("Should Hide Menu");
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            objectsToHide[i].SetActive(false);
        }
    }

    private IEnumerator ProgressBar()
    {
        float progress = 0f;
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                progress += scenesToLoad[i].progress;
                if (loadingBar != null)
                {
                    slider.value = progress / scenesToLoad.Count;
                }
                else
                {
                    Debug.Log("assign the loading bar!");
                }
                yield return null;
            }
            
        }
    }
}

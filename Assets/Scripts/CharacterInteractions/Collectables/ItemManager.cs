using UnityEngine;
using TMPro;

public class ItemManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int orbCount = 0;
    public TextMeshProUGUI orbText;
    //public GameObject orbPrefab;
    //public Transform orbParent;
    //public Transform orbStart;

    public void Collect(CollectableSOBase collectable)
    {
        if (collectable is CollectableOrbSO orb)
        {
            //ShowOrb();
            IncrementOrbCount(orb.OrbAmount);
        }
    }
    

   /* public void ShowOrb()
    {
        var orbObject=Instantiate(orbPrefab,orbParent);
        orbObject.transform.position = orbStart.transform.position;
    }*/

    void IncrementOrbCount(int orbAmount)
    {
        Debug.Log("Increneeetwaeg");
        orbCount += orbAmount;
        if (orbText != null)
        {
            Debug.Log("orbText != null");
            orbText.text = "Orbs: " + orbCount.ToString();
        }
        
        //update UI elemnt to = orbCount
    }
}
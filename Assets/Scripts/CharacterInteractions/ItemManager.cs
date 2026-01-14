using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private int orbCount;
    
    public void Collect(CollectableSOBase collectable)
    {
        if (collectable is CollectableOrbSO orb)
        {
            IncrementOrbCount(orb.OrbAmount);
        }
    }
    
    void IncrementOrbCount(int orbAmount)
    {
        orbCount += orbAmount;
        Debug.Log(orbCount);
        //update UI elemnt to = orbCount
    }
}
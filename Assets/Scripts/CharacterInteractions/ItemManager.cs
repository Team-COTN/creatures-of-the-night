using UnityEngine;

public class ItemManager : MonoBehaviour
{
    int orbCount = 0;

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
        //update UI elemnt to = orbCount
    }
}
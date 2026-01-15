using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    //these are the attributes pool objects have
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary; 
    
    //queue objects invisibly on Start
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    //actually spawn the object (visible) with specific pos. 
    public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("No pool found for tag: " + tag);
            return null;
        }
        
        GameObject poolObject = poolDictionary[tag].Dequeue();
        poolObject.SetActive(true);
        poolObject.transform.position = position;
        poolObject.transform.rotation = rotation;
        
        poolDictionary[tag].Enqueue(poolObject);
        
        return poolObject;
    }
}

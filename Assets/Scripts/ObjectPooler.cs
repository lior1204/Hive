using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;//singelton pooler
    private void Awake()
    {
        Instance = this;
    }


    [System.Serializable]
    private class Pool//pool class
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    [SerializeField] private List<Pool> pools;//types of pools - set from inspector
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach(Pool pool in pools)//for each pool add queue to dictionary
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)//instantiate objects acording to pool size and store in queue
            {
                GameObject obj = Instantiate(pool.prefab);

                obj.transform.parent = this.transform;
                objectPool.Enqueue(obj);
                obj.SetActive(false);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string objType,Vector3 position,Quaternion rotation)//get object from pool - like instantiating
    {
        GameObject obj= SpawnFromPool(objType);
        if (obj)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;
    }
    public GameObject SpawnFromPool(string objType)//get object from pool - like instantiating
    {
        if (poolDictionary.ContainsKey(objType))//check that pool exists
        {
            GameObject obj;
            if (poolDictionary[objType].Count > 0)
                obj = poolDictionary[objType].Dequeue();
            else
                obj = Instantiate(pools.First(pool => pool.tag == objType).prefab);
            obj.SetActive(true);
            //poolDictionary[objType].Enqueue(obj);//return object to end of queue
            return obj;
        }
        Debug.LogWarning("no object pool for type " + objType);
        return null;
    }
    public void ReturnToPool(string objType,GameObject obj)
    {
        if (poolDictionary.ContainsKey(objType))//check that pool exists
        {
            obj.transform.parent = this.transform;
            obj.SetActive(false);
            poolDictionary[objType].Enqueue(obj);
        }
    }

}

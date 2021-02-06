using System.Collections.Generic;
using UnityEngine;
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    private Dictionary<string, Queue<GameObject>> _poolDict = new Dictionary<string, Queue<GameObject>>();
    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetObject(GameObject gameObject)
    {
        if (_poolDict.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            if (objectList.Count == 0)
            {
                return CreateNewObject(gameObject);
            }

            GameObject obj = objectList.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return CreateNewObject(gameObject);
    }

    public void ReturnGameObject(GameObject gameObject)
    {
        if (_poolDict.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            objectList.Enqueue(gameObject);
        }
        else
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            objectQueue.Enqueue(gameObject);
            _poolDict.Add(gameObject.name, objectQueue);
        }
        gameObject.SetActive(false);
    }

    private GameObject CreateNewObject(GameObject gameObject)
    {
        GameObject obj = Instantiate(gameObject);
        obj.name = gameObject.name;
        return obj;
    }
}

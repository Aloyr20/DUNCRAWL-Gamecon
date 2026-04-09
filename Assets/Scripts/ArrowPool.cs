using UnityEngine;
using System.Collections.Generic;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance;
    public GameObject arrowPrefab;
    public int poolSize = 20;

    private Queue<GameObject> pool;

    void Start()
    {
        Instance = this;
        pool = new Queue<GameObject>();

        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowPool: arrowPrefab is not assigned in the inspector!");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            pool.Enqueue(arrow);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject arrow = null;

        while (pool.Count > 0)
        {
            arrow = pool.Dequeue();
            if (arrow != null)
            {
                break;
            }
            arrow = null;
        }

        if (arrow == null)
        {
            if (arrowPrefab == null)
            {
                Debug.LogError("ArrowPool: arrowPrefab is not assigned!");
                return null;
            }
            arrow = Instantiate(arrowPrefab);
        }

        arrow.transform.SetParent(null);
        arrow.transform.position = position;
        arrow.transform.rotation = rotation;

        arrow.SetActive(true);
        return arrow;
    }

    public void Return(GameObject arrow)
    {
        if (arrow == null)
        {
            return;
        }
        arrow.SetActive(false);
        pool.Enqueue(arrow);
    }
}
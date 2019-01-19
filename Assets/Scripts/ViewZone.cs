using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewZone : MonoBehaviour
{
    public event System.Action<Transform> OnEnter = t => { };
    public event System.Action<Transform> OnExit = t => { };

    public List<string> tags = new List<string>();

    private List<Transform> objectsInRange = new List<Transform>();


    public Transform GetObject()
    {
        while (objectsInRange.Count != 0)
        {
            if (!CheckObject(objectsInRange[0]))
            {
                objectsInRange.RemoveAt(0);
                continue;
            }

            return objectsInRange[0];
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform obj = collision.transform;
        if (CheckObject(obj))
        {
            objectsInRange.Add(obj);
            OnEnter(obj);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform obj = collision.transform;
        if (CheckObject(obj) && objectsInRange.Contains(obj))
        {
            objectsInRange.Remove(obj);
            OnExit(obj);
        }
    }

    private bool CheckObject(Transform obj)
    {
        if (obj == null) return false;

        foreach (var tag in tags)
        {
            if (obj.tag == tag) return true;
        }
        return false;
    }
}
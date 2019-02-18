using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    public class ObservedZone : MonoBehaviour
    {
        [HelpBox("Специальное поведение, необходимое для отслеживания объектов в зоне")]
        public bool callOnInsideEvent = false;
        public List<string> tags = new List<string>();

        [Space]
        public ZoneEvent OnEnter = new ZoneEvent();
        public ZoneEvent OnInside = new ZoneEvent();
        public ZoneEvent OnExit = new ZoneEvent();


        private List<Transform> objectsInRange = new List<Transform>();


        private void Update()
        {
            if (callOnInsideEvent)
            {
                for (int i = 0; i < objectsInRange.Count; i++)
                {
                    if (CheckObject(objectsInRange[i]))
                    {
                        OnInside.Invoke(objectsInRange[i]);
                    }
                    else
                    {
                        objectsInRange.RemoveAt(i);
                        i--;
                    }
                }
            }
        }


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

        public List<Transform> GetObjects()
        {
            List<Transform> result = new List<Transform>();

            for (int i = 0; i < objectsInRange.Count; i++)
            {
                if (CheckObject(objectsInRange[i]))
                {
                    result.Add(objectsInRange[i]);
                }
                else
                {
                    objectsInRange.RemoveAt(i);
                    i--;
                }
            }

            return result;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Transform obj = collision.transform;
            if (CheckObject(obj))
            {
                objectsInRange.Add(obj);
                OnEnter.Invoke(obj);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Transform obj = collision.transform;
            if (CheckObject(obj) && objectsInRange.Contains(obj))
            {
                objectsInRange.Remove(obj);
                OnExit.Invoke(obj);
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


        [System.Serializable]
        public class ZoneEvent : UnityEvent<Transform> { }
    }
}
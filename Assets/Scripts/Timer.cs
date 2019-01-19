using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private static Timer Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public static void Wait(float time, System.Action action)
    {
        Instance.StartCoroutine(Instance.WaitRoutine(time, action));
    }

    private IEnumerator WaitRoutine(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);

        action.Invoke();
    }
}

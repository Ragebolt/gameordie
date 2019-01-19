using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed;

    [SerializeField] private Transform cameraTransform;


    public static CameraController Instance { get; private set; }


	void Awake ()
    {
        Instance = this;
    }

	void LateUpdate ()
    {
        Vector3 targetPos = target.position;
        targetPos.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}

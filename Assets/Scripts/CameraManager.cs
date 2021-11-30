using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
[System.Serializable]
public struct CameraTransforms
{
    public Vector3 camPos;
    public Vector3 camRot;
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public GameObject player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public List<CinemachineVirtualCamera> vCams;

    public CinemachineBrain cinemachineBrain;
    public Camera mainCamera, uiCamera;

    private void Awake()
    {
        AssignInstance();
        mainCamera = GetComponent<Camera>();
    }

   


    public void ActivateCamera(int camera, Transform target = null, Transform followTarget = null)
    {
        for (int i = 0; i < vCams.Count; i++)
        {
            if (i != camera)
            {
                vCams[i].gameObject.SetActive(false);
            }
            else
            {
                if (target != null)
                    vCams[i].LookAt = target;

                if (target != null)
                    vCams[i].Follow = followTarget;

                vCams[i].gameObject.SetActive(true);
            }
        }
    }

    void AssignInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.transform.position + offset;
        //desiredPosition.x = Camera.main.transform.position.x;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition,  smoothSpeed);
        transform.position = smoothedPosition;

        
    }
}
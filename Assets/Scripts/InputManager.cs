using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public delegate void OnHoldEvent(bool isHeld);
    public event OnHoldEvent OnHold;

    public delegate void OnTapEvent();
    public delegate void OnStopEvent();

    public event OnTapEvent OnTap;
    public event OnStopEvent OnStop;

    public Vector3 startPos, endPos, uiStartPos, uiEndPos;
    public bool checkUI, isStationary;

    private void Awake()
    {
        AssignInstance();
    }

    private void Update()
    {
        //if (GameManager.Instance.CURRENTGAMEPLAYSTATE == GameManager.GamePlayState.Game)
        //{
        TouchEvent();
        //}
    }

    private void TouchEvent()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (checkUI && EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }
            Tap();
            SetStartPos(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (checkUI && EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }
            if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse X") == 0)
            {
                isStationary = true;
            }
            else
            {
                isStationary = false;
            }
            SetEndPos(Input.mousePosition);
            Hold(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetStartPos(Input.mousePosition);
            SetEndPos(Input.mousePosition);
            isStationary = true;
            Stop();
            Hold(false);
        }
#endif

#if UNITY_IPHONE || UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (checkUI && EventSystem.current.IsPointerOverGameObject(touch.fingerId) && EventSystem.current.currentSelectedGameObject != null)
                        {
                            return;
                        }
                        Tap();
                        SetStartPos(touch.position);
                    }
                    break;
                case TouchPhase.Moved:
                    {
                        if (checkUI && EventSystem.current.IsPointerOverGameObject(touch.fingerId) && EventSystem.current.currentSelectedGameObject != null)
                        {
                            return;
                        }
                        SetEndPos(touch.position);
                        isStationary = false;
                        Hold(true);
                    }
                    break;
                case TouchPhase.Stationary:
                    {
                        isStationary = true;
                        SetEndPos(Input.mousePosition);
                        Hold(true);
                    }
                    break;
                case TouchPhase.Ended:
                    {
                        SetStartPos(touch.position);
                        SetEndPos(touch.position);
                        isStationary = true;
                        Stop();
                        Hold(false);
                    }
                    break;
            }
        }
#endif
    }

    public bool GetRaycast(string layerMask, out RaycastHit raycastHit)
    {
        Vector3 pos = Input.mousePosition;
#if UNITY_IPHONE || UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            pos = touch.position;
        }
#endif
        Ray ray = Camera.main.ScreenPointToRay(pos);
        return Physics.Raycast(ray.origin, ray.direction, out raycastHit, float.MaxValue, LayerMask.GetMask(layerMask));
    }

    public void SetStartPos(Vector2 input)
    {
        startPos = Camera.main.ScreenToWorldPoint(new Vector3(input.x, input.y, Camera.main.transform.position.z));
        if (CameraManager.Instance.uiCamera)
        {
            uiStartPos = CameraManager.Instance.uiCamera.ScreenToWorldPoint(new Vector3(input.x, input.y, CameraManager.Instance.uiCamera.transform.position.z));
        }
    }

    public void SetEndPos(Vector2 input)
    {
        endPos = Camera.main.ScreenToWorldPoint(new Vector3(input.x, input.y, Camera.main.transform.position.z));
        if (CameraManager.Instance.uiCamera)
        {
            uiEndPos = CameraManager.Instance.uiCamera.ScreenToWorldPoint(new Vector3(input.x, input.y, CameraManager.Instance.uiCamera.transform.position.z));
        }
    }

    public void ResetMousePos()
    {
#if UNITY_EDITOR
        startPos = CameraManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraManager.Instance.mainCamera.transform.position.z));
        endPos = CameraManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraManager.Instance.mainCamera.transform.position.z));
#endif

#if UNITY_IPHONE || UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            startPos = CameraManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, CameraManager.Instance.mainCamera.transform.position.z));
            endPos = CameraManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, CameraManager.Instance.mainCamera.transform.position.z));
        }
#endif
    }

    public void Hold(bool isHeld)
    {
        if (OnHold != null)
        {
            OnHold.Invoke(isHeld);
        }
    }

    public void Tap()
    {
        if (OnTap != null)
        {
            OnTap.Invoke();
        }
    }

    public void Stop()
    {
        if (OnStop != null)
        {
            OnStop.Invoke();
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
}
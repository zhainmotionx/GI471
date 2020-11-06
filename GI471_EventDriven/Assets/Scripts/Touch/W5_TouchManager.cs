using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TouchData
{
    public int fingerId;
    public Vector2 position;
}

public class W5_TouchManager : MonoBehaviour
{
    public delegate void DelegateHandleTouch(TouchData touchData);

    public event DelegateHandleTouch OnTouchBegan;
    public event DelegateHandleTouch OnTouchMoved;
    public event DelegateHandleTouch OnTouchStationary;
    public event DelegateHandleTouch OnTouchEnded;
    public event DelegateHandleTouch OnTouchCancled;

    private static W5_TouchManager instance;

    public static W5_TouchManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<W5_TouchManager>();

                if(instance == null)
                {
                    Debug.LogError("Make sure have TouchManager on your scene");
                }
            }

            return instance;
        }
    }
    
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        for(int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);

            TouchData touchData;
            touchData.fingerId = touch.fingerId;
            touchData.position = touch.position;

            switch(touch.phase)
            {
                case TouchPhase.Began:
                {
                    if(OnTouchBegan != null)
                        OnTouchBegan(touchData);
                    break;
                }
                case TouchPhase.Moved:
                {
                    if(OnTouchMoved != null)
                        OnTouchMoved(touchData);
                    break;
                }
                case TouchPhase.Stationary:
                {
                    if(OnTouchStationary != null)
                        OnTouchStationary(touchData);
                    break;
                }
                case TouchPhase.Ended:
                {
                    if(OnTouchEnded != null)
                        OnTouchEnded(touchData);
                    break;
                }
                case TouchPhase.Canceled:
                {
                    if(OnTouchCancled != null)
                        OnTouchCancled(touchData);
                    break;
                }
            }
        }
#else

        TouchData touchData;
        touchData.fingerId = 0;
        touchData.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (OnTouchBegan != null)
                OnTouchBegan(touchData);
        }

        if(Input.GetMouseButton(0))
        {
            if (OnTouchMoved != null)
                OnTouchMoved(touchData);
        }

        if(Input.GetMouseButtonUp(0))
        {

            if (OnTouchEnded != null)
                OnTouchEnded(touchData);

            if (OnTouchCancled != null)
                OnTouchCancled(touchData);
        }
#endif
    }
}

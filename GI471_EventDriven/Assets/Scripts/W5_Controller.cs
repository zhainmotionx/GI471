using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class W5_Controller : MonoBehaviour
{
    public class KeyEvent
    {
        public List<KeyCode> keyCodeList;
        public UnityEvent eventPress;
        public UnityEvent eventRelease;
    }

    public class UnityAxisEvent : UnityEvent<float>
    {
        public float axis;
    }

    public class AxisEvent
    {
        public List<string> axisNameList;
        public UnityAxisEvent eventAxis;
    }

    public W5_InputMapping inputMapping;

    private Dictionary<string, KeyEvent> keyList = new Dictionary<string, KeyEvent>();

    private Dictionary<string, AxisEvent> axisList = new Dictionary<string, AxisEvent>();

    public virtual void Start()
    {
        foreach(var inputKey in inputMapping.inputKeyList)
        {
            InitKey(inputKey.keyName, inputKey.keyCodeList);
        }

        foreach(var inputAxis in inputMapping.inputAxisList)
        {
            InitAxis(inputAxis.axisName, inputAxis.axisList);
        }

        StartCoroutine(IEUpdateInput());
    }

    private IEnumerator IEUpdateInput()
    {
        while(true)
        {
            UpdateInputKey();
            UpdateInputAxis();
            yield return null;
        }
    }

    private void InitKey(string keyName, List<KeyCode> keyCodeList)
    {
        KeyEvent keyEvent = new KeyEvent();

        keyEvent.keyCodeList = keyCodeList;
        keyEvent.eventPress = new UnityEvent();
        keyEvent.eventRelease = new UnityEvent();

        keyList.Add(keyName, keyEvent);
    }

    private void InitAxis(string axisName, List<string> axisNameList)
    {
        AxisEvent axisEvent = new AxisEvent();

        axisEvent.axisNameList = axisNameList;
        axisEvent.eventAxis = new UnityAxisEvent();

        axisList.Add(axisName, axisEvent);
    }

    private void UpdateInputKey()
    {
        foreach(var key in keyList)
        {
            foreach(var keyCode in key.Value.keyCodeList)
            {
                if(Input.GetKeyDown(keyCode))
                {
                    key.Value.eventPress.Invoke();
                }

                if(Input.GetKeyUp(keyCode))
                {
                    key.Value.eventRelease.Invoke();
                }
            }
        }
    }

    private void UpdateInputAxis()
    {
        foreach(var axis in axisList)
        {
            foreach(var axisName in axis.Value.axisNameList)
            {
                axis.Value.eventAxis.Invoke(Input.GetAxis(axisName));
            }
        }
    }

    public void BindKey(string keyName, UnityAction actionPress, UnityAction actionRelease)
    {
        if (keyList.ContainsKey(keyName))
        {
            if(actionPress != null)
            {
                keyList[keyName].eventPress.AddListener(actionPress);
            }

            if(actionRelease != null)
            {
                keyList[keyName].eventRelease.AddListener(actionRelease);
            }
        }
        else
        {
            Debug.LogError("KeyName [" + keyName + "] is not match in InputMapping");
        }
    }

    public void BindAxis(string axisName, UnityAction<float> actionAxis)
    {
        if(axisList.ContainsKey(axisName))
        {
            if(actionAxis != null)
            {
                axisList[axisName].eventAxis.AddListener(actionAxis);
            }
        }
        else
        {
            Debug.LogError("AxisName [" + axisName + "] is not match in InputMapping");
        }
    }
}

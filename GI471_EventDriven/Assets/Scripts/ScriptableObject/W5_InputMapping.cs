using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="InputMapping")]
public class W5_InputMapping : ScriptableObject
{
    [System.Serializable]
    public class InputKeyMapping
    {
        public string keyName;
        public List<KeyCode> keyCodeList = new List<KeyCode>();
    }

    [System.Serializable]
    public class InputAxisMapping
    {
        public string axisName;
        public List<string> axisList = new List<string>();
    }

    public List<InputKeyMapping> inputKeyList = new List<InputKeyMapping>();

    public List<InputAxisMapping> inputAxisList = new List<InputAxisMapping>();
}

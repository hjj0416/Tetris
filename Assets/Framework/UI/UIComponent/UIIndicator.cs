using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIndicator : ToggleGroup
{
    private List<Toggle> toggles = new List<Toggle>(10);

    private Transform _parent;

    void Awake()
    {
        _parent = transform;
        InitToggles();
    }

    public void SetCount(int count)
    {
        if (_parent.childCount > count)
        {
            int c = _parent.childCount - count;
            for (int i = 0; i < c; i++)
            {
                DestroyImmediate(_parent.GetChild(0).gameObject);
            }
            InitToggles();
        }
        else if (_parent.childCount < count)
        {
            Transform child = _parent.GetChild(0);
            int c = count - _parent.childCount;
            for (int i = 0; i < c; i++)
            {
                GameObject go = Instantiate(child.gameObject);
                Transform tf = go.transform;
                tf.SetParent(_parent);
                tf.localScale = Vector3.one;
                tf.localEulerAngles = Vector3.zero;
                Toggle tog = go.GetComponent<Toggle>();
                tog.group = this;
            }
            InitToggles();
        }
    }

    public void SetIndex(int index)
    {
        if(index >=0 && index < toggles.Count)
            toggles[index].isOn = true;
    }

    private void InitToggles()
    {
        toggles.Clear();
        Toggle[] arr = GetComponentsInChildren<Toggle>();
        for (int i = 0; i < arr.Length; i++)
        {
            toggles.Add(arr[i]);
            arr[i].group = this;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using TheNextMoba.UI;
using UnityEngine;

public class SetUICamera : MonoBehaviour 
{
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
            canvas.worldCamera = UIManager.Instance.UICamera;
        Destroy(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UITip : MonoBehaviour {

    [SerializeField] Text text;
    private float time;

    public void ShowTip(string content)
    {
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(Vector3.one,0.2f);
        text.text = content;
        time = 2;
    }


    void Update()
    {
        time -= Time.deltaTime;
        if(time<=0)
        {
            gameObject.SetActive(false);
        }
    }
}

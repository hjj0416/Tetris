using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameData : MonoBehaviour {

    [SerializeField] Text scoreText;
    [SerializeField] Text timeText;
    private float gameTime;
    private int second;
    private int minute;

	// Use this for initialization
	void Start () {
        scoreText.text = "0";

        PlayerDataMgr.Instance.dispatcher.AddEventHandler<int>(PlayerDataEvent.SCORE_CHANGE, UpdateScore);
	}

    private void OnDestroy()
    {
        PlayerDataMgr.Instance.dispatcher.RemoveEventHandler<int>(PlayerDataEvent.SCORE_CHANGE,UpdateScore);
    }

    // Update is called once per frame
    void Update () {
        gameTime += Time.deltaTime;
        if (gameTime >= 60)
        {
            minute++;
            gameTime = gameTime%60;
        }
        second = (int)gameTime;
        timeText.text = string.Format("{0}:{1}", minute, second);
	}

    void UpdateScore(short evt,int count)
    {
        scoreText.text = count.ToString();
    }
}

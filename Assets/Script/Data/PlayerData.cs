using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public int score;

    public void AddScore(int count)
    {
        score += count ;
        PlayerDataMgr.Instance.dispatcher.SendEvent<int>(PlayerDataEvent.SCORE_CHANGE,score);
    }
}

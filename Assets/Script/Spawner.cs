using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject[] Groups;

    private void Start()
    {
        SpanNext();
    }

    //产生下一个方块
    public void SpanNext()
    {
        int i = Random.Range(0,Groups.Length);

        Instantiate(Groups[i],transform.position,Quaternion.identity);
    }
}

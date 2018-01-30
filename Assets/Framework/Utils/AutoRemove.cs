using UnityEngine;

public class AutoRemove : MonoBehaviour 
{

    public float duration = 1;



    float curTime = 0;
	void Update () 
    {
	    if(curTime > duration)
        {
            Destroy(gameObject);
        }
        else
        {
            curTime += Time.deltaTime;
        }
	}
}

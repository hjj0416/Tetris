using UnityEngine;

public class CountDownTimer
{

    private float mStartTime;
    private float mTotalTime;

    public void Set(float totalTime)
    {
        mStartTime = Time.realtimeSinceStartup;
        mTotalTime = totalTime;
    }

    public float GetTotalTime()
    {
        return mTotalTime;
    }

    public float GetLastTime()
    {
        return mTotalTime - (Time.realtimeSinceStartup - mStartTime);
    }

    public bool IsFinished()
    {
        return GetLastTime() <= 0;
    }
    
}

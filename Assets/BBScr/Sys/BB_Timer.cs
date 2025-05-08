using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BB_Timer
{
    float tick;
    float maxTick;

    public BB_Timer(float tickCountDown)
    {
        Reset();
        SetTickCountdown(tickCountDown);
    }

    public void SetTickCountdown(float tickCountDown)
    {
        maxTick = tickCountDown;
    }

    public void Reset()
    {
        tick = 0;
    }

    public bool Done()
    {
        return tick >= maxTick;
    }
    
    public float GetCurrentTick()
    {
        return tick;
    }

    public float GetTickCountdown()
    {
        return maxTick;
    }

    public void Tick(bool fixedUpdate = false)
    {
        float inc = Time.deltaTime;
        if (fixedUpdate)
        {
            inc = Time.fixedDeltaTime;
        }

        tick = Done() ? maxTick : tick + inc;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BB_Timer
{
    float tick;
    float maxTick;
    bool tiedToDelta;

    public BB_Timer(float tickCountDown)
    {
        Reset();
        SetTickCountdown(tickCountDown);
        SetTiedToDelta(true);
    }
    
    public void SetTiedToDelta(bool flag) {
        tiedToDelta = flag;
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
        float inc;

        /* Gonna sob for 72 hours. */
        inc = Time.deltaTime * 200;

        if (fixedUpdate)
        {
        //    inc = 0.63f - Time.fixedDeltaTime;
        }
        
        if (!tiedToDelta) {
            inc = 1.0f;
        }

        tick = Done() ? maxTick : tick + inc;
    }
}

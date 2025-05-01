using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_Timer
{
    float tick;
    float incrementer;
    float maxTick;

    public BB_Timer(float tickCountDown, float increment = 1)
    {
        Reset();
        SetTickCountdown(tickCountDown);
        SetIncrement(increment);
    }

    public void SetTickCountdown(float tickCountDown)
    {
        maxTick = tickCountDown;
    }

    public void SetIncrement(float increment)
    {
        incrementer = increment;
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

    public void Tick()
    {
        tick = Done() ? maxTick : tick + incrementer;
    }
}

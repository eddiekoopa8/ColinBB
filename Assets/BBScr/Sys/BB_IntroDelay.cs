using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class BB_IntroDelay : MonoBehaviour
{
    // Start is called before the first frame update
    BB_Timer timer;
    public string ScnName;
    void Start()
    {
        timer = new BB_Timer(2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer.Tick(true);
        if (timer.Done())
        {
            Debug.Log("change");
            ScnManager.Goto("BBScn/" + ScnName);
        }
    }
}

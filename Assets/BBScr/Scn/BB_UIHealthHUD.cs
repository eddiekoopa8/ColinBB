using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BB_UIHealthHUD : MonoBehaviour
{
    public TMP_Text health;
    public TMP_Text gotKeyText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        health.text = "x " + BB_ActPlayer.GetHealth().ToString();
        if (BB_ActPlayer.HasKey())
        {
            gotKeyText.text = "Got the key for the exit!";
        }
    }
}

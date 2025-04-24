using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BB_DialogMan : MonoBehaviour
{
    private TMP_Text s_text;

    void Start()
    {
        s_text = GameObject.Find("DIALOGTxt").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        s_text.text = "hiiii";
    }
}

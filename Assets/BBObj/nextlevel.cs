using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextlevel : MonoBehaviour
{
    public string SceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (BB_ActPlayer.CollidedNoCheck(collision))
        {
            if (SceneName == "CURR")
            {
                ScnManager.Reload();
            }
            else
            {
                ScnManager.Goto(SceneName);
            }
        }
    }
}

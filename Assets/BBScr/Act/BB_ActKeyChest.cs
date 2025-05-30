using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_ActKeyChest : BB_PhysicsObject
{
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
        if (BB_ActPlayer.Collided(collision))
        {
            if (BB_ActPlayer.IsDamaging() || BB_ActPlayer.Pounded())
            {
                BB_ActPlayer.FoundKey();
                Destroy(gameObject);
            }
        }
    }
}

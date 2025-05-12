using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BB_ActEnemy : BB_PhysicsObject
{
    float speed = 3f;
    BB_Timer stunTime;
    bool moving;
    bool stunned;
    public override void ActorStart()
    {
        stunTime = new BB_Timer(1000);
        moving = true;
        stunned = false;
    }

    public override void ActorUpdate()
    {
        
        if (moving && !stunned)
        {
            if ((isLeft && speed == -3) || (isRight && speed == 3))
            {
                speed = -speed;
            }
            rigidbody.velocityX = speed;
        }
        else
        {
            rigidbody.velocityX = 0;
        }

        if (stunned)
        {
            rigidbody.velocityX = 0;
            stunTime.Tick();
            if (stunTime.Done())
            {
                stunned = false;

                Debug.Log("stun over");
            }
        }

        if (BB_ActPlayer.Pounded())
        {
            Debug.Log("stun");
            stunned = true;
            stunTime.Reset();
            rigidbody.velocityY = 6;
            rigidbody.velocityX = 0;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (BB_ActPlayer.Collided(collision))
        {
            if (BB_ActPlayer.Charging())
            {
                BB_ActPlayer.ForceStopCharge();
                Destroy(gameObject);
            }
        }
    }
}

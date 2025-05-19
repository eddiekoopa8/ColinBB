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
    int dir;
    public override void ActorStart()
    {
        stunTime = new BB_Timer(1000);
        moving = true;
        stunned = false;
        dir = -1;
    }

    public override void ActorUpdate()
    {
        if (isLeft && dir == -1)
        {
            dir = 1;
        }
        else if (isRight && dir == 1)
        {
            dir = -1;
        }

        if (moving)
        {
            rigidbody.velocityX = speed * dir;
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
            if (BB_ActPlayer.IsDamaging() || BB_ActPlayer.Pounded())
            {
                BB_ActPlayer.ForceStopCharge();
                Destroy(gameObject);
            }
        }
    }
}

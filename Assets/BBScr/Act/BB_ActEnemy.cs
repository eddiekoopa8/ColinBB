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
    int dirTimeCool = 5;
    bool canDir = true;
    void FixedUpdate()
    {
        if (dirTimeCool++ >= 5)
        {
            dirTimeCool = 5;
            canDir = true;
        }
    }
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
            canDir = false;
            dirTimeCool = 0;
        }
        else if (isRight && dir == 1)
        {
            dir = -1;
            canDir = false;
            dirTimeCool = 0;
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
        
        renderer.flipX = dir == 1;
        renderer.flipY = stunned == true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (BB_ActPlayer.Collided(collision))
        {
            if (BB_ActPlayer.IsDamaging() || BB_ActPlayer.Pounded())
            {
                BB_ActPlayer.ForceStopCharge();
                ScnManager.Instance().SetCameraShakeLevel(0.25f);
                Destroy(gameObject);
            }
            else {
                BB_ActPlayer.ForceStopCharge(1, true, true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_ActPlayer : BB_PhysicsObject
{
    enum Dir
    {
        LEFT,
        RIGHT,
    };

    Dir direction = Dir.RIGHT;
    bool moving = false;
    bool jumping = false;
    bool pressJump = false;
    bool pressMoveKey()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
    }
    void playerPreLogic()
    {
        if (pressMoveKey())
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        if (isLeft && direction == Dir.LEFT)
        {
            moving = false;
        }

        // Get direction
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Dir.LEFT;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = Dir.RIGHT;
        }

        // Jumping state
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumping = true;
            pressJump = true;
        }
    }

    void playerLogic()
    {
        if (direction == Dir.LEFT && moving)
        {
            rigidbody.velocityX = -10;
        }

        if (direction == Dir.RIGHT && moving)
        {
            rigidbody.velocityX = 10;
        }

        if (!moving)
        {
            rigidbody.velocityX = 0;
        }

        if (jumping)
        {
            rigidbody.velocityY = 8;
        }
    }

    void playerPostLogic()
    {
        // Jumping check
        if (isGrounded && jumping)
        {
            jumping = false;
        }
        pressJump = false;
    }

    public override void ActorUpdate()
    {
        playerPreLogic();
        playerLogic();
        playerPostLogic();
    }
}

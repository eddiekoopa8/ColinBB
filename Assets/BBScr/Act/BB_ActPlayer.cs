using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class BB_ActPlayer : BB_PhysicsObject
{
    /* Some of this could be moved to BB_PhysicsObject */
    enum Dir
    {
        LEFT,
        RIGHT,
    };

    public float JumpHeight = 14.25f;
    public float NormalFallSpeed = 18.0f;
    public float PoundFallSpeed = 22.0f;
    public float XSpeed = 10.0f;
    public float XAcceleration = 0.225f;
    public float ChargeXSpeed = 22.0f;

    float currFallSpeed;

    Dir prevDirection;
    Dir direction = Dir.RIGHT;

    bool moving = false;
    bool restrictMoving = false;

    bool jumping = false;
    bool pressJump = false;

    bool charging = false;
    bool pressCharge = false;
    bool abortCharge = false;

    BB_Timer chargerTimer;

    bool pressMoveKey()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
    }
    void playerPreLogic()
    {
        if (pressMoveKey() && !restrictMoving)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        // Get direction
        if (Input.GetKey(KeyCode.LeftArrow) && !restrictMoving)
        {
            direction = Dir.LEFT;
        }
        if (Input.GetKey(KeyCode.RightArrow) && !restrictMoving)
        {
            direction = Dir.RIGHT;
        }

        // Jumping state
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumping = true;
            pressJump = true;
        }

        // Charging state
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !charging)
        {
            charging = true;
            abortCharge = false;
            pressCharge = true;
        }
    }

    bool compareSpeed(float speed)
    {
        return rigidbody.velocityX >= speed || rigidbody.velocityX <= -speed;
    }

    void playerLogic()
    {
        /* MOVING LEFT AND RIGHT */

        if (prevDirection != direction)
        {
            moving = false;
            rigidbody.velocityX = 0;
        }

        if (direction == Dir.LEFT && moving)
        {
            rigidbody.velocityX = rigidbody.velocityX <= -XSpeed ? -XSpeed : rigidbody.velocityX - XAcceleration;
        }

        if (direction == Dir.RIGHT && moving)
        {
            rigidbody.velocityX = rigidbody.velocityX >= XSpeed ? XSpeed : rigidbody.velocityX + XAcceleration;
        }

        if (!moving)
        {
            rigidbody.velocityX = 0;
        }

        /* JUMPING */

        if (jumping)
        {
            rigidbody.velocityY = JumpHeight;
            if (moving && compareSpeed(XSpeed))
            {
                rigidbody.velocityX += 2.15f;
            }
        }

        /* CHARGING */

        if (pressCharge)
        {
            chargerTimer.Reset();
        }

        if (charging)
        {
            /* CHARGE LOGIC */

            restrictMoving = true;
            if (direction == Dir.LEFT)
            {
                rigidbody.velocityX = -ChargeXSpeed;
            }
            else if (direction == Dir.RIGHT)
            {
                rigidbody.velocityX = ChargeXSpeed;
            }

            /* RESET CHARGE LOGIC */

            if (abortCharge)
            {
                charging = false;
                restrictMoving = false;
                chargerTimer.Reset();
                abortCharge = false;
            }

            if ((chargerTimer.Done() && isGrounded) || (isLeft || isRight))
            {
                abortCharge = true;
            }
        }

        chargerTimer.Tick();

        /* GRAVITY */

        if (rigidbody.velocityY <= -currFallSpeed)
        {
            rigidbody.velocityY = -currFallSpeed;
        }
    }

    void playerPostLogic()
    {
        // Jumping check
        if (isGrounded && jumping)
        {
            jumping = false;
        }

        // Reset press booleans
        pressJump = false;
        pressCharge = false;

        // Update previous direction
        if (prevDirection != direction)
        {
            prevDirection = direction;
        }
    }

    public override void ActorStart()
    {
        chargerTimer = new BB_Timer(100);

        currFallSpeed = NormalFallSpeed;
        prevDirection = direction;
    }

    public override void ActorUpdate()
    {
        playerPreLogic();
        playerLogic();
        playerPostLogic();
    }
}

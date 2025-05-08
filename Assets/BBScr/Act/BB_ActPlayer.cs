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
    public float CrouchJumpHeight = 8.56f;
    public float NormalFallSpeed = 18.0f;
    public float PoundLimitSpeed = 20.0f;
    public float XSpeed = 10.0f;
    public float XAcceleration = 0.225f;
    public float ChargeXSpeed = 22.0f;

    float currFallSpeed;

    Dir prevDirection;
    Dir direction = Dir.RIGHT;

    bool moving = false;
    bool pressMove = false;
    bool restrictMoving = false;

    bool jumping = false;
    bool pressJump = false;
    bool restrictJumping = false;

    bool charging = false;
    bool pressCharge = false;
    bool abortCharge = false;

    bool pounding = false;

    bool crouching = false;
    bool pressCrouch = false;

    bool knocked = false;

    BB_Timer chargerTimer;
    BB_Timer knockTimer;

    public override void ActorStart()
    {
        knockTimer = new BB_Timer(100);
        chargerTimer = new BB_Timer(100);

        currFallSpeed = NormalFallSpeed;
        prevDirection = direction;
    }

    bool hasPressMoveKey()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
    }
    int getDirectionNegate()
    {
        return direction == Dir.LEFT ? -1 : 1;
    }
    void playerPreLogic()
    {
        // Crouching
        if (Input.GetKey(KeyCode.DownArrow) && !moving && isGrounded)
        {
            if (!crouching)
            {
                pressCrouch = true;
            }
            crouching = true;
        }
        else
        {
            crouching = false;
        }

        // Moving state
        if (hasPressMoveKey() && !restrictMoving)
        {
            if (!moving)
            {
                pressMove = true;
            }
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !restrictJumping)
        {
            jumping = true;
            pressJump = true;
            isGrounded = false;
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

        if (pressJump)
        {
            if (crouching)
            {
                rigidbody.velocityY = CrouchJumpHeight;
                pounding = true;
            }
            else
            {
                rigidbody.velocityY = JumpHeight;
                if (moving && compareSpeed(XSpeed))
                {
                    rigidbody.velocityY += 2.0f;
                }
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
                if ((isLeft || isRight))
                {
                    knocked = true;
                }
                abortCharge = true;
                restrictMoving = true;
            }
        }

        chargerTimer.Tick();

        /* KNOCK OUT ON WALL DURING CHARGE */

        if (knocked)
        {
            restrictMoving = true;
            restrictJumping = true;

            knockTimer.Tick();

            float vel = (knockTimer.GetTickCountdown() - knockTimer.GetCurrentTick()) / 12;
            rigidbody.velocityX = vel * -getDirectionNegate();

            if (knockTimer.Done())
            {
                restrictMoving = false;
                restrictJumping = false;
                knocked = false;
                knockTimer.Reset();
            }
        }

        /* CROUCHING */



        /* GROUND POUNDING */

        if (pounding)
        {
            rigidbody.velocityX = (CrouchJumpHeight / 1.25f) * getDirectionNegate();

            restrictMoving = true;

            if (isGrounded)
            {
                if (previousVelocity.y <= -PoundLimitSpeed)
                {
                    Debug.Log("BOOOOOM !!!!!!!!!!!");
                    Debug.Log("was " + previousVelocity.y);
                    ScnManager.Instance().SetCameraShakeLevel(3);
                }
                restrictMoving = false;
                pounding = false;
            }
        }

        /* GRAVITY (if not POUNDING) */

        if (!pounding)
        {
            if (rigidbody.velocityY <= -currFallSpeed)
            {
                rigidbody.velocityY = -currFallSpeed;
            }
        }
    }

    bool fallAnim = false;
    void playerAnimByLogic()
    {
        if (pressMove)
        {
            Debug.Log("ColinAnim:MOVE_ANIM");
        }

        if (pressCharge)
        {
            Debug.Log("ColinAnim:CHARGE_ANIM");
        }

        if (pressCrouch)
        {
            Debug.Log("ColinAnim:CROUCH_ANIM");
        }

        if (pressJump && !isGrounded && !charging)
        {
            if (crouching)
            {
                Debug.Log("ColinAnim:CROUCH_JUMP_ANIM");
            }
            else
            {
                Debug.Log("ColinAnim:JUMP_ANIM");
            }
        }

        if (isGrounded && rigidbody.velocityY < 0 && fallAnim == false)
        {
            Debug.Log("ColinAnim:FALL_ANIM");
            fallAnim = true;
        }

        if (isGrounded)
        {
            fallAnim = false;
        }
    }

    void playerPostLogic()
    {
        playerAnimByLogic();

        // Jumping check
        if (isGrounded && jumping)
        {
            Debug.Log("landed !");
            jumping = false;
        }

        // Reset press booleans
        pressJump = false;
        pressCharge = false;
        pressCrouch = false;
        pressMove = false;

        // Update previous direction
        if (prevDirection != direction)
        {
            prevDirection = direction;
        }

        Vector2 vec = ScnManager.GetCamera().transform.position;
    }

    public override void ActorUpdate()
    {
        playerPreLogic();
        playerLogic();
        playerPostLogic();
    }
}

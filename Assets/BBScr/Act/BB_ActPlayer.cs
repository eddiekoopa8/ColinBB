using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_ActPlayer : BB_PhysicsObject
{
    /* Some of this could be moved to BB_PhysicsObject */
    enum Dir
    {
        LEFT,
        RIGHT,
    };
    
    Animator animator;

    public float JumpHeight = 15f;
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
    bool chargeKnock = false;
    bool pounding = false;
    bool damaged = false;

    bool crouching = false;
    bool pressCrouch = false;
    bool hadPounded = false;

    bool knocked = false;
    bool gotkey = false;
    
    int health = 0;

    BB_Timer chargerTimer;
    BB_Timer knockTimer;
    public static BB_ActPlayer GetInstance()
    {
        return GameObject.Find("ScnPlayer").GetComponent<BB_ActPlayer>();
    }
    public static GameObject GetObject()
    {
        return GameObject.Find("ScnPlayer");
    }
    public static bool HasKey()
    {
        return GetInstance().gotkey;
    }
    public static void FoundKey()
    {
        GetInstance().gotkey = true;
    }
    public static int GetHealth()
    {
        return GetInstance().health;
    }
    public static bool Collided(Collision2D col)
    {
        return col.gameObject == GetObject() && (GetInstance().isLeft || GetInstance().isRight || GetInstance().isGrounded);
    }
    public static bool CollidedNoCheck(Collision2D col)
    {
        return col.gameObject == GetObject();
    }
    public static bool Pounded()
    {
        return GetInstance().hadPounded;
    }
    public static bool IsDamaging()
    {
        return GetInstance().charging || GetInstance().chargeKnock || GetInstance().pounding;
    }
    public static void ForceStopCharge(float yvel = 0.0f, bool knocked = false, bool isdamage = false)
    {
        if (GetInstance().damaged) return;
        GetInstance().rigidbody.velocityY += yvel;
        if (knocked)
        {
            GetInstance().knocked = true;
        }
        GetInstance().abortCharge = true;
        GetInstance().charging = false;
        GetInstance().restrictMoving = true;
        
        if (isdamage)
        {
            GetInstance().health--;
            GetInstance().damaged = true;
            ScnManager.Instance().SetCameraShakeLevel(1);
        }

    }
    public override void ActorStart()
    {
        animator = GetComponent<Animator>();
        
        Debug.Assert(gameObject.name == "ScnPlayer", "player MUST be ScnPlayer. for GetInsance");

        knockTimer = new BB_Timer(100);
        chargerTimer = new BB_Timer(100);

        currFallSpeed = NormalFallSpeed;
        prevDirection = direction;
        health = 3;
        gotkey = false;
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
        // Reset shared settings
        hadPounded = false;

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
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isLeft && !isRight && !charging)
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
            rigidbody.velocityX = rigidbody.velocityX <= -XSpeed ? -XSpeed : rigidbody.velocityX - (XAcceleration*1.5f);
        }

        if (direction == Dir.RIGHT && moving)
        {
            rigidbody.velocityX = rigidbody.velocityX >= XSpeed ? XSpeed : rigidbody.velocityX + (XAcceleration*1.5f);
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
                if (charging)
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
                    charging = true;
                    chargeKnock = true;
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

            float vel = (knockTimer.GetTickCountdown() - knockTimer.GetCurrentTick()) / 25;
            rigidbody.velocityX = vel * -getDirectionNegate();

            if (knockTimer.Done())
            {
                chargeKnock = false;
                restrictMoving = false;
                restrictJumping = false;
                damaged = false;
                renderer.enabled = true;
                knocked = false;
                knockTimer.Reset();
            }
        }
        
        if (damaged)
        {
            renderer.enabled = !renderer.enabled;
        }
        
        if (health <= 0)
        {
            ScnManager.Reload();
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
                    hadPounded = true;
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
        if (!charging && !moving && !crouching && !pounding)
        {
            animator.Play("IDLE");
        }

        if (pressMove && !charging)
        {
            Debug.Log("ColinAnim:MOVE_ANIM");
            animator.Play("RUN");
        }

        if (pressCharge)
        {
            Debug.Log("ColinAnim:CHARGE_ANIM");
            animator.Play("CHARGE");
        }

        if (pressCrouch && !charging)
        {
            Debug.Log("ColinAnim:CROUCH_ANIM");
            animator.Play("IDLE");
        }

        if (pressJump && !isGrounded && !charging)
        {
            fallAnim = true;
            if (crouching)
            {
                Debug.Log("ColinAnim:CROUCH_JUMP_ANIM");
                animator.Play("POUND");
            }
            else
            {
                Debug.Log("ColinAnim:JUMP_ANIM");
                animator.Play("RUN");
            }
        }

        if (!isGrounded && !pressJump && fallAnim == false)
        {
            Debug.Log("ColinAnim:FALL_ANIM");
            animator.Play("RUN");
            fallAnim = true;
        }

        if (isGrounded)
        {
            fallAnim = false;
        }
        
        renderer.flipX = direction == Dir.LEFT;
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

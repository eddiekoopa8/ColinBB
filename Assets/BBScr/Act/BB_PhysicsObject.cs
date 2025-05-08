using System.Drawing;
using UnityEngine;
public class BB_PhysicsObject : MonoBehaviour
{
    protected bool isGrounded = false;
    protected bool isLeft = false;
    protected bool isRight = false;

    protected bool alwaysActive = false;

    protected bool offsetOnTop = false;

    Vector3 groundLeft = Vector3.zero;
    Vector3 groundRight = Vector3.zero;

    Vector3 leftUp = Vector3.zero;
    Vector3 leftDown = Vector3.zero;

    Vector3 rightUp = Vector3.zero;
    Vector3 rightDown = Vector3.zero;

    protected Rigidbody2D rigidbody = new Rigidbody2D();
    protected SpriteRenderer renderer = new SpriteRenderer();
    protected BoxCollider2D collide = new BoxCollider2D();

    protected Vector2 previousVelocity;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();

        ActorStart();

        if (alwaysActive == false)
        {
            enabled = false;
        }

        Debug.Log("BB_PhysicsObject(" + name + ") is ready.");
    }

    public virtual void ActorStart()
    {
        // Debug.Log("BB_PhysicsObject(" + name + ") : no init available");
    }

    private static float sBonkBound = 1.05f;

    private static bool sBonkLine(Vector3 s, Vector3 e, float bound = 0.0f)
    {
        Collider2D[] start;
        Collider2D[] end;

        start = Physics2D.OverlapCircleAll(s, bound == 0.0f ? sBonkBound : bound);
        end   = Physics2D.OverlapCircleAll(e, bound == 0.0f ? sBonkBound : bound);

        return start.Length > 1 || end.Length > 1;
    }

    private void Update()
    {
        if (isActiveAndEnabled == false)
        {
            return;
        }

        // This part took so long to figure out. the axis and offsetting is so confusing. I still am not 100% how this fully works.

        // For ground
        groundLeft = rigidbody.position + collide.offset;
        groundLeft.x -= (collide.size.x / 2) - 0.13f;
        groundLeft.y -= collide.size.y;

        groundRight = groundLeft;
        groundRight.x += collide.size.x - 0.25f;

        // For left side collision
        leftUp = rigidbody.position + collide.offset;
        leftUp.x -= collide.size.x / 2;
        leftUp.y += collide.size.y;
        leftDown = rigidbody.position + collide.offset;
        leftDown.x -= collide.size.x / 2;
        leftDown.y -= collide.size.y;

        // For right side collision
        rightUp = rigidbody.position;
        rightUp.x += collide.size.x / 2;
        rightUp.y += collide.size.y;
        rightDown = rightUp;
        rightDown.y -= collide.size.y * 2f;

        if (GameObject.Find("AAAA_2") != null && GameObject.Find("AAAA") != null)
        {
            GameObject.Find("AAAA").transform.position = groundLeft;
            GameObject.Find("AAAA_2").transform.position = groundRight;
        }

        if (GameObject.Find("AAAA_2 (1)") != null && GameObject.Find("AAAA (1)") != null)
        {
            GameObject.Find("AAAA (1)").transform.position = leftUp;
            GameObject.Find("AAAA_2 (1)").transform.position = leftDown;
        }

        if (GameObject.Find("AAAA_2 (2)") != null && GameObject.Find("AAAA (2)") != null)
        {
            GameObject.Find("AAAA (2)").transform.position = rightUp;
            GameObject.Find("AAAA_2 (2)").transform.position = rightDown;
        }

        isGrounded = sBonkLine(groundLeft, groundRight) && (rigidbody.velocityY <= 0.1f && rigidbody.velocityY >= -0.01f);
        isLeft = sBonkLine(leftUp, leftDown, 0.095f);
        isRight = sBonkLine(rightUp, rightDown, 0.095f);

        // DEBUGGING PURPOSES
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*Debug.Log("INFO");
            Debug.Log("========================");
            Debug.Log("rigid " + rigidbody.position);
            Debug.Log("groundLeft " + groundLeft);
            Debug.Log("groundRight " + groundRight);
            Debug.Log("collide.offset " + collide.offset);
            Debug.Log("collide.size " + collide.size);
            Debug.Log("isGrounded " + isGrounded);
            Debug.Log("rigidbody.velocity " + rigidbody.velocity);
            Debug.Log("========================");*/
        }

        ActorUpdate();

        previousVelocity = rigidbody.velocity;
    }

    public virtual void ActorUpdate()
    {
        // Debug.Log("uppdate!!!");
    }

    void OnBecameVisible()
    {
        if (alwaysActive == false)
        {
            enabled = true;
        }
    }
}
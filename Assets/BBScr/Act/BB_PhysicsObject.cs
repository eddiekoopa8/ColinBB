using System.Drawing;
using UnityEngine;
public class BB_PhysicsObject : MonoBehaviour
{
    protected bool isGrounded = false;
    protected bool isLeft = false;
    protected bool isRight = false;

    protected bool alwaysActive = false;

    protected bool offsetOnTop = false;

    //public Transform groundCheckLeft;
    //public Transform groundCheckRight;

    Vector3 groundLeft = Vector3.zero;
    Vector3 groundRight = Vector3.zero;

    Vector3 leftUp = Vector3.zero;
    Vector3 leftDown = Vector3.zero;

    Vector3 rightUp = Vector3.zero;
    Vector3 rightDown = Vector3.zero;

    protected Rigidbody2D rigidbody = new Rigidbody2D();
    protected SpriteRenderer renderer = new SpriteRenderer();
    protected BoxCollider2D collide = new BoxCollider2D();

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();

        ActorStart();

        Debug.Log("BB_PhysicsObject(" + name + ") is ready.");

        if (alwaysActive == false)
        {
            enabled = false;
        }
    }

    public virtual void ActorStart()
    {
        // Debug.Log("BB_PhysicsObject(" + name + ") : no init available");
    }

    private static float sBonkBound = 0.45f;

    private static bool sBonkLine(Vector3 s, Vector3 e, float bound = 0.0f)
    {
        Collider2D[] start;
        Collider2D[] end;

        if (bound == 0.0f)
        {
            bound = sBonkBound;
        }

        start = Physics2D.OverlapCircleAll(s, bound);
        end   = Physics2D.OverlapCircleAll(e, bound);

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
        groundLeft.y -= collide.size.y + 0.25f;

        groundRight = groundLeft;
        groundRight.x += collide.size.x - 0.25f;

        // For left side collision
        leftUp = rigidbody.position;
        leftUp.x -= collide.size.x / 2;
        leftUp.y += collide.size.y;
        leftDown = leftUp;
        leftDown.y -= collide.size.y * 2f;

        // For right side collision
        rightUp = rigidbody.position;
        rightUp.x += collide.size.x / 2;
        rightUp.y += collide.size.y;
        rightDown = rightUp;
        rightDown.y -= collide.size.y * 2f;

        /*if (GameObject.Find("groundLeftBlock") != null && GameObject.Find("groundRightBlock") != null)
        {
            GameObject.Find("groundLeftBlock").transform.position = rightUp;
            GameObject.Find("groundRightBlock").transform.position = rightDown;
        }*/

        isGrounded = sBonkLine(groundLeft, groundRight) && (rigidbody.velocityY >= -0.1f && rigidbody.velocityY <= 0.1f);
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
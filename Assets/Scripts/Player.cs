using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //Component
    private Rigidbody2D rb;
    private Animator anim;

    //Movement
    public int deathFloor;
    public float speed, runningSpeed, jumpPower, divePower;

    private float xMove, yMove;
    private bool running = false, jumping = false, midair = false, diving = false, climbing = false;

    //Animation States
    private enum STATE
    {
        IDLE, WALKING, RUNNING, JUMPING, FALLING, DIVING, CLIMBING
    }
    private STATE state = STATE.IDLE;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {

        //DATA
        if (rb.velocity.y != 0) midair = true;
        else {
            midair = false;
            climbing = false;
        }

        if (Input.GetKey(KeyCode.C)) running = true;
        else running = false;

        if (Input.GetKeyDown(KeyCode.V) && !midair || Input.GetKeyDown(KeyCode.V) && climbing)
        {
            jumping = true;
            climbing = false;
        }
        else jumping = false;

        //MOVEMENT
        xMove = Input.GetAxis("Horizontal") * speed;
        yMove = jumpPower;

        //Run
        if (running)
        {
            xMove *= runningSpeed;
            yMove *= 1.25f;
        }

        //Dive
        if (Input.GetKeyDown(KeyCode.V) && midair && !diving && state != STATE.CLIMBING)
        {
            if (transform.localScale.x > 0) xMove += divePower;
            else xMove -= divePower;
            diving = true;
        }

        if (!jumping) yMove = 0f;

        rb.AddForce(new Vector2(xMove, yMove), ForceMode2D.Force);

        checkFall();
    }

    void FixedUpdate()
    {
        animate();
    }

    void grounded()
    {
        if (rb.velocity.y > 0) state = STATE.JUMPING;
        else if (rb.velocity.y < 0) state = STATE.FALLING;
        else state = STATE.IDLE;
    }

    void airborn()
    {
        if (running) state = STATE.RUNNING;
        else if (xMove != 0) state = STATE.WALKING;
        else state = STATE.IDLE;
    }

    void animate()
    {
        if (diving) state = STATE.DIVING;
        else if (!midair) airborn();
        else if (!climbing) grounded();
        else state = STATE.CLIMBING;

        //Animation Set
        Vector3 flip = transform.localScale;
        if (xMove > 0 && flip.x < 0) flip.x *= -1;
        else if (xMove < 0 && flip.x > 0) flip.x *= -1;
        transform.localScale = flip;

        anim.ResetTrigger("IDLE");
        anim.ResetTrigger("WALKING");
        anim.ResetTrigger("RUNNING");
        anim.ResetTrigger("JUMPING");
        anim.ResetTrigger("FALLING");
        anim.ResetTrigger("CLIMBING");
        anim.ResetTrigger("DIVING");

        anim.SetTrigger(state + "");
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            climbing = true;
            diving = false;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            climbing = false;
            midair = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Exit")
        {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
    }

    public string getState()
    {
        return state + "";
    }

    //DEATH
    void checkFall()
    {
        if (rb.position.y < deathFloor) Application.LoadLevel(Application.loadedLevel);
    }
}

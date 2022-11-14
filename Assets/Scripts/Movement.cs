using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public static Movement _instance { get; set; }

    //assignables
    [SerializeField]
    private Rigidbody2D rb;

    //movement
    public float speed, maxSpeed;

    public bool canMove = true;
    private float movement;

    //friction stuff
    private float friction = 10f;
    private float frictionThreshold = 0.35f;

    //jumping
    public float jumpForce;
    public float fallMultiplier;

    private bool canJump;
    private bool jump, ctrl;

    //die
    private float fadeDuration = 0;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Stats._instance.SetCheckpoint(transform.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) Stats._instance.ProgressGhost(10f);
        movement = 0;
        GetInputs();
        MoreGhost();
    }

    private void FixedUpdate()
    {
        if (!canMove) rb.gravityScale = 0f;
        else rb.gravityScale = 1f;
        Move();
    }


    private void GetInputs()
    {
        if (!canMove) return;
        if (rb.velocity.magnitude <= maxSpeed)
        movement = Input.GetAxisRaw("HorizontalAD");
        jump = Input.GetKey(KeyCode.Space);
        ctrl = Input.GetKey(KeyCode.LeftControl);
    }

    private void Move()
    {
        if (canJump && ctrl && jump) { PerformSoulJump(); return; }
        else if (canJump && jump) { PerformJump(); return; }

        float currSpeed = rb.velocity.magnitude;
        if (currSpeed > 0.001f)
        {
            float dropLim = Mathf.Max(currSpeed, frictionThreshold);
            float dropAmount = Mathf.Max(0, currSpeed - (dropLim * friction * Time.deltaTime));

            rb.velocity *= dropAmount / (currSpeed == 0 ? 0.1f : currSpeed);
        }
 
        movement *= speed;

        float yChange = 0;
        if (rb.velocity.y < 0)
            yChange = Physics2D.gravity.y * (fallMultiplier - Stats._instance.GetGhost()) * Time.deltaTime;

        Vector2 force = new Vector2(movement, yChange);

        rb.AddForce(force);

        if (GetComponent<Animator>().GetBool("Attack")) return;
        if (movement < 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = Vector3.one;

    }

    private void MoreGhost()
    {
        //more ghost = lower friction
        friction = 10f - Stats._instance.GetGhost() / 15f;
    }

    private void PerformJump()
    {
        canJump = false;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    }

    private void PerformSoulJump()
    {
        float soul = Stats._instance.GetSoul(); Stats._instance.UseSoul();

        rb.velocity += new Vector2(movement, (soul / 2.5f) + jumpForce/2f);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { 
            canJump = true; 
        }

        if (collision.gameObject.CompareTag("Danger"))
        {
            KillSelf();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
        }
    }

    public Rigidbody2D GetRB()
    {
        return rb;
    }

    public float GetDirection() {
        return movement; // speed;
    }

    public void KillSelf()
    {
        StartCoroutine("Die");
    }

    private float Lerp(float a, float b, float t) => a + (b - a) * t;

    IEnumerator Die()
    {
        canMove = false;
        Color sr = GetComponent<SpriteRenderer>().color;

        yield return new WaitForSeconds(0.15f);

        while (fadeDuration < 1f)
        {
            sr.a = Lerp(1f, 0.125f, fadeDuration);
            GetComponent<SpriteRenderer>().color = sr;
            fadeDuration += Time.deltaTime * 2f;
            yield return new WaitForSeconds(0.01f);
        }
        if (Stats._instance.GetLives() > 0) StartCoroutine("Respawn");

    }

    IEnumerator Respawn()
    {
        canMove = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        Color sr = GetComponent<SpriteRenderer>().color;

        yield return new WaitForSeconds(0.15f);

        transform.position = Stats._instance.GetCheckPoint();

        while (fadeDuration > 0f)
        {
            sr.a = Lerp(1f, 0.125f, fadeDuration);
            GetComponent<SpriteRenderer>().color = sr;
            fadeDuration -= Time.deltaTime * 2f;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<CapsuleCollider2D>().enabled = true;
        canMove = true;
    }

}

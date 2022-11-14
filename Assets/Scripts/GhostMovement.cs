using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public GameObject indicator;

    public CapsuleCollider2D cap;

    private Rigidbody2D rb;

    private Vector2 direction, iDirection;

    private bool showIndicator;
    private bool fade, canTP = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        indicator.transform.localPosition = Vector3.zero;
        GetInputs();
        if (showIndicator) ShowDirection();
        
    }

    private void FixedUpdate()
    {
        if(fade)
        Fade();
    }

    private void GetInputs()
    {
        direction.x = Input.GetAxisRaw("HorizontalAD");
        direction.y = Input.GetAxisRaw("Vertical");

        iDirection.x = Mathf.Abs(direction.x);
        iDirection.y = direction.y;

        showIndicator = Input.GetKey(KeyCode.E);
        fade = Input.GetKeyUp(KeyCode.E);
    }

    private void ShowDirection()
    {
        indicator.transform.localPosition = iDirection * 4;
        cap.offset = iDirection * Stats._instance.GetGhost() / 5f;
    }

    private void ResetTest()
    {
        cap.offset = Vector2.zero;
    }

    private void Fade()
    {
        //movement . canmove = false
        //lerp to new location for less snappy
        //translucent sprite image

        float ghostProgress = Stats._instance.GetGhost();

        //cap.offset = direction * ghostProgress/5f;


        if (canTP) { 
        //rb.MovePosition(transform.position + (new Vector3(direction.x, direction.y, 0) * ghostProgress / 4f));
            rb.position = transform.position + (new Vector3(direction.x, direction.y, 0) * ghostProgress / 4f);
        //    transform.position = transform.position + (new Vector3(direction.x, direction.y, 0) * ghostProgress / 5f); 
            this.enabled = false; Invoke("EnableSelf", 2f); }
    }

    private void OnEnable()
    {
        cap.enabled = true;
        cap.offset = Vector2.zero;
    }

    private void OnDisable()
    {
        cap.enabled = false;
        cap.offset = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canTP = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canTP = true;
    }

    private void EnableSelf()
    {
        enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Audio;

public class EnemyMovement : MonoBehaviour
{
    //roam state
    //detect player and chase state [steer towards player]
    //attack state
    //constantly check it's not falling off a platform

    enum State
    {
        roam,
        chase,
        attack
    }

    #region attack variables
    //does it attack?
    public bool attack;

    //what type of attack?
    public bool ranged; //false = meele, true = ranged

    public float attackSpeed;
    public float damage;
    public float attackRange;
    public float knock;

    public GameObject projectile;
    public float bulletSpeed;

    private float attackTimer;
    private bool canAttack;
    #endregion

    public string sound;

    private Rigidbody2D rb;
    private Animator anim;

    private int currentState;
    private bool detectedPlayer, closeEnoughToAttack;
    private GameObject player;

    #region roam variables

    public float moveSpeed;
    
    private Vector3 positionA, positionB;
    private Vector3 movementVector;

    private float desiredVelocity;

    private bool goingToA;
    
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        RecalcDestinations();
        currentState = (int)State.roam;

        if (Random.Range(0, 2) == 0) goingToA = true;
        moveSpeed /= Random.Range(1f, 1.25f);
    }

    private void Update()
    {
        if(!attack) { Travel(); return; }

        if (detectedPlayer) currentState = (int)State.chase;
        else if (currentState != (int)State.roam) { currentState = (int)State.roam; RecalcDestinations(); }
        if (closeEnoughToAttack) currentState = (int)State.attack;

        transform.localScale = new Vector3((movementVector.x < 0 ? -1 : 1), 1, 1);

        switch (currentState)
        {
            case 0:
                Travel();
                break;
            case 1:
                CheckDistance();
                break;
            case 2:
                Attack();
                break;
            default:
                break;
        }

        UpdateTimer();
    }


    private void FixedUpdate()
    {
        if (!closeEnoughToAttack) Move();
    }

    private void Move()
    {
        movementVector = movementVector * desiredVelocity * Time.deltaTime;

        transform.position += movementVector;

        if (goingToA)
        {
            if (Mathf.Abs(positionA.x - transform.position.x) <= 0.2f) goingToA = false;
        }
        else
        {
            if (Mathf.Abs(positionB.x - transform.position.x) <= 0.2f) goingToA = true;
        }

        
    }

    private void UpdateTimer()
    {
        if (attackTimer > attackSpeed) canAttack = true;
        else { canAttack = false; attackTimer += Time.deltaTime; }
    }

    //------------------------------------roam------------------------------------//

    private void RecalcDestinations() // call this everytime we switch state to roam
    {
        //raycast diagonallty 3 units to both sides and down
        //if hit, set that x value to be a point
        //if not hit, set current value to be a point

        if (Physics2D.Raycast(transform.position, ((Vector2.right * 5) - Vector2.down).normalized, 3f)) positionB = new Vector3(transform.position.x + Random.Range(1f,4f), transform.position.y, transform.position.z);
        else positionB = transform.position;

        if (Physics2D.Raycast(transform.position, ((Vector2.right * -5) - Vector2.down).normalized, 3f)) positionA = new Vector3(transform.position.x - Random.Range(1f, 5f), transform.position.y, transform.position.z);
        else positionA = transform.position;
    }

    /// <summary>
    /// Uses arrival steering behaviour to travel between two points
    /// </summary>
    private void Travel()
    {
        if (goingToA)
        {
            //move and stop at target
            //_movementVector = (target.transform.position - transform.position).normalized;
            //_desiredVelocity = Mathf.Clamp(Vector3.Distance(transform.position, target.transform.position), 0f, _maxVelocity);
            movementVector = (positionA - transform.position).normalized;
            desiredVelocity = Mathf.Clamp(Vector3.Distance(transform.position, positionA), 0f, moveSpeed);
        }
        else
        {
            //move and stop at other target
            movementVector = (positionB - transform.position).normalized;
            desiredVelocity = Mathf.Clamp(Vector3.Distance(transform.position, positionB), 0f, moveSpeed);
        }
    }


    //------------------------------------Detect and chase------------------------//

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        detectedPlayer = true;
        player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        detectedPlayer = false;
        closeEnoughToAttack = false;
        player = null;
        }
    }

    private void CheckDistance()
    {
        if (player == null) { closeEnoughToAttack = false; detectedPlayer = false; currentState = (int)State.roam; return; }
        if(Vector2.Distance(transform.position, player.transform.position) < attackRange)
        {
            closeEnoughToAttack = true;
        }
        else
        {
            closeEnoughToAttack = false;
            currentState = (int)State.chase;
            GoToPlayer();
        }
    }

    private void GoToPlayer()
    {
        //arrival behaviour to attack range
        movementVector = (player.transform.position - transform.position).normalized;
        desiredVelocity = Mathf.Clamp(Vector3.Distance(transform.position, positionA), 0f, moveSpeed);
    }


    //------------------------------------Attack----------------------------------//

    private void Attack()
    {
        if (!canAttack) return;
        canAttack = false; attackTimer = 0f;
        //anim.attack
        anim.SetBool("Attack", true);

        //AudioManager.Instance.Play(sound);

        if (!ranged)
        {
            //deal damage to player
            Stats._instance.TakeDamage(damage);

            Vector2 knockBack = (Vector2)(transform.position - player.transform.position).normalized + Vector2.up * knock;
            Vector2 knockBackPlayer = (Vector2)(player.transform.position - transform.position).normalized * knock * 1.5f + Vector2.up * (knock * 1.5f);

            rb.AddForce(knockBack, ForceMode2D.Impulse);
            player.GetComponent<Rigidbody2D>().AddForce(knockBackPlayer, ForceMode2D.Impulse);

            closeEnoughToAttack = false; detectedPlayer = false;
            currentState = (int)State.roam;
        }

        else
        {
            //spawn projectile
            GameObject bullet = Instantiate(projectile, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = ((player.transform.position - transform.position).normalized * bulletSpeed);
            bullet.GetComponent<EnemyShot>().damage = this.damage;
            bullet.GetComponent<EnemyShot>().knock = this.knock;
        }

        CheckDistance();
        Invoke("ResetAttackAnim", 0.4f);
    }

    private void ResetAttackAnim()
    {
        anim.SetBool("Attack", false);
    }

}

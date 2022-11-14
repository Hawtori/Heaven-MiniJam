using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Audio;

public class Weapon : MonoBehaviour
{
    public Animator anim;

    public GameObject VFX;

    public string sound;

    public float attackCD, knockBack;
    public float animMultiplier;
    public float range;
    public float damage;

    private float dirUp;

    private float attackTimer;
    private bool canAttack;

    private void OnEnable()
    {
        GetComponent<PickupWeapon>().enabled = false;
        
    }

    private void Update()
    {
        if (attackTimer >= attackCD) canAttack = true;
        else { attackTimer += Time.deltaTime; canAttack = false; }

        dirUp = Mathf.Max(0, Input.GetAxisRaw("Vertical"));

        canAttack = Movement._instance.canMove;

        if (canAttack && Input.GetKeyDown(KeyCode.LeftShift)) Attack();
    }

    private void Attack()
    {
        Movement._instance.canMove = false;
        Invoke("FinishAttack", 0.833f / animMultiplier);
        anim.speed = animMultiplier;
        anim.SetBool("Attack", true);

        float d = Movement._instance.GetDirection();
        d = (d == 0 ? 1 : d);

        RaycastHit2D[] hit = new RaycastHit2D[5];
        ContactFilter2D filter = new ContactFilter2D();

        filter = filter.NoFilter();

        int hits = Physics2D.CircleCast(transform.position, 2f, Vector2.right * d + Vector2.up * dirUp, filter, hit, range);

        int i;
        for(i = 0; i < hits; i++)
        {
            if (hit[i].transform.CompareTag("Enemy"))
            break;
        }

        if(i < 5)
        {
            //vfx
            GameObject fx = Instantiate(VFX, hit[i].transform.position, Quaternion.identity);
            fx.GetComponent<Animator>().speed = animMultiplier;
            fx.transform.localScale = transform.parent.localScale;
            Destroy(fx, 0.7f / animMultiplier);

            //play sound
            //AudioManager.Instance.Play(sound);

            //damage
            if(hit[i].transform.GetComponent<Health>() != null)
                hit[i].transform.GetComponent<Health>().TakeDamage(damage);

            //knockback
            hit[i].transform.GetComponent<Rigidbody2D>().AddForce((hit[i].transform.position - transform.position) * knockBack + Vector3.up * 5f, ForceMode2D.Impulse);
        }

    }

    private void FinishAttack()
    {
        Movement._instance.canMove = true;
        anim.SetBool("Attack", false);
        attackTimer = 0f;
    }
}

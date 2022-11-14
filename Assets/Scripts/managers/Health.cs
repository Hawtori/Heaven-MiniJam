using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    public float soul, ghost;

    private float fadeTime = 0f;

    private void Update()
    {
        if (health <= 0) StartCoroutine("Die");
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;  
    }

    private float Lerp(float a, float b, float time) => a + (b - a) * time;

    IEnumerator Die()
    {
        GetComponent<Animator>().SetBool("Dead", true);
        Color sr = GetComponent<SpriteRenderer>().color;

        yield return new WaitForSeconds(0.15f);

        while(fadeTime < 1f)
        {
            sr.a = Lerp(1f, 0f, fadeTime);
            GetComponent<SpriteRenderer>().color = sr;
            fadeTime += Time.deltaTime/5f;
        }

        //instantiate particles

        //player gaining particles

        Stats._instance.AddSouls(soul);
        Stats._instance.ProgressGhost(ghost);

        Destroy(gameObject);
        
    }
}

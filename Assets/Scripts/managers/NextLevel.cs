using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public Vector3 point;

    public TMP_Text tip;
    
    public GameObject currentLevel, nextLevel;
    
    private bool triggered = false;
    private GameObject player;
    
    private void Update()
    {
        tip.text = "";
        if (triggered)
        {
            tip.text = "Press F to go next";
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (nextLevel == null) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); return; }
                nextLevel.SetActive(true);
                player.transform.position = point;
                Stats._instance.SetCheckpoint(point);
                currentLevel.SetActive(false);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggered = true;
            player = collision.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggered = false;
        }
    }
}

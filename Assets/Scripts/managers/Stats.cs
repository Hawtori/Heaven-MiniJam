using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Player stats:
/// progress towards ghost and how much soul they have
/// </summary>
public class Stats : MonoBehaviour 
{
    public static Stats _instance { get; set; }

    #region UI stuff

    public Slider slider;
    public Gradient gradient;

    public Image soulFill;

    public Image ghost;

    public GameObject _healthBar;
    public List<GameObject> _hearts;

    #endregion

    #region effects

    public GameObject succ;

    #endregion


    private float soulAmount, ghostProgress;
    private float health = 50f;

    private int lives = 3;

    private Vector3 checkPoint;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);

        soulAmount = 0;
        ghostProgress = 0;
    }

    private void Start()
    {
        ShowHearts();
        AddSouls(1f);
        ProgressGhost(1f);
    }

    private void Update()
    {
        if(health <= 0)
        {
            Movement._instance.KillSelf();
            Revive();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        slider.value = health;
        ShowHealthBar();
    }


    public void AddSouls(float amount)
    {
        soulAmount += amount;
        soulAmount = Mathf.Clamp(soulAmount, 0, 50);

        GameObject p = Instantiate(succ, Movement._instance.transform.position, Quaternion.identity, Movement._instance.transform); Destroy(p, 0.65f);

        soulFill.color = gradient.Evaluate(soulAmount/50f);
    }

    public void ProgressGhost(float amount)
    {
        ghostProgress += amount;
        ghostProgress = Mathf.Clamp(ghostProgress, 0, 100);

        Color g = ghost.color;
        g.a = ghostProgress / 100f;
        ghost.color = g;
    }

    public void UseSoul()
    {
        soulAmount = 0;
        soulFill.color = gradient.Evaluate(soulAmount / 50f);
    }

    public void Revive()
    {
        HideHealthBar();
        UseSoul();
        ProgressGhost(-30f);

        lives--;
        health = 50f;
        ShowHearts();
        if(lives == 0) SceneManager.LoadScene("Dead");
    }

    public void SetCheckpoint(Vector3 checkpoint)
    {
        checkPoint = checkpoint;
    }

    private void ShowHearts()
    {
        for (int i = 0; i < lives; i++) _hearts[i].SetActive(true);

        Invoke("HideHearts", 1.2f);
    }

    private void HideHearts()
    {
        for (int i = 0; i < _hearts.Count; i++) _hearts[i].SetActive(false);
    }

    private void ShowHealthBar()
    {
        _healthBar.SetActive(true);

        Invoke("HideHealthBar", 1.4f);
    }

    private void HideHealthBar()
    {
        _healthBar.SetActive(false);
    }

    public float GetSoul() =>soulAmount;
    
    public float GetGhost() => ghostProgress;
    
    public int GetLives() => lives;

    public Vector3 GetCheckPoint() => checkPoint;
}

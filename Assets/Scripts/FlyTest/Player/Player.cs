using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject StartTimerUI;
    [SerializeField] TextMeshProUGUI startTimer;

    [SerializeField] QTEManager qteManager;
    [SerializeField] GameObject qteScreen;
    [SerializeField] GameOver gameover;

    public Collider2D Obstacle;
    public Rigidbody2D rb;
    public Animator anim;

    public bool flyAnimation = true;

    [SerializeField] SpriteRenderer shieldSprite;
    [SerializeField] SpriteRenderer windSprite;
    [SerializeField] PlayerJump playerjump;

    public StaminaSlider slider;
    public ObstacleSO obstacleData;
    public WallData wallData;
    public CollideItem item;

    public static Player Instance;

    public bool isInvincible = false;
    public bool onShield = false;

    public float stageFactor = 1f;


    private string currentTrigger = "";

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Start()
    {
        StartCoroutine(StartTime());
    }
    private IEnumerator StartTime()
    {
        StartTimerUI.SetActive(true);
        Time.timeScale = 0f;
        startTimer.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        startTimer.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        startTimer.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        StartTimerUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Update()
    {
        FlyAnimation();
        ItemCheck();
    }

    private void ItemCheck()
    {
        if (onShield)
        {
            shieldSprite.enabled = true;
        }
        else
        {
            shieldSprite.enabled = false;
        }

        if (playerjump.onWind)
        {
            windSprite.enabled = true;
        }
        else
        {
            windSprite.enabled = false;
        }
    }

    public void FlyAnimation()
    {
        float vy = rb.velocity.y;
        string newTrigger = null;

        if (flyAnimation)
        {
        if (vy >= 1f)
            newTrigger = "UpUp";
        else if (vy >= 0.5f)
            newTrigger = "Up";
        else if (vy <= -8f)
            newTrigger = "DownDown";
        else if (vy <= -2f)
            newTrigger = "Down";
        else if (vy >= -2f && vy <= -0.5f)
            newTrigger = "Fly";

            if (newTrigger != null && currentTrigger != newTrigger)
            {
                if (!string.IsNullOrEmpty(currentTrigger))
                    anim.ResetTrigger(currentTrigger);

                anim.SetTrigger(newTrigger);
                currentTrigger = newTrigger;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameover.gameover)
        {
            if (other.CompareTag("Wall") && !isInvincible)
            {
                if (other.TryGetComponent<CollideWall>(out var wall))
                {
                    int damage = wall.wallData.damage;
                    string name = wall.wallData.wallName;
                    StartCoroutine((HitWall(damage, name)));
                }
            }
            else if (other.CompareTag("Obstacle") && !isInvincible)
            {
                if (other.TryGetComponent<CollideObstacle>(out var obstacle))
                {
                    float coefficient = obstacle.obstacleData.Factor;
                    int damage = obstacle.obstacleData.damage;
                    StartCoroutine(HitInvincible(damage, coefficient));
                }
            }
            else if ((other.CompareTag("Obstacle") || other.CompareTag("Wall")) && isInvincible)
            {
                StartCoroutine(HitDuringInvincible());
            }
        }
    }

    public IEnumerator HitInvincible(int damage, float coefficient)
    {
        StartCoroutine(qteManager.StartQTETime(coefficient, damage));

        yield return new WaitForSecondsRealtime(5f);

            if (!qteManager.clearQTE)
            {
                Hit(damage);
            }
        isInvincible = true;
        yield return new WaitForSecondsRealtime(1f);
        isInvincible = false;
    }
    public IEnumerator HitWall(int damage, string name)
    {
        Hit(damage);
        Debug.Log($"{name}에 충돌하여 {damage}의 피해를 입었다.");
        isInvincible = true;
        yield return new WaitForSecondsRealtime(1f);
        isInvincible = false;
    }
    public IEnumerator HitDuringInvincible()
    {
        if (onShield)
        {
            SoundManager.instance.PlaySFX(9, 0);
        }
        yield return new WaitForSeconds(0.5f);
        onShield = false;
        isInvincible = false;
    }

    public void Hit(int damage)
    {
        SoundManager.instance.PlaySFX(0, 0);
        StartCoroutine(HitAnimation());
        onShield = false;
        slider.stamina -= damage;
        Debug.Log($"플레이어가 {damage} 만큼의 대미지를 입었습니다.");
    }

    private IEnumerator HitAnimation()
    {
        flyAnimation = false;
        anim.SetTrigger("Hit");
        yield return new WaitForSecondsRealtime(0.25f);
        flyAnimation = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    QTEManager qteManager;
    [SerializeField]
    GameObject qteScreen;

    public Collider2D Obstacle;
    public Rigidbody2D rb;
    public Animator anim;

    [SerializeField] SpriteRenderer shieldSprite;
    [SerializeField] SpriteRenderer windSprite;
    [SerializeField] PlayerJump playerjump;

    public StaminaSlider slider;
    public ObstacleSO obstacleData;
    public WallData wallData;
    public CollideItem item;

    public static Player Instance;

    public bool onCollide = false;
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
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
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

    public void OnTriggerEnter2D(Collider2D other)
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
        else if((other.CompareTag("Obstacle") || other.CompareTag("Wall")) && isInvincible)
            {
                StartCoroutine(HitDuringInvincible());
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
        onCollide = false;
    }
    public IEnumerator HitWall(int damage, string name)
    {
        Hit(damage);
        Debug.Log($"{name}에 충돌하여 {damage}의 피해를 입었다.");
        isInvincible = true;
        yield return new WaitForSecondsRealtime(1f);
        isInvincible = false;
        onCollide = false;
    }
    public IEnumerator HitDuringInvincible()
    {
        yield return new WaitForSeconds(1f);
        isInvincible = false;
        onShield = false;
        onCollide = false;
    }

    public void Hit(int damage)
    {
        anim.SetTrigger("Hit");
        onShield = false;
        slider.stamina -= damage;
        Debug.Log($"플레이어가 {damage} 만큼의 대미지를 입었습니다.");
    }
}

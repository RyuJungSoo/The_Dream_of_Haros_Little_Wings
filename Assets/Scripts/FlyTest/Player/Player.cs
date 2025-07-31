using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Collider2D Obstacle;
    public Rigidbody2D rb;
    public Animator anim;

    public StaminaSlider slider;
    public ObstacleSO obstacleData;
    public CollideItem item;

    public bool onCollide = false;
    public bool isInvincible = false;


    private string currentTrigger = "";

    public void Awake()
    {
        DontDestroyOnLoad(this);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        FlyAnimation();
    }
    public void FlyAnimation()
    {
        float vy = rb.velocity.y;
        string newTrigger = "";

        if (vy >= 1f)
            newTrigger = "UpUp";
        else if (vy >= 0.5f)
            newTrigger = "Up";
        else if (vy < -8f)
            newTrigger = "DownDown";
        else if (vy < -2f)
            newTrigger = "Down";
        else if (vy >= -2f && vy <= -0.5f)
            newTrigger = "Fly";

        if (!string.IsNullOrEmpty(newTrigger) && currentTrigger != newTrigger)
        {
            anim.ResetTrigger(currentTrigger);
            anim.SetTrigger(newTrigger);
            currentTrigger = newTrigger;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && !isInvincible)
        {
            if (other.TryGetComponent<CollideObstacle>(out var obstacle))
            {
                int damage = obstacle.obstacleData.damage;
                Debug.Log($"플레이어가 {obstacle.obstacleData.name} 와(과) 트리거 접촉했습니다.");
                StartCoroutine(HitInvincible(damage));
            }
        }
        else if(other.CompareTag("Obstacle") && isInvincible)
            {
                StartCoroutine(HitDuringInvincible());
            }
    }

    public IEnumerator HitInvincible(int damage)
    {
        Hit(damage);
        isInvincible = true;
        yield return new WaitForSeconds(1f);
        isInvincible = false;
        onCollide = false;
    }
    public IEnumerator HitDuringInvincible()
    {
        yield return new WaitForSeconds(1f);
        isInvincible = false;
        onCollide = false;
    }

    public void Hit(int damage)
    {
        item.onShield = false;
        slider.stamina -= damage;
        Debug.Log($"플레이어가 {damage} 만큼의 대미지를 입었습니다.");
    }
}

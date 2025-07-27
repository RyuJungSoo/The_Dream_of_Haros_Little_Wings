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

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        if(rb.velocity.y>=1)
        {
            Debug.Log("업업 실행");
            anim.SetTrigger(name="UpUp");
        }
        else if (rb.velocity.y < 1 && rb.velocity.y > 1/2)
        {
            Debug.Log("업 실행");
            anim.SetTrigger(name = "Up");
        }
        else if (rb.velocity.y >= -8 && rb.velocity.y < -2)
        {
            Debug.Log("다운 실행");
            anim.SetTrigger(name = "Down");
        }
        else if (rb.velocity.y < -8)
        {
            Debug.Log("다운다운 실행");
            anim.SetTrigger(name = "DownDown");
        }
        else if(rb.velocity.y >= -2 && rb.velocity.y <= -1/2)
        {
            Debug.Log("비행 실행");
            anim.SetTrigger(name = "Fly");
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
            if(other.CompareTag("Obstacle") && isInvincible)
            {
                StartCoroutine(HitDuringInvincible());
            }
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

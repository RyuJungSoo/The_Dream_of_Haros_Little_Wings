using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Collider2D Obstacle;

    public StaminaSlider slider;
    public ObstacleSO obstacleData;

    public bool onCollide = false;
    public bool isInvincible = false;

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
    }

    public IEnumerator HitInvincible(int damage)
    {
        Hit(damage);
        isInvincible = true;
        yield return new WaitForSeconds(1f);
        isInvincible = false;
        onCollide = false;
    }

    public void Hit(int damage)
    {
        slider.stamina -= damage;
        Debug.Log($"플레이어가 {damage} 만큼의 대미지를 입었습니다.");
    }
}

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
                Debug.Log($"�÷��̾ {obstacle.obstacleData.name} ��(��) Ʈ���� �����߽��ϴ�.");
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
        Debug.Log($"�÷��̾ {damage} ��ŭ�� ������� �Ծ����ϴ�.");
    }
}

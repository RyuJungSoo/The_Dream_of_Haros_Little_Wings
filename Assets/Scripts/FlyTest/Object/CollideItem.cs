using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Name
    {
        Wind,
        Bell,
        acorn
    }

public class CollideItem : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    public Name state;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ Item�� ȹ���߽��ϴ�.");
            if (state == Name.acorn)
            {

            }
            if (state == Name.Bell)
            {

            }
            if (state==Name.Wind)
            {
                
            }
        }
    }

}

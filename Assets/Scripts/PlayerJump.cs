using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb;

    [Header("���� ����")]
    public float jumpPower = 24f;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("�����̽��� �Է� ����, ���� ����");
        }
    }

    private void Jump()
    {
        
    }
}

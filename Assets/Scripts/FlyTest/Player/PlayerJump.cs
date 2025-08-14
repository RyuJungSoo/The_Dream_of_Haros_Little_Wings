using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public GameOver gameover;
    public CollideItem item;

    public bool onWind = false;
    public bool isJump = false;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] StatManager data;

    public float jumpPower;

    private void Awake()
    {
        jumpPower = StatManager.Instance.Total_FlightSpeed;
    }
    public void Update()
    {
        if (!gameover.gameover && !onWind && Input.GetKey(KeyCode.Space))
        {
            isJump = true;
            Jump();
        }
        else if(!gameover.gameover && onWind)
        {
            Debug.Log("윈드 실행");
            rb.velocity = new Vector2(rb.velocity.x, jumpPower * 2);
        }
        else
        {
            isJump = false;
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower / 3);
        // rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
    }
}

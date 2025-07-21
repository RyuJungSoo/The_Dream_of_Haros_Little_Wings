using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public GameOver gameover;
    public CollideItem item;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    Stat_Data data;

    public float jumpPower;

    private void Awake()
    {
        jumpPower = data.Total_FlightSpeed;
    }
    public void Update()
    {
        if (!gameover.gameover && Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        else if(!gameover.gameover && item.onWind)
        {
            Jump();
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        // rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Force);
    }
}

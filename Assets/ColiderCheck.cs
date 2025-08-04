using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiderCheck : MonoBehaviour
{
    float time = 0;

    void Awake()
    {
        time = 0;
    }
    private void Update()
    {
        time = Time.time;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{time} 초 입니다.");
        }
    }
}

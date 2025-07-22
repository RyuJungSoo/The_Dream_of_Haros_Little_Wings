using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField]
    CapsuleCollider2D col;

    void Start()
    {
        col = GetComponent<CapsuleCollider2D>();
    }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene("Raising_Stage");
            }
    }

}

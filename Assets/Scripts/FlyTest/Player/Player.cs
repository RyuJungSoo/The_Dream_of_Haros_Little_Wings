using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool onCollide = false;
    public bool isInvincible = false;
    void Start()
    {
        
    }

    void Update()
    {
        if(onCollide)
        {
            HitInvinclble();
        }
    }

    public IEnumerator HitInvinclble()
    {
        isInvincible = true;
        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
        onCollide = false;
    }
}

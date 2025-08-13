using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum PortalName
{
    Stage1Clear,
    Stage2Clear
}
public class Portal : MonoBehaviour
{
    [SerializeField]
    CapsuleCollider2D col;

    public GameClear gameclear;
    public PortalName portalState;

    void Start()
    {
        col = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (portalState == PortalName.Stage1Clear)
            {
                gameclear.Stage1Clear(); 
            }
            else if (portalState == PortalName.Stage2Clear)
            {
                gameclear.Stage2Clear();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideObstacle : MonoBehaviour
{
    public ObstacleSO obstacleData;

    private void Start()
    {
        Destroy(gameObject, 10f);
        Debug.Log("스타트 시작점");
    }

}

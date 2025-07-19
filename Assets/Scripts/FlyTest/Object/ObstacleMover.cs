using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public ObstacleSO obstacleData;
    public float moveSpeed = 3f;

    void Update()
    {
        Vector3 direction = GetDirection(obstacleData.moveType);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    Vector3 GetDirection(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.forward: return Vector3.right;
            case MoveType.fall: return Vector3.down;
            case MoveType.curve: return new Vector3(Mathf.Sin(Time.time), -1, 0).normalized;
            default: return Vector3.zero;
        }
    }
}
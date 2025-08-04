using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public ObstacleSO obstacleData;
    public float moveSpeed = 3f;

    private Vector3 curveStartPos;
    private float curveTime;
    private bool curveInitialized = false;

    void Update()
    {
        Vector3 direction = GetDirection(obstacleData.moveType);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    Vector3 GetDirection(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.forward:
                return Vector3.left;

            case MoveType.fall:
                return Vector3.down;

            case MoveType.curve:
                // 초기화: 시작 위치 저장
                if (!curveInitialized)
                {
                    curveStartPos = transform.position;
                    curveTime = 0f;
                    curveInitialized = true;
                }

                curveTime += Time.deltaTime;

                float x = -15f; // 왼쪽 이동
                float y = 4f - 9.8f * curveTime; // 초기에 올라갔다가 떨어지는 y 속도

                return new Vector3(x, y, 0).normalized;

            default:
                return Vector3.zero;
        }
    }
}

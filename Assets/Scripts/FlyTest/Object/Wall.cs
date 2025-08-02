using UnityEngine;

public class CollideWall : MonoBehaviour
{
    [Header("벽 데이터")]
    public WallData wallData;

    [Header("스프라이트 렌더러")]
    public SpriteRenderer sr;

    private void Awake()
    {
        ApplyWallData();
    }

    private void OnValidate()
    {
        ApplyWallData();
    }

    private void ApplyWallData()
    {
        if (wallData == null) return;

        if (sr != null)
            sr.sprite = wallData.sprite;
    }
}
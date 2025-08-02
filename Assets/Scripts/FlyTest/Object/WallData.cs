using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallData", menuName = "Data/WallData")]
public class WallData : ScriptableObject
{
    public string wallName;
    public int damage;
    public Sprite sprite;
}
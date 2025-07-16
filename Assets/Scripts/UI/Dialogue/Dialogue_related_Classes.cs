using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("��� ġ�� ĳ���� �̸�")]
    public string name;

    [Tooltip("��� ����")]
    public string[] contexts;

    [Tooltip("Sprite_ID")]
    public string[] Sprite_ID;
}

[System.Serializable]
public class DialogueEvent
{

    public string name; // �̺�Ʈ �̸�

    public Vector2 line; // x �ٺ��� y �ٱ��� ��縦 ������
    public Dialogue[] dialogues; // �� ����� ��縦 ġ�°� �ƴϱ� ������ �迭�� ����

}
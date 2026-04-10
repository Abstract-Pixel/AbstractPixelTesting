using AbstractPixel.Core;
using UnityEngine;

public class Booyah : MonoBehaviour
{
    [SerializeField,ReadOnly(true)]
    private int exampleField;
    [SerializeField, ReadOnly(true)]
    private Leaderboard leaderboard;
    [SerializeField, ReadOnly(true)]
    bool istesting = false;
}


[System.Serializable]
public class Leaderboard
{
    public int score;
    public Vector3 position;
    public string playerName;
    bool isLeaderboard = false;
}

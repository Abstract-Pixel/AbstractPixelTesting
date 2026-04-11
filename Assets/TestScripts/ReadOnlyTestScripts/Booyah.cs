using AbstractPixel.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Booyah : MonoBehaviour
{
    [SerializeField,ReadOnly(true)]
    private int exampleField;
    [SerializeField, ReadOnly(true)]
    private Leaderboard leaderboard;

}


[System.Serializable]
public class Leaderboard
{
    public int score;
    public Vector3 position;
    public string playerName;
    bool isLeaderboard = false;
}

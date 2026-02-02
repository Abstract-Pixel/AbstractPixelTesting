using AbstractPixel.Utility;
using AbstractPixel.Utility.Save;
using System;
using UnityEngine;


[Serializable]
public class leaderboardData
{
    public int bestTime;
    public int leaderboardCount;
    public int leaderboardIndex;

    public leaderboardData(int bestTime, int leaderboardCount, int leaderboardIndex)
    {
        this.bestTime = bestTime;
        this.leaderboardCount = leaderboardCount;
        this.leaderboardIndex = leaderboardIndex;
    }
}

[Saveable(SaveCategory.Game,"anotherFake")]
public class anotherFako : MonoBehaviour, ISaveable<leaderboardData>
{
    [SerializeField] leaderboardData example;

    public leaderboardData CaptureData()
    {
        return example;
    }

    public void RestoreData(leaderboardData _loadedData)
    {
        example = _loadedData;
    }
}



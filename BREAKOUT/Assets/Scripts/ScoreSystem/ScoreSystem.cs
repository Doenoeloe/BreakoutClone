using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static Action<int> AddScore;

    int score = 0;
    int highScore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddScore += ScoreAdd;
        highScore = PlayerPrefs.GetInt("highScore");
    }

    void ScoreAdd(int amount)
    {
        score += amount;

        if(highScore < score)
        {
            PlayerPrefs.SetInt("highScore", score);
            highScore = PlayerPrefs.GetInt("highScore");
            PlayerPrefs.Save();
        }
    }

    public int GetScore()
    {
        return score;
    }
}

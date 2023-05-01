using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager
{
    public int score;

    public TMP_Text scoreText;

    public ScoreManager(TMP_Text scoreText)
    {
        score = 0;
        this.scoreText = scoreText;
    }

    public void IncreaseScore(int value)
    {
        score += value;

        RenderScore();
    }

    public void RenderScore()
    {
        scoreText.text = string.Format("Score: {0:000 000 000}", score);
    }
}
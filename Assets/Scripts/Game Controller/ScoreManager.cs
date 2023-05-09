using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager
{
    private int mScore;
    private TMP_Text mScoreText;

    public ScoreManager(TMP_Text scoreText)
    {
        mScore = 0;
        mScoreText = scoreText;
    }

    public void IncreaseScore(int value)
    {
        mScore += value;

        RenderScore();
    }

    public void RenderScore()
    {
        mScoreText.text = string.Format("Score: {0:000 000 000}", mScore);
    }
}
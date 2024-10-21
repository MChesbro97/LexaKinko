using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableLetter
{
    public char Letter { get; set; }
    public int Score { get; set; }

    public UsableLetter(char letter, int baseScore)
    {
        Letter = letter;
        Score = baseScore;
    }

    public void AddPoints(int points)
    {
        Score += points;
    }

    public void ResetScore(int baseScore)
    {
        Score = baseScore;
    }

    public override string ToString()
    {
        return $"{Letter} (Score: {Score})";
    }
}

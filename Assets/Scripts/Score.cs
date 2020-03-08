using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static void Start()
    {
        //ResetHighscore();
        BirdControl.GetInstance().OnDied += Bird_OnDied;
    }

    private static void Bird_OnDied(object sender, System.EventArgs e)
    {
        SetHighscore(Level.GetInstance().GetPipesPassedCount());
    }

    public static int GetHighscore()
    {
        return PlayerPrefs.GetInt("Highscore");
    }

    public static bool SetHighscore(int Score)
    {
        int CurrentHighscore = GetHighscore();
        if (Score > CurrentHighscore)
        {
            PlayerPrefs.SetInt("Highscore", Score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResetHighscore()
    {
        PlayerPrefs.SetInt("Highscore", 0);
        PlayerPrefs.Save();
    }
}

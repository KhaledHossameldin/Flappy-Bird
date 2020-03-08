using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text ScoreText;
    private Text HighscoreText;

    public void RestartGame()
    {
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
        Invoke("RestartWithDelay", 0.235f);
    }

    private void RestartWithDelay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void Start()
    {
        ScoreText = transform.Find("ScoreText").GetComponent<Text>();
        HighscoreText = transform.Find("HighscoreText").GetComponent<Text>();
        Hide();
        BirdControl.GetInstance().OnDied += Bird_OnDied;
        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        ScoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
        if (Level.GetInstance().GetPipesPassedCount() >= Score.GetHighscore())
        {
            HighscoreText.text = "NEW HIGHSCORE";
        }
        else
        {
            HighscoreText.text = "HIGHSCORE: " + Score.GetHighscore().ToString();
        }
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}

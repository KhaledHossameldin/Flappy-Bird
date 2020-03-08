using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text ScoreText;
    private Text HighscoreText;

    private void Awake()
    {
        ScoreText = transform.Find("ScoreText").GetComponent<Text>();
        HighscoreText = transform.Find("HighscoreText").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        HighscoreText.text = "HIGHSCORE: " + Score.GetHighscore().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
    }
}

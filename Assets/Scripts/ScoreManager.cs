using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
  GameData gameData;
  public TextMeshProUGUI scoreText;

  private int totalScore = 0;
  private int multiplier = 1;

  // Start is called before the first frame update
  void Start()
  {
    gameData = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
  }

  // Update is called once per frame
  void Update()
  {
    if(scoreText != null)
    {
      scoreText.text = "Score: " + totalScore.ToString();
    }
    gameData.SetTotalScore(totalScore);
  }

  public void AddScore(int score)
  {
    totalScore += score * multiplier;
    multiplier += 1;
  }

  public void ResetMultiplier()
  {
    multiplier = 1;
  }

  public int GetScore()
  {
    return totalScore;
  }
}

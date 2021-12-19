using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
  public int totalScore = 0;
  // Start is called before the first frame update
  void Start()
  {
    DontDestroyOnLoad(gameObject);
  }

  // Update is called once per frame
  void Update()
  {
    
  }

  public void SetTotalScore(int score)
  {
    totalScore = score;
  }

  public int GetScore()
  {
    return totalScore;
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowScores : MonoBehaviour
{
  public GameObject enterScorePanel;
  public GameObject scorePanel;

  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
        
  }

  public void ShowScoresPanel()
  {
    enterScorePanel.SetActive(false);
    scorePanel.SetActive(true);
  }
}

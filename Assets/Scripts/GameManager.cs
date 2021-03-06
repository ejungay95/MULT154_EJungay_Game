using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
  public PlayerController player;
  public GameObject gameOverPanel;
  //public GameObject pausePanel;

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    if(gameOverPanel != null && player != null)
    {
      if(!player.isAlive)
      {
        gameOverPanel.SetActive(true);
        player.enabled = false;
      }
    }
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
  }
}
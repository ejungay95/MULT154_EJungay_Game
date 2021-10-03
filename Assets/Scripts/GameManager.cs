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
    if(!player.isAlive)
    {
      gameOverPanel.SetActive(true);
    }

    if(Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
  }
}
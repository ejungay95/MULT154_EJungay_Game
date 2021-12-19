using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{

  GameData gameData;
  // Start is called before the first frame update
  void Start()
  {
    gameData = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
  }

  // Update is called once per frame
  void Update()
  {
        
  }

  public void LoadLevel(int level)
  {
    gameData.SetTotalScore(0);
    SceneManager.LoadScene(level);
  }
}

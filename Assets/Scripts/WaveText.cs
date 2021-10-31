using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveText : MonoBehaviour
{
  private SpawnManager spawnManager;

  public TextMeshProUGUI waveNumberText;
  public TextMeshProUGUI countDownText;

  // Start is called before the first frame update
  void Start()
  {
    spawnManager = GetComponent<SpawnManager>();
  }

  // Update is called once per frame
  void Update()
  {
    if(spawnManager.currentState == SpawnManager.SpawnState.COUNTING)
    {
      waveNumberText.enabled = true;
      countDownText.enabled = true;
      waveNumberText.text = "Wave " + (spawnManager.waveIndex + 1);
      countDownText.text = ((int)spawnManager.waveCountdown + 1).ToString();

    }
    if(spawnManager.currentState == SpawnManager.SpawnState.SPAWNING)
    {
      waveNumberText.enabled = false;
      countDownText.enabled = false;
    }
  }
}

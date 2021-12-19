using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseOptionManager : MonoBehaviour
{
  public GameObject pausePanel;
  public Slider volumeSlider;

  // Start is called before the first frame update
  void Start()
  {
    DontDestroyOnLoad(gameObject);

    float volume = PlayerPrefs.GetFloat(AudioManager.VOLUME_LEVEL_KEY, AudioManager.DEFAULT_VOLUME);
    //pausePanel.GetComponentInChildren<Slider>().value = volume;
    volumeSlider.value = volume;

    pausePanel.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      if(!pausePanel.activeInHierarchy)
      {
        pausePanel.SetActive(true);
        Time.timeScale = 0.0f;
      } else
      {
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
      }  
    }
  }

  // Used for options button in main menu
  public void OpenMenuOnButtonPress()
  {
    pausePanel.SetActive(true);
    Time.timeScale = 0.0f;
  }

  public void CloseMenu()
  {
    pausePanel.SetActive(false);
    Time.timeScale = 1.0f;
  }
}

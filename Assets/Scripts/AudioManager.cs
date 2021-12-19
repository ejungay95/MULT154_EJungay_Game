using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public AudioSource audioSource;

  public AudioClip mainMenuClip;
  public AudioClip gameAudioClip;


  public const float DEFAULT_VOLUME = .5f;
  public const string VOLUME_LEVEL_KEY = "VolumeLevel";

  // Start is called before the first frame update
  void Start()
  {
    //audioSource = GetComponent<AudioSource>();

    float volume = PlayerPrefs.GetFloat(VOLUME_LEVEL_KEY, DEFAULT_VOLUME);
    audioSource.volume = volume;

    DontDestroyOnLoad(gameObject);
  }

  private void Update()
  {
    int currentScene = SceneManager.GetActiveScene().buildIndex;

    if(currentScene == 0)
    {
      audioSource.clip = mainMenuClip;
      if (!audioSource.isPlaying)
      {
        audioSource.Play();
      }
    } else if(currentScene == 1)
    {
      audioSource.clip = gameAudioClip;
      if(!audioSource.isPlaying)
      {
        audioSource.Play();
      }
    }
  }

  public void AdjustVolume(float volumeLevel)
  {
    audioSource.volume = volumeLevel;
    PlayerPrefs.SetFloat(VOLUME_LEVEL_KEY, volumeLevel);
  }
}

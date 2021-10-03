using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCredits : MonoBehaviour
{
  public GameObject creditsPanel;
  public GameObject MainMenuPanel;

  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    
  }

  public void ShowOrHidePanel()
  {
    if (creditsPanel.activeSelf == false)
    {
      MainMenuPanel.SetActive(false);
      creditsPanel.SetActive(true);
    } else
    {
      MainMenuPanel.SetActive(true);
      creditsPanel.SetActive(false);
    }
  }
}

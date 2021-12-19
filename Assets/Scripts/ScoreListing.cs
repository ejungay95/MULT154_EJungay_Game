using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class Score
{
  public Score(string _name, int _score)
  {
    name = _name;
    score = _score;
  }

  public string name;
  public int score;
}

public class ScoreListing : MonoBehaviour
{
  public List<Score> scores = new List<Score>();
  public string fileName;
  public GameObject input;

  public GameObject finalPanel;
  public GameObject scorePanel;

  public GameObject entryPrefab;
  GameData gameData;

  // Start is called before the first frame update
  void Start()
  {
    gameData = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();

    if (File.Exists(fileName))
    {
      using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
      {
        while (true)
        {
          try
          {
            string name = reader.ReadString();
            int score = reader.ReadInt32();

            scores.Add(new Score(name, score));
          } catch (EndOfStreamException)
          {
            break;
          }
        }
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void NewEntry()
  {
    string name = input.GetComponent<TMP_InputField>().text;
    int score = gameData.GetScore();
    scores.Insert(0, new Score(name, score));

    int offset = 0;

    foreach (Score scoreEntry in scores)
    {
      GameObject temp = Instantiate(entryPrefab);
      Transform[] children = temp.GetComponentsInChildren<Transform>();
      children[1].GetComponent<TextMeshProUGUI>().text = scoreEntry.name;
      children[2].GetComponent<TextMeshProUGUI>().text = scoreEntry.score.ToString();

      temp.transform.SetParent(scorePanel.transform);
      RectTransform rtrans = temp.GetComponent<RectTransform>();
      rtrans.anchorMin = new Vector2(.5f, .5f);
      rtrans.anchorMax = new Vector2(.5f, .5f);
      rtrans.pivot = new Vector2(.5f, .5f);
      rtrans.localPosition = new Vector3(0, offset, 0);

      offset -= 50;
    }

    finalPanel.SetActive(false);
    scorePanel.SetActive(true);
  }

  private void OnDestroy()
  {
    using (BinaryWriter write = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
    {
      foreach (Score scoreEntry in scores)
      {
        write.Write(scoreEntry.name);
        write.Write(scoreEntry.score);
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEditor.SearchService;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    public string BestPlayerName;
    public int BestScore;
    public TextMeshProUGUI BestText;


    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    public TextMeshProUGUI playerName;

    // Start is called before the first frame update

  
    void Start()
    {
        playerName.text = MenuManager.Instance.PlayerName;
        Loading();
        BestText.text = "Best score: " + BestPlayerName + BestScore;
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        BestText.text = "Best score: " + BestPlayerName + " " + BestScore;
        if (m_Points > BestScore)
        {
            BestScore = m_Points;
            BestPlayerName = MenuManager.Instance.PlayerName;
            Saving();
            Loading();
        }
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Loading();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > BestScore)
        {
            BestScore = m_Points;
            BestPlayerName = MenuManager.Instance.PlayerName;
            Saving();
        }    
    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    [System.Serializable]
    class SaveData
    {
        public string PlayerName;
        public int BestScore;
    }

    public void Saving()
    {
        SaveData data = new SaveData();
        data.PlayerName = BestPlayerName;
        data.BestScore = BestScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void Loading()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            BestPlayerName = data.PlayerName;
            BestScore = data.BestScore;
        }
    }
}

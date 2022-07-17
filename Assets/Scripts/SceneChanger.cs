using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    //config parameters
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI scoreText;


    //cached references
    AudioManager audioManager;
    ThreatMeter threatMeter;
    public bool gameOver = false;
    
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.ResetSliders();
        threatMeter = FindObjectOfType<ThreatMeter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOverPanel == null) { return; }
        if(gameOver)
        {
            gameOverPanel.SetActive(true);
            scoreText.text = "on round " + threatMeter.currentThreatValue.ToString();
        }
        else
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void GoToGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GoToCredits()
    {
        SceneManager.LoadScene(2);
    }
   
}

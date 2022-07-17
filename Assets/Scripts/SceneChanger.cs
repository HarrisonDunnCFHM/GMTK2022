using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    //cached references
    AudioManager audioManager;
    
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.ResetSliders();
    }

    // Update is called once per frame
    void Update()
    {
        
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

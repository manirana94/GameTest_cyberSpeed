using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }//Singleton instance
    public AudioClip buttonClickSound;
    public AudioClip MatchSound;
    public AudioClip NotMatchSound;
    public AudioClip CoinsSound;
    public AudioSource audioSource;


    public GameObject WinTextPanel;
    public GameObject TitleText;
    public TextMeshProUGUI ScoreText;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        TitleText.gameObject.SetActive(true);
        ScoreText.text ="Score: "+ PlayerPrefs.GetInt("Score",0).ToString();

    }
    
    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    public void PlayMatchedSound()
    {
        if (MatchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(MatchSound);
        }
    }
    public void PlayNotMatchedSound()
    {
        if (NotMatchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(NotMatchSound);
        }
    }
    
    public void PlayCoinsSound()
    {
        if (CoinsSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(CoinsSound);
        }
    }

    public void PauseGame() // Pause the game
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()// Resume the game
    {
        Time.timeScale = 1; 
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int value)
    {
       int score = PlayerPrefs.GetInt("Score",0);
       score += value;
       PlayerPrefs.SetInt("Score" ,score);
       ScoreText.text ="Score: "+ score.ToString();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject quitButton;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TMP_Text gameOverText;

    #region  SingletonImplementation

    private static GameManager _instance;
    
    public static GameManager Instance {
        get { return _instance; }
    }

    #endregion

    #region MonoBehavior Callbacks

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(_instance);
        } else {
            _instance = this;
        }
    }

    #endregion
    
    #region Custom Methods

    public void GameOver(bool playerWin) {
        if (gameOverPanel.activeInHierarchy)
            return;
        
        if (playerWin) {
            gameOverText.text = "YOU WON!";
            retryButton.SetActive(false);
            gameOverPanel.SetActive(true);
        }
        else {
            gameOverText.text = "TRY AGAIN :(";
            nextLevelButton.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    #endregion
    
    #region Button Methods

    public void OnNextLevelClick() {
        if (SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings) {
            //we are on last level
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnRetryClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitClick() {
        Application.Quit();
    }

    #endregion
    
    
}

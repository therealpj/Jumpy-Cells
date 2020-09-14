using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOver : MonoBehaviour
{
    private int score;
    TextMeshProUGUI tmp;
    void OnEnable() {
        PlayerPrefs.SetInt("level", 0);
        score = PlayerPrefs.GetInt("score");
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = "You Scored:    " + score;
    }

    public void PlayAgain() {
        SceneManager.LoadSceneAsync("Game");
    }

    public void MainMenu() {
        SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex + 1)%SceneManager.sceneCountInBuildSettings);
    }
}

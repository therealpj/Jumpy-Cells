using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour  {
    int score = 0;
    Text scoreText;
    void Awake() {
        scoreText = GetComponent<Text>();
        scoreText.text = "Score: " + score;
    }

    public void UpdateScore(int amount) {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public int GetScore() {
        return score;
    }

    // void OnDisable() {
        // PlayerPrefs.SetInt("score", score);
    // }

}

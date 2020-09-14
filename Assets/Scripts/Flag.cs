using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other){
        if (other.gameObject.tag == "Player") {
            AudioSource source = GetComponent<AudioSource>();
            source.Play();
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.Out,"Game"));
            int score = PlayerPrefs.GetInt("score");
            PlayerPrefs.SetInt("score", score + 500);
        }
    }
}

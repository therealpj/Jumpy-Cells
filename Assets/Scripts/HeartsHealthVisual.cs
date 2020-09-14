using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartsHealthVisual : MonoBehaviour
{

    public Sprite heartFull, heartEmpty, heartHalf;
    private List<HeartImage> heartImageList;
    private HeartHealthSystem heartHealthSystem;

    public static HeartHealthSystem heartHealthSystemStatic;
    private void Awake() {
        heartImageList = new List<HeartImage>();
    }

    private HeartImage CreateHeartImage(Vector2 anchoredPosition) {
        GameObject heartGameObject = new GameObject("Heart", typeof(Image));
        heartGameObject.transform.parent = transform;
        heartGameObject.GetComponent<Image>().sprite = heartFull;
        heartGameObject.transform.localPosition = Vector3.zero;

        heartGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        heartGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

        Image heartImageUI = heartGameObject.GetComponent<Image>();
        heartImageUI.sprite = heartFull;

        HeartImage heartImage = new HeartImage(this, heartImageUI);
        heartImageList.Add(heartImage);
        return heartImage;
    }

    void Start() {
        HeartHealthSystem hhs = new HeartHealthSystem(4);
        SetHeartHealthSystem(hhs);
    }

    public void SetHeartHealthSystem(HeartHealthSystem heartsHealthSystem) {
        this.heartHealthSystem = heartsHealthSystem;
        heartHealthSystemStatic = heartsHealthSystem;

        Vector2 heartAnchoredPosition = new Vector2(-350, 250);
        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        for(int i = 0; i < heartList.Count; i++) {
            HeartHealthSystem.Heart heart = heartList[i];
            CreateHeartImage(heartAnchoredPosition).SetHeartFragment(heart.GetFragmentAmount());
            heartAnchoredPosition += new Vector2(40, 0);
        }

        heartsHealthSystem.OnDamaged += HeartsHealthSystem_OnDamaged;
        heartsHealthSystem.OnHealed += HeartsHealthSystem_OnHealed;
        heartsHealthSystem.OnDead += HeartsHealthSystem_OnDead;
    }

    private void HeartsHealthSystem_OnDead(object sender, System.EventArgs e) {
        StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.Out,"Game Over"));
    }

    private void HeartsHealthSystem_OnHealed(object sender, System.EventArgs e) {
        RefreshAllHearts();
    }

    private void HeartsHealthSystem_OnDamaged(object sender, System.EventArgs e) {
        RefreshAllHearts();
    }
    public void RefreshAllHearts() {
        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        for(int i = 0; i < heartImageList.Count; i++) {
            HeartImage heartImage = heartImageList[i];
            HeartHealthSystem.Heart heart = heartList[i];
            heartImage.SetHeartFragment(heart.GetFragmentAmount());
        }
    }

    public class HeartImage {
        private Image heartImage;
        private HeartsHealthVisual heartsHealthVisual;

        public HeartImage(HeartsHealthVisual heartsHealthVisual, Image heartImage) {
            this.heartsHealthVisual = heartsHealthVisual;
            this.heartImage = heartImage;
        }


        public void SetHeartFragment(int fragments) {
            switch(fragments) {
                case 2: heartImage.sprite = heartsHealthVisual.heartFull; break;
                case 1: heartImage.sprite = heartsHealthVisual.heartHalf; break;
                case 0: heartImage.sprite = heartsHealthVisual.heartEmpty; break;
            }
        }
    }
}

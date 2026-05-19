using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuBackground : MonoBehaviour {
    [Serializable] public struct MainMenuBackgroundData {
        public Sprite image;
        public int levelIndexRequirement;
    }
    
    [SerializeField] private List<MainMenuBackgroundData> images;
    [SerializeField] private Image backgroundImage;

    private int currentImage;

    private void Awake() {
        int maxLevelCompleted = LevelLoading.GetHighestLevelCompleted();
        for (int i = images.Count - 1; i >= 0; i--) {
            if (images[i].levelIndexRequirement > maxLevelCompleted) {
                images.RemoveAt(i);
            }
        }

        PickRandomImage();
    }

    public void PickRandomImage() {
        int i = Random.Range(0, images.Count);

        if (i == currentImage) {
            if (i == images.Count - 1) {
                i = 0;
            }
            else {
                i++;
            }
        }

        currentImage = i;
        backgroundImage.sprite = images[i].image;
    }
}
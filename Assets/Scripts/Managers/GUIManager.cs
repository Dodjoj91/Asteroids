using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Manager
{
    [SerializeField] TMP_Text scoreAmount;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] List<Image> imageLives;

    private void Start()
    {
        ManagerSystem.Instance.GameManager.ScoreDelegate += SetScore;
        ManagerSystem.Instance.GameManager.LivesDelegate += SetLives;
    }

    private void SetScore(int score)
    {
        scoreAmount.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        foreach (var lifeImage in imageLives)
        {
            lifeImage.gameObject.SetActive(false);
        }


        for (int i = 0; i  < lives; i++)
        {
            imageLives[i].gameObject.SetActive(true);
        }

    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Manager
{
    [SerializeField] TMP_Text endingPrompt;
    [SerializeField] TMP_Text scoreAmount;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] List<Image> imageLives;

    const string WinningString = "You won, next round starting soon!";
    const string LoseString = "You Lost!";

    private void Start()
    {
        ManagerSystem.Instance.GameManager.ScoreDelegate += SetScore;
        ManagerSystem.Instance.GameManager.LivesDelegate += SetLives;
        ManagerSystem.Instance.GameManager.EndingDelegate += ShowEndingPrompt;
    }

    private void SetScore(int score)
    {
        scoreAmount.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        imageLives.ForEach(live => live.gameObject.SetActive(false));

        for (int i = 0; i  < lives; i++)
        {
            imageLives[i].gameObject.SetActive(true);
        }
    }

    private void ShowEndingPrompt(bool winningState, bool shouldShow)
    {
        endingPrompt.gameObject.SetActive(shouldShow);
        endingPrompt.text = winningState ? WinningString : LoseString;
    }
}

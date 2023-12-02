using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Manager
{
    #region Variables

    [SerializeField] private TMP_Text endingPrompt;
    [SerializeField] private TMP_Text scoreAmount;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private List<Image> imageLives;


    private const string WinningString = "You won, next round starting soon!";
    private const string LoseString = "You Lost!";

    #endregion

    #region Unity Functions

    private void Start()
    {
        ManagerSystem.Instance.GameManager.ScoreDelegate += SetScore;
        ManagerSystem.Instance.GameManager.LivesDelegate += SetLives;
        ManagerSystem.Instance.GameManager.EndingDelegate += ShowEndingPrompt;
    }

    #endregion

    #region Delegate Functions

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

    #endregion
}
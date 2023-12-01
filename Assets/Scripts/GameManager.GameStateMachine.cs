

using Unity.VisualScripting;
using UnityEngine;

public partial class GameManager
{
    private EGameState currentState = EGameState.StartingGame;


    public void SetState(EGameState newState)
    {
        if (currentState != newState) { SetStartState(newState); }
        currentState = newState;
    }

    private EGameState GetState()
    {
        return currentState;
    }

    protected void UpdateStateMachine()
    {
        switch (currentState)
        {
            case EGameState.StartingGame:
                UpdateStartingGame();
                break;
            case EGameState.Playing:
                UpdatePlayingGame();
                break;
            case EGameState.EndingGame:
                UpdateEndingGame();
                break;
            case EGameState.Paused:
                UpdatePausedGame();
                break;
        }
    }

    private void UpdateStartingGame()
    {
        bool hasLoadedAllAssets = true;

        if (loadOperations.Count > 0)
        {
            loadOperations.ForEach(op =>
            {
                if (!op.IsDone) 
                {
                    hasLoadedAllAssets = false;
                    return;
                }
            });
        }

        if (!hasLoadedAllAssets) { return; }

        SetState(EGameState.Playing);
    }

    private void UpdatePlayingGame()
    {

    }

    private void UpdateEndingGame()
    {
        newStartGameTimer -= Time.deltaTime;

        if (newStartGameTimer < 0.0f)
        {
            ResetGame();
        }

    }

    private void UpdatePausedGame()
    {

    }

    private void SetStartState(EGameState startGameState)
    {
        currentState = startGameState;

        switch (startGameState)
        {
            case EGameState.StartingGame:
                Time.timeScale = 1.0f;
                break;
            case EGameState.Playing:
                Time.timeScale = 1.0f;
                //SpawnEnemies(EEnemyType.Asteroid, Random.Range(minAsteroidAmount, maxAsteroidAmount));
                SpawnEnemies(EEnemyType.FlyingSaucer, Random.Range(1, 1));
                break;
            case EGameState.EndingGame:
                newStartGameTimer = newStartGameTimerMax;
                Time.timeScale = 1.0f;
                break;
            case EGameState.Paused:
                Time.timeScale = 0.0f;
                break;
        }
    }

    private void SetEndingState(bool isWinning)
    {
        this.isWinning = isWinning;
        SetStartState(EGameState.EndingGame);
        EndingDelegate?.Invoke(isWinning, true);
    }
}
using UnityEngine;

public partial class GameManager
{
    #region Variables

    private EGameState currentState = EGameState.StartingGame;

    #endregion

    #region Update State Functions

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
        if (HasLoadedAllAssets()) { SetState(EGameState.Playing); }
    }

    private void UpdatePlayingGame()
    {

    }

    private void UpdateEndingGame()
    {
        newStartGameTimer -= Time.deltaTime;

        if (newStartGameTimer < 0.0f) { ResetGame(); }
    }

    private void UpdatePausedGame()
    {

    }

    #endregion

    #region State Functions

    public void SetState(EGameState newState)
    {
        if (currentState != newState) { SetStartState(newState); }
        currentState = newState;
    }

    private EGameState GetState()
    {
        return currentState;
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
                SpawnEnemies(EEnemyType.Asteroid, Random.Range(minAsteroidAmount, maxAsteroidAmount));
                SpawnEnemies(EEnemyType.FlyingSaucer, Random.Range(minFlyingSaucerAmount, maxFlyingSaucerAmount));
                break;
            case EGameState.EndingGame:
                SetEndingVariables();
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
        SetState(EGameState.EndingGame);
        EndingDelegate?.Invoke(isWinning, true);
    }

    private void SetEndingVariables()
    {
        if (isWinning) { ClearBullets(); }
        else { player.gameObject.SetActive(false); }

        newStartGameTimer = newStartGameTimerMax;
    }

    #endregion

    #region Setup Functions

    private bool HasLoadedAllAssets()
    {
        bool hasLoadedAllAssets = true;

        if (startingLoadOperations.Count > 0)
        {
            startingLoadOperations.ForEach(op =>
            {
                if (!op.IsDone)
                {
                    hasLoadedAllAssets = false;
                    return;
                }
            });

            if (hasLoadedAllAssets) { startingLoadOperations.Clear(); }
        }

        return hasLoadedAllAssets;
    }

    #endregion
}
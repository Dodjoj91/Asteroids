

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
                break;
            case EGameState.Paused:
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
                SpawnEnemies(EEnemyType.Asteroid, Random.Range(minAsteroidAmount, maxAsteroidAmount));
                SpawnEnemies(EEnemyType.FlyingSaucer, Random.Range(minFlyingSaucerAmount, maxFlyingSaucerAmount));
                break;
            case EGameState.Paused:
                Time.timeScale = 0.0f;
                break;
        }

    }

}
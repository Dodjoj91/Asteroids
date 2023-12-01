using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;


public partial class GameManager : Manager
{
    #region Variables

    [Header("General"), Space]
    PlayerShip player;
    [SerializeField] private Camera mainCamera;


    [Header("Game Logic"), Space]
    [SerializeField] private Vector2 playerSpawnPosition = new Vector2(0.5f, 0.5f);
    [SerializeField] float newStartGameTimerMax = 10.0f;

    [SerializeField] private int minAsteroidAmount = 10;
    [SerializeField] private int maxAsteroidAmount = 20;

    [SerializeField] private int minFlyingSaucerAmount = 3;
    [SerializeField] private int maxFlyingSaucerAmount = 7;

    [SerializeField] private Dictionary<EEnemyType, List<UnitDataEnemy>> enemyData;
    [SerializeField] private Dictionary<EPlayerType, List<UnitDataPlayer>> playerData;


    private AddressablesManager addressablesManager;
    private ObjectPoolManager objectPoolManager;

    UnityAction<int> scoreDelegate;
    UnityAction<int> livesDelegate;
    UnityAction<bool, bool> endingDelegate;

    bool isWinning = false;
    float newStartGameTimer = 0.0f;

    int totalScore = 0;
    int lives = 3;

    const int startLives = 3;
    const int maxLives = 5;
    const int maxScore = 999999;

    List<Bullet> spawnedBullets = new List<Bullet>();
    Dictionary<EEnemyType, List<GameObject>> enemies = null;

    List<AsyncOperationHandle<IList<Object>>> startingLoadOperations = new List<AsyncOperationHandle<IList<Object>>>();

    #endregion

    #region Properties

    public UnityAction<int> ScoreDelegate { get { return scoreDelegate; } set { scoreDelegate = value; } }
    public UnityAction<int> LivesDelegate { get { return livesDelegate; } set { livesDelegate = value; } }
    public UnityAction<bool, bool> EndingDelegate { get { return endingDelegate; } set { endingDelegate = value; } }
    public int Lives { get { return lives; } }

    #endregion

    #region Unity Functions

    private void Start()
    {
        SpawnEnemyPreset(ESpawnPreset.AssetPackOne);
    }

    void OnAddressableSpawnPresetComplete(AsyncOperationHandle<IList<Object>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var obj in handle.Result)
            {
                Object prefabInstance = obj;

                if (prefabInstance != null && prefabInstance is UnitData)
                {
                    LoadUnitData(prefabInstance as UnitData);
                }
            }

            InitStartGame();
        }
        else
        {
            Debug.LogError("Failed to load prefab: + " + handle.DebugName);
        }
    }

    private void Update()
    {
        UpdateStateMachine();
    }

    #endregion

    #region Setup Functions

    private void InitStartGame()
    {
        try
        {
            PlayerShip playerShip = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.PlayerShip).GetComponent<PlayerShip>();
            InputHandler inputHandler = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.InputHandler).GetComponent<InputHandler>();
            inputHandler.transform.SetParent(playerShip.transform, false);

            playerShip.AttachInputHandler(inputHandler);
            playerShip.UnitData = playerData[EPlayerType.PlayerShip][0];
            player = playerShip;

            ResetGame();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception occurred in GameManager - InitStartGame() " + ex.Message);
        }
    }

    private void LoadUnitData(UnitData data)
    {
        if (data is UnitDataEnemy)
        {
            enemyData ??= new Dictionary<EEnemyType, List<UnitDataEnemy>>();

            UnitDataEnemy assetEnemyData = data as UnitDataEnemy;
            if (!enemyData.ContainsKey(assetEnemyData.enemyType)) { enemyData.Add(assetEnemyData.enemyType, new List<UnitDataEnemy>()); }
            enemyData[assetEnemyData.enemyType].Add(assetEnemyData);
        }
        else if (data is UnitDataPlayer)
        {
            playerData ??= new Dictionary<EPlayerType, List<UnitDataPlayer>>();

            UnitDataPlayer assetPlayerData = data as UnitDataPlayer;
            if (!playerData.ContainsKey(assetPlayerData.playerType)) { playerData.Add(assetPlayerData.playerType, new List<UnitDataPlayer>()); }
            playerData[assetPlayerData.playerType].Add(assetPlayerData);
        }
    }

    public void AttachManagers(AddressablesManager addressablesManager, ObjectPoolManager objectPoolManager)
    {
        this.addressablesManager = addressablesManager;
        this.objectPoolManager = objectPoolManager;
    }

    #endregion

    #region Game Logic Functions

    private void ResetGame()
    {
        ReturnAllActiveObjects();
        SetState(EGameState.StartingGame);
        SetObjectPositionOnCameraView(player.gameObject, playerSpawnPosition);

        player.gameObject.SetActive(true);
        lives = startLives;
        totalScore = isWinning ? totalScore : 0;
        isWinning = false;
        scoreDelegate?.Invoke(totalScore);
        livesDelegate?.Invoke(lives);
        EndingDelegate?.Invoke(false, false);
    }

    private bool IsEnemyListEmpty()
    {
        bool isEmpty = currentState == EGameState.Playing ? true : false;

        if (enemies != null)
        {
            foreach (var enemyList in enemies)
            {
                if (enemyList.Value.Count > 0)
                {
                    isEmpty = false;
                    break;
                }
            }
        }

        return isEmpty;
    }

    private void SpawnEnemyPreset(ESpawnPreset spawnPreset)
    {
        var op = addressablesManager.LoadAsyncGroup(spawnPreset);
        if (op.IsValid()) 
        { 
            op.Completed += OnAddressableSpawnPresetComplete;
            startingLoadOperations.Add(op);
        }
    }

    private void SpawnEnemies(EEnemyType enemyType, int enemyAmount)
    {
        try
        {
            enemies ??= new Dictionary<EEnemyType, List<GameObject>>();
            List<GameObject> listOfEnemies = new List<GameObject>();

            for (int i = 0; i < enemyAmount; i++)
            {
                switch (enemyType)
                {
                    case EEnemyType.Asteroid:
                        GameObject asteroidObj = objectPoolManager.GetPooledObject(EObjectPooling.Asteroid);
                        Asteroid asteroidComp = asteroidObj.GetComponent<Asteroid>();
                        asteroidComp.UnitData = enemyData[EEnemyType.Asteroid][0];
                        listOfEnemies.Add(asteroidObj);
                        break;
                    case EEnemyType.FlyingSaucer:
                        GameObject flyingSaucerObj = objectPoolManager.GetPooledObject(EObjectPooling.FlyingSaucer);
                        FlyingSaucer flyingSaucerComp = flyingSaucerObj.GetComponent<FlyingSaucer>();
                        flyingSaucerComp.UnitData = enemyData[EEnemyType.FlyingSaucer][0];
                        listOfEnemies.Add(flyingSaucerObj);
                        break;
                }
            }

            SetListOfObjectOnCameraViewRandom(listOfEnemies);

            enemies[enemyType] = listOfEnemies;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception occurred in GameManager - SpawnEnemies() " + ex.Message);
        }
    }

    public void AddScore(int score)
    {
        totalScore += score;
        totalScore = Mathf.Clamp(totalScore, 0, maxScore);
        scoreDelegate?.Invoke(totalScore);
    }

    public void AddLife(int life)
    {
        lives += life;
        lives = Mathf.Clamp(lives, 0, maxLives);
        livesDelegate?.Invoke(lives);

        if (lives == 0)
        {
            SetEndingState(false);
        }
    }

    #region Remove/Return Functions

    public void ShouldRemoveBullet(Bullet asteroidObject, bool shouldRemove)
    {
        if (shouldRemove && spawnedBullets.Contains(asteroidObject)) { spawnedBullets.Remove(asteroidObject); }
        else if (!shouldRemove && !spawnedBullets.Contains(asteroidObject)) { spawnedBullets.Add(asteroidObject); }
    }

    public void RemoveEnemy(EEnemyType enemyType, GameObject asteroidObject)
    {
        if (!enemies.ContainsKey(enemyType)) { return; }

        var enemyList = enemies[enemyType];
        if (enemyList.Contains(asteroidObject)) { enemyList.Remove(asteroidObject); }

        if (IsEnemyListEmpty())
        {
            SetEndingState(true);
        }
    }

    private void ReturnAllActiveObjects()
    {
        ClearEnemies();
        ClearBullets();
    }

    private void ClearEnemies()
    {
        if (enemies != null)
        {
            foreach (var enemyList in enemies)
            {
                if (enemyList.Value.Count > 0)
                {
                    foreach (var enemyObj in enemyList.Value)
                    {
                        EObjectPooling poolingType = ExtensionUtility.ConvertEnemyTypeToObjectType(enemyList.Key);
                        if (poolingType != EObjectPooling.None) { objectPoolManager.ReturnObject(poolingType, enemyObj); }
                    }
                }
            }

            enemies.Clear();
        }
    }

    private void ClearBullets()
    {
        if (spawnedBullets != null)
        {
            foreach (var bullet in spawnedBullets)
            {
                objectPoolManager.ReturnObject(EObjectPooling.Bullet, bullet.gameObject);
            }

            spawnedBullets.Clear();
        }
    }

    #endregion
    #endregion

    #region Utility Functions

    private void SetObjectPositionOnCameraView(GameObject obj, Vector2 position)
    {
        Vector3 worldPoint = mainCamera.ViewportToWorldPoint(position);
        worldPoint.z = mainCamera.transform.position.z + 10.0f;
        obj.transform.SetPositionAndRotation(worldPoint, Quaternion.identity);
    }

    private void SetListOfObjectOnCameraViewRandom(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            SetObjectPositionOnAvailableSpot(obj.GetComponent<BoxCollider2D>());
        }
    }

    public void SetObjectPositionOnAvailableSpot(BoxCollider2D boxCollider)
    {
        Vector3 worldPoint = Vector3.zero;
        bool spawnedAtRandomLocation = false;

        for (int i = 0; i < 10; i++)
        {
            float randomXPos = Random.Range(0.0f, 1.0f);
            float randomYPos = Random.Range(0.0f, 1.0f);

            Vector2 randomPosition = new Vector2(randomXPos, randomYPos);
            worldPoint = GetViewportToWorldPoint(randomPosition);

            if (!Physics2D.OverlapBox(worldPoint, boxCollider.size, 0.0f))
            {
                boxCollider.gameObject.transform.position = worldPoint;
                spawnedAtRandomLocation = true;
            }
        }

        //Just forcefully set the object on top of another gameObject
        if (!spawnedAtRandomLocation) { boxCollider.gameObject.transform.position = worldPoint; }
    }

    private Vector3 GetViewportToWorldPoint(Vector2 position)
    {
        Vector3 worldPoint = mainCamera.ViewportToWorldPoint(position);
        worldPoint.z = mainCamera.transform.position.z + 10.0f;
        return worldPoint;
    }

    #endregion
}
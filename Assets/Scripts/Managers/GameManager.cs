using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;


public partial class GameManager : Manager
{
    [SerializeField] private Camera mainCamera;
    
    private AddressablesManager addressablesManager;
    private ObjectPoolManager objectPoolManager;

    [Header("General"), Space]
    public string temporary = "";
    PlayerShip player;


    [Header("Game Logic"), Space]
    [SerializeField] private Vector2 playerSpawnPosition = new Vector2(0.5f, 0.5f);
    [SerializeField] float newStartGameTimerMax = 10.0f;

    [SerializeField] private int minAsteroidAmount = 10;
    [SerializeField] private int maxAsteroidAmount = 20;

    [SerializeField] private int minFlyingSaucerAmount = 3;
    [SerializeField] private int maxFlyingSaucerAmount = 7;

    [SerializeField] private Dictionary<EEnemyType, List<UnitDataEnemy>> enemyData;
    [SerializeField] private Dictionary<EPlayerType, List<UnitDataPlayer>> playerData;


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

    List<AsyncOperationHandle> loadOperations = new List<AsyncOperationHandle>();


    public UnityAction<int> ScoreDelegate { get { return scoreDelegate; } set { scoreDelegate = value; } }
    public UnityAction<int> LivesDelegate { get { return livesDelegate; } set { livesDelegate = value; } }
    public UnityAction<bool, bool> EndingDelegate { get { return endingDelegate; } set { endingDelegate = value; } }

    private void Awake()
    {
        LoadUnitData();
    }

    private void Start()
    {
        InitStartGame();
    }

    private void Update()
    {
        UpdateStateMachine();
    }

    private void LoadUnitData()
    {
        try
        {
            if (!AssetDatabase.IsValidFolder(StaticDefines.UNIT_ASSET_PATH))
            {
                Debug.LogError("Invalid folder path: " + StaticDefines.UNIT_ASSET_PATH);
                return;
            }

            string[] assetPaths = AssetDatabase.FindAssets("Data", new[] { StaticDefines.UNIT_ASSET_PATH });

            UnitData[] assets = new UnitData[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<UnitData>(AssetDatabase.GUIDToAssetPath(assetPaths[i]));
            }

            foreach (var asset in assets)
            {
                if (asset is UnitDataEnemy)
                {
                    enemyData ??= new Dictionary<EEnemyType, List<UnitDataEnemy>>();

                    UnitDataEnemy assetEnemyData = asset as UnitDataEnemy;
                    if (!enemyData.ContainsKey(assetEnemyData.enemyType)) { enemyData.Add(assetEnemyData.enemyType, new List<UnitDataEnemy>()); }
                    enemyData[assetEnemyData.enemyType].Add(assetEnemyData);
                }
                else if (asset is UnitDataPlayer)
                {
                    playerData ??= new Dictionary<EPlayerType, List<UnitDataPlayer>>();

                    UnitDataPlayer assetPlayerData = asset as UnitDataPlayer;
                    if (!playerData.ContainsKey(assetPlayerData.playerType)) { playerData.Add(assetPlayerData.playerType, new List<UnitDataPlayer>()); }
                    playerData[assetPlayerData.playerType].Add(assetPlayerData);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception occurred in GameManager - LoadUnitData() " + ex.Message);
        }
    }

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

    public void AttachManagers(AddressablesManager addressablesManager, ObjectPoolManager objectPoolManager)
    {
        this.addressablesManager = addressablesManager;
        this.objectPoolManager = objectPoolManager;
    }

    private void SpawnEnemyPreset(ESpawnPreset spawnPreset)
    {
        var op = addressablesManager.LoadAsyncGroup(spawnPreset);
        if (op.IsValid()) { loadOperations.Add(op); }
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

    private void ResetGame()
    {
        ReturnAllActiveObjects();
        SetState(EGameState.StartingGame);
        SetObjectPositionOnCameraView(player.gameObject, playerSpawnPosition);
        SpawnEnemyPreset(ESpawnPreset.AssetPackOne);

        player.gameObject.SetActive(true);
        lives = startLives;
        totalScore = isWinning ? totalScore : 0;
        isWinning = false;
        scoreDelegate?.Invoke(totalScore);
        livesDelegate?.Invoke(lives);
        EndingDelegate?.Invoke(false, false);
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

    public void SetBullet(Bullet asteroidObject, bool shouldRemove)
    {
        if (shouldRemove && spawnedBullets.Contains(asteroidObject)) { spawnedBullets.Remove(asteroidObject); }
        else if (!shouldRemove && !spawnedBullets.Contains(asteroidObject)) { spawnedBullets.Add(asteroidObject); }   
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

    public int GetLives() => lives;
}

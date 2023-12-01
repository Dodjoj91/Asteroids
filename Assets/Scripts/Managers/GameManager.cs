using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

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

    [SerializeField] private int minAsteroidAmount = 10;
    [SerializeField] private int maxAsteroidAmount = 20;

    [SerializeField] private int minFlyingSaucerAmount = 3;
    [SerializeField] private int maxFlyingSaucerAmount = 7;

    [SerializeField] private UnitData playerData;
    [SerializeField] private List<UnitData> asteroidData;
    [SerializeField] private List<UnitData> flyingSaucerData;

    UnityAction<int> scoreDelegate;
    UnityAction<int> livesDelegate;

    int totalScore = 0;
    int lives = 3;

    const int startLives = 3;
    const int maxLives = 5;
    const int maxScore = 999999;

    Dictionary<EEnemyType, List<GameObject>> enemies = null;

    List<AsyncOperationHandle> loadOperations = new List<AsyncOperationHandle>();


    public UnityAction<int> ScoreDelegate { get { return scoreDelegate; } set { scoreDelegate = value; } }
    public UnityAction<int> LivesDelegate { get { return livesDelegate; } set { livesDelegate = value; } }


    private void Start()
    {
        InitStartGame();
    }

    private void Update()
    {
        UpdateStateMachine();
    }

    private void InitStartGame()
    {
        PlayerShip playerShip = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.PlayerShip).GetComponent<PlayerShip>();
        InputHandler inputHandler = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.InputHandler).GetComponent<InputHandler>();
        inputHandler.transform.SetParent(playerShip.transform, false);

        playerShip.AttachInputHandler(inputHandler);
        playerShip.UnitData = playerData;
        player = playerShip;

        ResetGame();
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
        enemies ??= new Dictionary<EEnemyType, List<GameObject>>();
        List<GameObject> listOfEnemies = new List<GameObject>();

        for (int i = 0; i < enemyAmount; i++)
        {
            switch (enemyType)
            {
                case EEnemyType.Asteroid:
                    GameObject asteroidObj = objectPoolManager.GetPooledObject(EObjectPooling.Asteroid);
                    Asteroid asteroidComp = asteroidObj.GetComponent<Asteroid>();
                    asteroidComp.UnitData = asteroidData[0];
                    listOfEnemies.Add(asteroidObj);
                    break;
                case EEnemyType.FlyingSaucer:
                    GameObject flyingSaucerObj = objectPoolManager.GetPooledObject(EObjectPooling.FlyingSaucer);
                    FlyingSaucer flyingSaucerComp = flyingSaucerObj.GetComponent<FlyingSaucer>();
                    flyingSaucerComp.UnitData = flyingSaucerData[0];
                    listOfEnemies.Add(flyingSaucerObj);
                    break;
            }
        }

        SetListOfObjectOnCameraViewRandom(listOfEnemies);
        
        enemies[enemyType] = listOfEnemies;
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
           //float xPos = Random.Range(0.0f, 1.0f);
           //float yPos = Random.Range(0.0f, 1.0f);
           //Vector2 randomPosition = new Vector2(xPos, yPos);
           //SetObjectPositionOnCameraView(obj, randomPosition);
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
            ResetGame();
        }
    }

    private void ResetGame()
    {
        if (enemies != null)
        {
            foreach (var enemyList in enemies)
            {
                if (enemyList.Value.Count > 0)
                {
                    foreach (var enemyObj in enemyList.Value)
                    {
                        EObjectPooling poolingType = ConvertEnemyTypeToObjectType(enemyList.Key);
                        if (poolingType != EObjectPooling.None) { objectPoolManager.ReturnObject(poolingType, enemyObj); }
                    }
                }
            }

            enemies.Clear();
        }

        SetState(EGameState.StartingGame);
        SetObjectPositionOnCameraView(player.gameObject, playerSpawnPosition);
        SpawnEnemyPreset(ESpawnPreset.AssetPackOne);

        lives = startLives;
        totalScore = 0;
        scoreDelegate?.Invoke(totalScore);
        livesDelegate?.Invoke(lives);
    }

    private EObjectPooling ConvertEnemyTypeToObjectType(EEnemyType enemyType)
    {
        switch (enemyType)
        {
            case EEnemyType.Asteroid:
                return EObjectPooling.Asteroid;
            case EEnemyType.FlyingSaucer:
                return EObjectPooling.PlayerShip;
            default:
                return EObjectPooling.None;
        }
    }

    public void RemoveEnemy(EEnemyType enemyType, GameObject asteroidObject)
    {
        if (!enemies.ContainsKey(enemyType)) { return; }

        var enemyList = enemies[enemyType];
        if (enemyList.Contains(asteroidObject)) { enemyList.Remove(asteroidObject); }
    }

    public int GetLives() => lives;
}

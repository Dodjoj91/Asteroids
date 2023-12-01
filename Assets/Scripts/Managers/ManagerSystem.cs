using System.Collections.Generic;
using UnityEngine;

public class ManagerSystem : MonoBehaviour
{
    #region Variables

    [Header("Managers"), Space]
    [SerializeField] private GameObject managerRoot;
    [SerializeField] private List<Manager> managers;

    private GameManager gameManager;
    private AddressablesManager addressablesManager;
    private GUIManager guiManager;
    private ObjectPoolManager objectPoolManager;

    #endregion

    #region Properties

    public static ManagerSystem Instance { get; set; }

    public GameManager GameManager { get { return gameManager; } }
    public AddressablesManager AddressablesManager { get { return addressablesManager; } }
    public GUIManager GUIManager { get { return guiManager; } }
    public ObjectPoolManager ObjectPoolManager { get { return objectPoolManager; } }

    #endregion

    #region Unity Functions

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        CreateAndFindManagers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Setup

    private void CreateAndFindManagers()
    {
        managers.ForEach(prefabManager =>
        {
            if (prefabManager != null) 
            {
                Manager manager = Instantiate(prefabManager, managerRoot.transform);

                if (gameManager == null && manager is GameManager) { gameManager = manager as GameManager; }
                else if (addressablesManager == null && manager is AddressablesManager) { addressablesManager = manager as AddressablesManager; }
                else if (guiManager == null && manager is GUIManager) { guiManager = manager as GUIManager; }
                else if (objectPoolManager == null && manager is ObjectPoolManager) { objectPoolManager = manager as ObjectPoolManager; }
            }
        });

        gameManager.AttachManagers(addressablesManager, objectPoolManager);
    }

    #endregion
}

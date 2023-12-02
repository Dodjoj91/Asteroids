using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, IInputHandlerController
{
    #region Variables

    [SerializeField] InputActionReference accelerateActionRef, rotateLeftActionRef, rotateRightActionRef, shootingActionRef;

    private bool isShooting = false;
    private bool isAccelerating = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    #endregion

    #region Properties

    public InputActionReference AccelerateActionRef { get { return accelerateActionRef; } }
    public InputActionReference RotateLeftActionRef { get { return rotateLeftActionRef; } }
    public InputActionReference RotateRightActionRef { get { return rotateRightActionRef; } }
    public InputActionReference ShootingActionRef { get { return shootingActionRef; } }
    public bool IsShooting { get { return isShooting; } }
    public bool IsAccelerating { get { return isAccelerating; } }
    public bool IsRotatingLeft { get { return isRotatingLeft; } }
    public bool IsRotatingRight { get { return isRotatingRight; } }

    #endregion

    #region Unity Functions

    private void Start()
    {
        shootingActionRef.action.performed += OnShooting;
        accelerateActionRef.action.performed += OnAccelerate;
        rotateLeftActionRef.action.performed += OnRotateLeft;
        rotateRightActionRef.action.performed += OnRotateRight;

        shootingActionRef.action.canceled += OnShooting;
        accelerateActionRef.action.canceled += OnAccelerate;
        rotateLeftActionRef.action.canceled += OnRotateLeft;
        rotateRightActionRef.action.canceled += OnRotateRight;
    }

    #endregion

    #region OnCallback Functions

    public void OnShooting(InputAction.CallbackContext callback) => isShooting = callback.action.IsPressed();

    public void OnAccelerate(InputAction.CallbackContext callback) => isAccelerating = callback.action.IsPressed();

    public void OnRotateLeft(InputAction.CallbackContext callback) => isRotatingLeft = callback.action.IsPressed();

    public void OnRotateRight(InputAction.CallbackContext callback) => isRotatingRight = callback.action.IsPressed();

    #endregion
}
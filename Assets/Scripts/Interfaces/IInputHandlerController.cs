
using UnityEngine.InputSystem;

public interface IInputHandlerController
{
    public void OnShooting(InputAction.CallbackContext callback);
    public void OnAccelerate(InputAction.CallbackContext callback);
    public void OnRotateLeft(InputAction.CallbackContext callback);
    public void OnRotateRight(InputAction.CallbackContext callback);
}
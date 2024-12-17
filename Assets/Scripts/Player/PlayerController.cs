using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Controls inputActions;
    private InputAction moveAction;

    private void Awake()
    {
        inputActions = new Controls();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        moveAction = inputActions.FindAction("Move");
        moveAction.performed += OnMove;

        moveAction.Enable();

    }

    private void OnDisable()
    {
        inputActions.Disable();

        moveAction.performed -= OnMove;
        moveAction.Disable();

    }

    public Vector2 GetMovement() 
    {
        return moveAction.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
    }
 
}

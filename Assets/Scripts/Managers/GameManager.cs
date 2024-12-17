using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPlayerDead { get; private set; } // state of the player

    private Controls inputActions;

    // Start is called before the first frame update
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inputActions = new Controls();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayerDied()
    {
        isPlayerDead = true;
       
    }

    public void ResetGame()
    {
        isPlayerDead = false;
        
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Space.performed += OnSpacePressed;
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
            inputActions.UI.Space.performed -= OnSpacePressed;
        }
    }

    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (isPlayerDead)
        {
            ResetGame();
            LoadMainMenu(); 
        }
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}

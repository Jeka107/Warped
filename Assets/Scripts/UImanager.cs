using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    [SerializeField] GameObject endCanvas;

    private bool escButton;
    private bool pauseButton;
    private bool inventoryButton;

    void Update()
    {           
        OnEsc();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Time.timeScale = 1;
    }
    public void MainMenu1()
    {
        // Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnEscButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            escButton = true;
        }
        if (context.canceled)
        {
            escButton = false;
        }
    }
    public void OnEsc()
    {
        if (escButton)
        {
            endCanvas.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1;

            Debug.Log("esc");
        }

    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}

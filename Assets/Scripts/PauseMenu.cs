using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseOverlay;
    public PlayerInput playerInput;

    private bool isPaused = false;

    void Start()
    {
        pauseOverlay.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                OpenMenu();
            else
                CloseMenu();
        }
    }

    public void OpenMenu()
    {
        isPaused = true;
        pauseOverlay.SetActive(true);
        Time.timeScale = 0f;

        // Disable all gameplay input
        if (playerInput != null)
            playerInput.DeactivateInput();
    }

    public void CloseMenu()
    {
        isPaused = false;
        pauseOverlay.SetActive(false);
        Time.timeScale = 1f;

        // Re-enable game controls
        if (playerInput != null)
            playerInput.ActivateInput();
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GMainMenu");
    }
}


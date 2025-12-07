using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    [Header("Menus")]
    public GameObject pauseMenuUI;
    public GameObject endGameMenuUI;
    public GameObject objectiveMenuUI;

    public static bool GameIsStopped = false;
    private bool objectiveMenuOpen = false;

    void Update()
    {
        // ESC = PAUSA
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeSelf)
                Resume();
            else
                Pause();
        }

        // M = OBJETIVOS
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (objectiveMenuOpen)
                HideObjectives();
            else
                ShowObjectives();
        }
    }

    // =========================
    //       PAUSA
    // =========================

    public void Pause()
    {
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsStopped = true;

        UnlockCursor();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsStopped = false;

        LockCursor();
    }

    // =========================
    //     OBJETIVOS
    // =========================

    public void ShowObjectives()
    {
        objectiveMenuUI.SetActive(true);
        objectiveMenuOpen = true;

        Time.timeScale = 0f;
        GameIsStopped = true;

        UnlockCursor();

        // 🔥 Refrescar los textos al abrir menú
        if (ObjectivesComplete.Instance != null)
            ObjectivesComplete.Instance.ShowMenu();
    }

    public void HideObjectives()
    {
        objectiveMenuUI.SetActive(false);
        objectiveMenuOpen = false;

        Time.timeScale = 1f;
        GameIsStopped = false;

        LockCursor();

        if (ObjectivesComplete.Instance != null)
            ObjectivesComplete.Instance.HideMenu();
    }

    // =========================
    //    FUNCIONES GENERALES
    // =========================

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // =========================
    //       CURSOR
    // =========================

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

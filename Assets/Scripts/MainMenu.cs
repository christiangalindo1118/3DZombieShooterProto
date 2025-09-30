using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject selectCharacter;
    public GameObject mainMenu;

    public void GoToSelectCharacter()
    {
        selectCharacter.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("ZombieLand");
    }

    public void OnQuitButton()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}

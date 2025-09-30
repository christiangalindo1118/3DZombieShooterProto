using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour
{
    public GameObject selectCharacter;
    public GameObject mainMenu;

    public void OnBackButton()
    {
        selectCharacter.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void OnCharacter1()
    {
        SceneManager.LoadScene("ZombieLand");
    }

    public void OnCharacter2()
    {
        SceneManager.LoadScene("ZombieLand1");
    }

    public void OnCharacter3()
    {
        SceneManager.LoadScene("ZombieLand2");
    }

}

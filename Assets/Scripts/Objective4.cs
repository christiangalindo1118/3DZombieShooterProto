using UnityEngine;
using UnityEngine.SceneManagement;

public class Objective4 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            if (ObjectivesComplete.Instance != null)
            {
                ObjectivesComplete.Instance.GetObjectivesDone(true, true, true, true);
            }
            else
            {
                Debug.LogWarning("ObjectivesComplete instance not found in OnTriggerEnter.");
            }

            SceneManager.LoadScene("MainMenu");
        }
    }


}

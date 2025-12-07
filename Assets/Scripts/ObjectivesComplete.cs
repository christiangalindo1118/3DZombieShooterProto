using UnityEngine;
using TMPro;
using System.Linq;

public class ObjectivesComplete : MonoBehaviour
{
    public static ObjectivesComplete Instance; // Singleton moderno

    [Header("Objectives Text")]
    public TMP_Text objective1;
    public TMP_Text objective2;
    public TMP_Text objective3;
    public TMP_Text objective4;

    [Header("Objectives Panel Container")]
    public GameObject objectivesPanel;

    // Estado real de cada objetivo
    private bool[] completed = new bool[4];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        Debug.Log("ObjectivesComplete initialized.");
    }

    private void Start()
    {
        ApplyCurrentStatesToUI();
    }

    // ========= PUBLIC API ========= //

    public void ShowMenu()
    {
        if (objectivesPanel != null)
            objectivesPanel.SetActive(true);

        Time.timeScale = 0f;
        ApplyCurrentStatesToUI();
    }

    public void HideMenu()
    {
        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void CompleteObjective(int index)
    {
        if (index < 1 || index > 4)
        {
            Debug.LogWarning("Invalid objective index: " + index);
            return;
        }

        int i = index - 1;

        if (!completed[i])
        {
            completed[i] = true;
            Debug.Log($"Objective {index} completed.");
            ApplyCurrentStatesToUI();
        }
    }

    public void SetObjectives(bool o1, bool o2, bool o3, bool o4)
    {
        completed[0] = o1;
        completed[1] = o2;
        completed[2] = o3;
        completed[3] = o4;

        ApplyCurrentStatesToUI();
    }

    /// <summary>
    /// 👇 Método faltante que causaba el error
    /// </summary>
    public void GetObjectivesDone(bool o1, bool o2, bool o3, bool o4)
    {
        SetObjectives(o1, o2, o3, o4);
    }

    // ========= INTERNAL LOGIC ========= //

    private void ApplyCurrentStatesToUI()
    {
        Debug.Log("Updating UI with states: " +
            string.Join(",", completed.Select(b => b ? "1" : "0")));

        if (objective1 != null)
        {
            objective1.text = completed[0] ? "1. Completed" : "01. Find the rifle";
            objective1.color = completed[0] ? Color.green : Color.white;
        }

        if (objective2 != null)
        {
            objective2.text = completed[1] ? "2. Completed" : "02. Locate the villagers";
            objective2.color = completed[1] ? Color.green : Color.white;
        }

        if (objective3 != null)
        {
            objective3.text = completed[2] ? "3. Completed" : "03. Find vehicle";
            objective3.color = completed[2] ? Color.green : Color.white;
        }

        if (objective4 != null)
        {
            objective4.text = completed[3] ? "4. Mission Completed" : "04. Get all villagers into vehicle";
            objective4.color = completed[3] ? Color.green : Color.white;
        }
    }
}



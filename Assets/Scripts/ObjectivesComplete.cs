using UnityEngine;
using TMPro;
using System.Linq;

public class ObjectivesComplete : MonoBehaviour
{
    [Header("Objectives to complete")]
    public TMP_Text objective1;
    public TMP_Text objective2;
    public TMP_Text objective3;
    public TMP_Text objective4;

    [Header("Menu container")]
    public GameObject objectivesPanel;

    public static ObjectivesComplete occurrence; // Singleton

    // Trackea el estado real de cada objetivo
    private bool[] completed = new bool[4];

    private void Awake()
    {
        // Patrón singleton básico
        if (occurrence != null && occurrence != this)
        {
            Destroy(gameObject);
            return;
        }
        occurrence = this;

        if (objectivesPanel != null) 
            objectivesPanel.SetActive(false);

        Debug.Log("ObjectivesComplete Awake. occurrence assigned: " + (occurrence != null));
    }

    private void Start()
    {
        // Inicializar la UI con todos los estados actuales (todo false al inicio)
        ApplyCurrentStatesToUI();
    }

    // Mostrar/ocultar menú
    public void ShowObjectivesMenu()
    {
        if (objectivesPanel != null) 
            objectivesPanel.SetActive(true);

        Time.timeScale = 0f;

        // 🔑 Forzar refresco de la UI al abrir menú
        ApplyCurrentStatesToUI();
    }

    public void HideObjectivesMenu()
    {
        if (objectivesPanel != null) 
            objectivesPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // Marcar objetivo (1..4) como completado
    public void CompleteObjective(int index)
    {
        if (index < 1 || index > 4)
        {
            Debug.LogWarning("CompleteObjective invalid index: " + index);
            return;
        }

        if (!completed[index - 1])
        {
            completed[index - 1] = true;
            Debug.Log("Objective " + index + " set to completed. Current states: " +
                      string.Join(",", completed.Select(b => b ? "1" : "0")));
            ApplyCurrentStatesToUI();
        }
        else
        {
            Debug.Log("Objective " + index + " already completed.");
        }
    }

    // Marcar varios de golpe
    public void SetObjectives(bool o1, bool o2, bool o3, bool o4)
    {
        completed[0] = o1;
        completed[1] = o2;
        completed[2] = o3;
        completed[3] = o4;

        Debug.Log("SetObjectives called. States: " +
                  string.Join(",", completed.Select(b => b ? "1" : "0")));

        ApplyCurrentStatesToUI();
    }

    // Actualizar UI según array 'completed'
    private void ApplyCurrentStatesToUI()
    {
        Debug.Log("Applying UI states. Completed array: " +
                  string.Join(",", completed.Select(b => b ? "1" : "0")));

        if (objective1 != null)
            objective1.text = completed[0] ? "1. Completed" : "01. Find the rifle";
        if (objective2 != null)
            objective2.text = completed[1] ? "2. Completed" : "02. Locate the villagers";
        if (objective3 != null)
            objective3.text = completed[2] ? "3. Completed" : "03. Find vehicle";
        if (objective4 != null)
            objective4.text = completed[3] ? "4. Mission Completed" : "04. Get all villagers into vehicle";

        // Colores
        if (objective1 != null) objective1.color = completed[0] ? Color.green : Color.white;
        if (objective2 != null) objective2.color = completed[1] ? Color.green : Color.white;
        if (objective3 != null) objective3.color = completed[2] ? Color.green : Color.white;
        if (objective4 != null) objective4.color = completed[3] ? Color.green : Color.white;
    }

    // Compatibilidad con código antiguo
    public void GetObjectivesDone(bool obj1, bool obj2, bool obj3, bool obj4)
    {
        SetObjectives(obj1, obj2, obj3, obj4);
    }
}



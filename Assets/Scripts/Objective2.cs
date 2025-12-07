using UnityEngine;

public class Objective2 : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      // Marca objetivos: 1 = true, 2 = true, 3 = false, 4 = false
      ObjectivesComplete.Instance.SetObjectives(true, true, false, false);

      Destroy(gameObject, 2f);
    }
  }
}


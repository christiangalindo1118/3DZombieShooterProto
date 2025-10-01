using UnityEngine;

public class Objective2 : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Player")
    {
      ObjectivesComplete.occurrence.GetObjectivesDone(true, true, false, false);
      
      Destroy(gameObject, 2f);
    }
  }
}

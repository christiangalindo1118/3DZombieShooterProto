using UnityEngine;

public class FootStepsSound : MonoBehaviour
{
  private AudioSource audioSource;
  
  [Header("FootSteps Sources")]
  public AudioClip[] footStepsSounds;

  private void Awake()
  {
    audioSource = GetComponent<AudioSource>();
  }

  private AudioClip GetRandomFootStep()
  {
    return footStepsSounds[UnityEngine.Random.Range(0, footStepsSounds.Length)];
  }

  private void Step()
  {
    AudioClip clip = GetRandomFootStep();
    audioSource.PlayOneShot(clip);
  }
}

using UnityEngine;

public class FootStepsSound : MonoBehaviour
{
  [Header("FootSteps Sources")]
  [SerializeField] private AudioClip[] footStepsSounds;
    
  [Header("Audio Settings")]
  [SerializeField][Range(0.8f, 1.2f)] private float pitchVariation = 0.1f;
    
  private AudioSource audioSource;
  private int lastIndex = -1;

  private void Awake()
  {
    audioSource = GetComponent<AudioSource>();
        
    if (audioSource == null)
    {
      Debug.LogWarning($"No AudioSource found on {gameObject.name}");
      audioSource = gameObject.AddComponent<AudioSource>();
    }
        
    audioSource.playOnAwake = false;
  }

  /// <summary>
  /// Call this method from Animation Events or code
  /// </summary>
  public void Step()
  {
    if (footStepsSounds == null || footStepsSounds.Length == 0)
    {
      Debug.LogWarning("No footstep sounds assigned!");
      return;
    }

    AudioClip clip = GetRandomFootStep();
    if (clip != null)
    {
      audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
      audioSource.PlayOneShot(clip);
    }
  }

  private AudioClip GetRandomFootStep()
  {
    if (footStepsSounds.Length == 1)
      return footStepsSounds[0];

    int randomIndex;
    do
    {
      randomIndex = Random.Range(0, footStepsSounds.Length);
    }
    while (randomIndex == lastIndex && footStepsSounds.Length > 1);

    lastIndex = randomIndex;
    return footStepsSounds[randomIndex];
  }
}

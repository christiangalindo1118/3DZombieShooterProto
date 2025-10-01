using UnityEngine;

public class RiflePickUp : MonoBehaviour
{
    [Header("Rifle")] 
    public GameObject PlayerRifle;
    public GameObject PickupRifle;
    public PlayerPunch playerPunch;
    public GameObject rifleUI;

    [Header("Rifle Assign Things")] 
    public PlayerScript player;
    private float radius = 2.5f;
    public Animator animator;
    private float nextTimeToPunch = 0f;
    public float punchCharge = 15f;
    
    private bool hasRifle = false;

    private void Awake()
    {
        if (PlayerRifle != null) PlayerRifle.SetActive(false);
        if (rifleUI != null) rifleUI.SetActive(false); // UI del rifle empieza desactivada
    }

    private void Update()
    {
        if (!hasRifle)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToPunch)
            {
                if (animator != null)
                {
                    animator.SetBool("Punch", true);
                    animator.SetBool("Idle", false);
                }
                nextTimeToPunch = Time.time + 1f / punchCharge;
                if (playerPunch != null) playerPunch.Punch();
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("Punch", false);
                    animator.SetBool("Idle", true);
                }
            }
        }

        // Pickup logic using distance
        if (!hasRifle && player != null && Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupTheRifle();
            }
        }
    }

    private void PickupTheRifle()
    {
        if (PlayerRifle != null) PlayerRifle.SetActive(true);
        if (PickupRifle != null) PickupRifle.SetActive(false);
        if (rifleUI != null) rifleUI.SetActive(true);
        hasRifle = true;

        if (animator != null)
        {
            animator.SetBool("Punch", false);
            animator.SetBool("Idle", true);
        }

        InitializeRifleUI();

        // marcar objetivo 1
        var oc = ObjectivesComplete.occurrence ?? FindObjectOfType<ObjectivesComplete>();
        if (oc != null)
        {
            oc.CompleteObjective(1);
            oc.ShowObjectivesMenu(); // opcional, abrir el menu cuando recoja el rifle
        }
        else
        {
            Debug.LogWarning("ObjectivesComplete instance not found. Objective not marked.");
        }

        Debug.Log("Rifle recogido - UI activada desde RiflePickUp");
    }

    private void InitializeRifleUI()
    {
        Rifle rifleComponent = PlayerRifle != null ? PlayerRifle.GetComponent<Rifle>() : null;
        if (rifleComponent != null && AmmoAcount.occurrence != null)
        {
            AmmoAcount.occurrence.UpdateAmmoText(32);
            AmmoAcount.occurrence.UpdateMagText(rifleComponent.mag);
        }
    }
}


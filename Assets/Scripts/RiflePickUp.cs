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
        PlayerRifle.SetActive(false);
        rifleUI.SetActive(false); // UI del rifle empieza desactivada
    }

    private void Update()
    {
        // Solo permitir golpes si NO tiene el rifle
        if (!hasRifle)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToPunch)
            {
                animator.SetBool("Punch", true);
                animator.SetBool("Idle", false);
                
                nextTimeToPunch = Time.time + 1f / punchCharge;
                playerPunch.Punch();
            }
            else
            {
                animator.SetBool("Punch", false);
                animator.SetBool("Idle", true);
            }
        }

        // Lógica para recoger el rifle
        if (!hasRifle && Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupTheRifle();
            }
        }
    }

    private void PickupTheRifle()
    {
        // Activar rifle del jugador
        PlayerRifle.SetActive(true);
        
        // Desactivar rifle del suelo
        PickupRifle.SetActive(false);
        
        // Activar UI del rifle
        rifleUI.SetActive(true);
        
        // Marcar que ya tiene rifle
        hasRifle = true;
        
        // Desactivar animaciones de puño
        if (animator != null)
        {
            animator.SetBool("Punch", false);
            animator.SetBool("Idle", true);
        }
        
        // Inicializar la UI con valores iniciales
        InitializeRifleUI();
        
        Debug.Log("Rifle recogido - UI activada desde RiflePickUp");
    }

    private void InitializeRifleUI()
    {
        // Obtener el componente Rifle para inicializar la UI
        Rifle rifleComponent = PlayerRifle.GetComponent<Rifle>();
        if (rifleComponent != null && AmmoAcount.occurrence != null)
        {
            // Inicializar UI con munición actual
            AmmoAcount.occurrence.UpdateAmmoText(32); // maxinumAmmunition
            AmmoAcount.occurrence.UpdateMagText(rifleComponent.mag);
        }
    }
}

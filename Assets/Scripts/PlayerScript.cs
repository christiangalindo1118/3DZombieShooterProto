
using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Movement")] 
    public float PlayerSpeed = 1.9F;

    public float PlayerSprint = 3f;

    [Header("Player Health Things")] 
    private float playerHealth = 120f;

    public float presentHealth;
    public GameObject PlayerDamage;
    public HealthBar healthbar;
    
    [Header("Player Script Cameras")]
    public Transform PlayerCamera;

    [Header(" Player animator and Gravity")]
    public CharacterController cC;
    public float gravity = -9.81F;
    public Animator animator;

    [Header("Player Jumping and velocity")]
    public float turnCalmTime = 0.1f;
    float turnCalmVelocity;
    public float jumpRange = 1f;
    Vector3 velocity;
    public Transform surfaceCheck;
    private bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        presentHealth = playerHealth;
        healthbar.GiveFullHealth(playerHealth);
    }

    private void Update()
    {
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);

        if (onSurface  && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);
        
        playerMove();
        
        Jump();

        Sprint();
    }

    void playerMove()
    {
        float horizontal_axis = Input.GetAxis("Horizontal");
        float vertical_axis = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Running", false);
            animator.SetBool("RifleWalk", false);
            animator.SetBool("IdleAim", false);
            
            
            
            

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + PlayerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cC.Move(moveDir.normalized * PlayerSpeed * Time.deltaTime);

        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Running", false);
            animator.SetBool("IdleAim", false);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && onSurface)
        {
            animator.SetBool("Idle", false);
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.ResetTrigger("Jump");
        }
    }

    void Sprint()
    {
        if (Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
        {
            float horizontal_axis = Input.GetAxis("Horizontal");
            float vertical_axis = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;
            
            if (direction.magnitude >= 0.1f)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Running", true);
                

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + PlayerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDir.normalized * PlayerSprint * Time.deltaTime);

            }
            else
            {
                animator.SetBool("Walk", true);
                animator.SetBool("Running", false); 
            }
        }
    }

    public void playerHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;
        StartCoroutine(playerDamage());
        
        healthbar.SetHealth(presentHealth);

        if (presentHealth <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        Cursor.lockState = CursorLockMode.None;
        Object.Destroy(gameObject, 1.0f);
    }

    IEnumerator playerDamage()
    {
        PlayerDamage.SetActive(true);
        yield return new WaitForSeconds(1.18f);
        PlayerDamage.SetActive(false);
    }

}

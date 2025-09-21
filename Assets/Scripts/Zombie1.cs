using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Zombie1 : MonoBehaviour
{
    [Header("Zombie Health and Damage")]
    private float zombieHealth = 100f;
    private float presentHealth;
    public float giveDamage = 5f;
    public HealthBar healthBar;

    [Header("Zombie Things")]
    public NavMeshAgent zombieAgent;
    public Transform LookPoint;
    public Camera AttackingRaycastArea;
    public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Zombie Guarding Var")]
    public GameObject[] walkPoints;
    private int currentZombiePosition = 0;
    public float zombieSpeed;
    private float walkingpointRadius = 2;

    [Header("Zombie Attacking Var")]
    public float timeBtwAttack = 2f;
    private bool previouslyAttack;

    [Header("Zombie Animation")]
    public Animator anim;

    [Header("Zombie mood/states")]
    public float visionRadius = 10f;
    public float attackingRadius = 3f;
    public bool playerInvisionRadius;
    public bool playerInattackingRadius;

    // ---- helpers internos ----
    private const float NAVMESH_SAMPLE_RADIUS = 5f;

    private void Awake()
    {
        presentHealth = zombieHealth;
    
        // Verificar si healthBar está asignado antes de usarlo
        if (healthBar != null)
        {
            healthBar.GiveFullHealth(zombieHealth);
        }
    
        zombieAgent = GetComponent<NavMeshAgent>();
        if (zombieAgent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en " + gameObject.name);
            return;
        }
    
        EnsureAgentOnNavMesh();
    }
    
    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            playerBody = playerObj.transform;
            // Si no tienes LookPoint asignado, usa el playerBody
            if (LookPoint == null) LookPoint = playerBody;
        }
    }

    private void Update()
    {
        if (playerBody == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerBody = playerObj.transform;
        }
        
        playerInvisionRadius    = Physics.CheckSphere(transform.position, visionRadius,    PlayerLayer);
        playerInattackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);

        if (!playerInvisionRadius && !playerInattackingRadius)
            Guard();
        else if (playerInvisionRadius && !playerInattackingRadius)
            PursuePlayer();
        else if (playerInvisionRadius && playerInattackingRadius)
            AttackPlayer();
    }

    private void Guard()
    {
        if (walkPoints == null || walkPoints.Length == 0) return;

        if (Vector3.Distance(walkPoints[currentZombiePosition].transform.position, transform.position) < walkingpointRadius)
        {
            int newPoint;
            do { newPoint = Random.Range(0, walkPoints.Length); }
            while (newPoint == currentZombiePosition);
            currentZombiePosition = newPoint;
        }

        TrySetDestination(walkPoints[currentZombiePosition].transform.position);

        if (anim)
        {
            anim.SetBool("Walking", true);
            anim.SetBool("Running", false);
            anim.SetBool("Attacking", false);
        }
    }

    private void PursuePlayer()
    {
        if (playerBody == null) return;
        if (!EnsureAgentOnNavMesh()) return;

        if (zombieAgent.stoppingDistance != 1.5f)
            zombieAgent.stoppingDistance = 1.5f;

        if (zombieAgent.speed != zombieSpeed)
            zombieAgent.speed = zombieSpeed;

        bool ok = zombieAgent.SetDestination(playerBody.position);

        if (anim)
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Running", ok);
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", false);
        }
    }

    private void AttackPlayer()
    {
        // mantener posición sin provocar error si aún no está en NavMesh
        TrySetDestination(transform.position);

        // mirar al punto (usa .position si es Transform)
        if (LookPoint)
        {
            Vector3 lookPos = LookPoint.position - transform.position;
            lookPos.y = 0; // bloquea la rotación vertical
            if (lookPos != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookPos);
        }


        if (!previouslyAttack)
        {
            if (AttackingRaycastArea != null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(AttackingRaycastArea.transform.position,
                        AttackingRaycastArea.transform.forward,
                        out hitInfo, attackingRadius))
                {
                    Debug.Log("Attacking " + hitInfo.transform.name);

                    // evita sombrear el campo playerBody
                    var player = hitInfo.transform.GetComponent<PlayerScript>();
                    if (player != null)
                    {
                        player.playerHitDamage(giveDamage);
                    }

                    if (anim)
                    {
                        anim.SetBool("Attacking", true);
                        anim.SetBool("Running", false);
                    }
                }
            }

            previouslyAttack = true;
            if (timeBtwAttack > 0f) Invoke(nameof(ActiveAttacking), timeBtwAttack);
            else ActiveAttacking();
        }
    }

    private void ActiveAttacking()
    {
        previouslyAttack = false;
    }

    public void zombieHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;
        healthBar.SetHealth(presentHealth);

        if (presentHealth <= 0)
        {
            if (anim)
            {
                anim.SetBool("Walking", false);
                anim.SetBool("Running", false);
                anim.SetBool("Attacking", false);
                anim.SetBool("Died", true);
            }

            zombieDie();
        }
    }

    private void zombieDie()
    {
        TrySetDestination(transform.position);

        zombieSpeed = 0f;
        attackingRadius = 0f;
        visionRadius = 0f;
        playerInattackingRadius = false;
        playerInvisionRadius = false;

        // Desactivar el NavMeshAgent
        if (zombieAgent != null)
        {
            zombieAgent.enabled = false;
        }

        Object.Destroy(gameObject, 3f); // Aumenté el tiempo para ver la animación de muerte
    }

    // ----------------- utilidades internas -----------------

    private bool EnsureAgentOnNavMesh()
    {
        if (zombieAgent == null || !zombieAgent.enabled) return false;
        if (zombieAgent.isOnNavMesh) return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, NAVMESH_SAMPLE_RADIUS, NavMesh.AllAreas))
        {
            zombieAgent.Warp(hit.position);
            return true;
        }
        return false;
    }

    private bool TrySetDestination(Vector3 target)
    {
        if (!EnsureAgentOnNavMesh()) return false;
        return zombieAgent.SetDestination(target);
    }

    // Para debug - visualizar los rangos en el Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackingRadius);
    }
}


using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Zombie2 : MonoBehaviour
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
    
    public float zombieSpeed;
    

    [Header("Zombie Attacking Var")]
    public float timeBtwAttack;

    private bool previouslyAttack;

    [Header("Zombie Animation")]
    public Animator anim;

    [Header("Zombie mood/states")]
    public float visionRadius;
    public float attackingRadius;
    public bool playerInvisionRadius;
    public bool playerInattackingRadius;

    // ---- helpers internos ----
    private const float NAVMESH_SAMPLE_RADIUS = 5f;

    private void Awake()
    {
        presentHealth = zombieHealth;
        healthBar.GiveFullHealth(zombieHealth);
        zombieAgent = GetComponent<NavMeshAgent>();
        EnsureAgentOnNavMesh(); // coloca el agente en el NavMesh si aún no lo está
        // zombieAgent.speed = zombieSpeed; // si quieres, descomenta
        
        
    }

    private void Update()
    {
        playerInvisionRadius    = Physics.CheckSphere(transform.position, visionRadius,    PlayerLayer);
        playerInattackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);

        if (!playerInvisionRadius && !playerInattackingRadius)
            Idle();
        else if (playerInvisionRadius && !playerInattackingRadius)
            PursuePlayer();
        else if (playerInvisionRadius && playerInattackingRadius)
            AttackPlayer();
    }

    private void Idle()
    {
        zombieAgent.SetDestination(transform.position);
        anim.SetBool("Idle", true);
        anim.SetBool("Running", false);
    }
        

    private void PursuePlayer()
    {
        if (zombieAgent.SetDestination(playerBody.position))
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Running", true);
            anim.SetBool("Attacking", false);
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
               
                anim.SetBool("Died", true);
            }

            zombieDie();
        }
    }

    private void zombieDie()
    {
        // no forzar SetDestination si no está en NavMesh
        TrySetDestination(transform.position);

        zombieSpeed = 0f;
        attackingRadius = 0f;
        visionRadius = 0f;
        playerInattackingRadius = false;
        playerInvisionRadius = false;

        Object.Destroy(gameObject, 0.5f);
    }

    // ----------------- utilidades internas -----------------

    /// Coloca el agente en el NavMesh si aún no lo está (evita el error de SetDestination).
    private bool EnsureAgentOnNavMesh()
    {
        if (zombieAgent == null || !zombieAgent.enabled) return false;
        if (zombieAgent.isOnNavMesh) return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, NAVMESH_SAMPLE_RADIUS, NavMesh.AllAreas))
        {
            zombieAgent.Warp(hit.position); // lo “asienta” en el NavMesh
            return true;
        }
        return false;
    }

    /// Envuelve SetDestination de forma segura.
    private bool TrySetDestination(Vector3 target)
    {
        if (!EnsureAgentOnNavMesh()) return false;
        return zombieAgent.SetDestination(target);
    }
}

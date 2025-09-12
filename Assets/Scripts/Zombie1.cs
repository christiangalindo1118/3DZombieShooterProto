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
        zombieAgent = GetComponent<NavMeshAgent>();
        EnsureAgentOnNavMesh(); // coloca el agente en el NavMesh si aún no lo está
        // zombieAgent.speed = zombieSpeed; // si quieres, descomenta
    }
    
    private void Start()
    {
        // Solo se busca el player una vez
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerBody = playerObj.transform;
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
        // evita errores si no hay puntos
        if (walkPoints == null || walkPoints.Length == 0) return;

        if (Vector3.Distance(walkPoints[currentZombiePosition].transform.position, transform.position) < walkingpointRadius)
        {
            int newPoint;
            do { newPoint = Random.Range(0, walkPoints.Length); }
            while (newPoint == currentZombiePosition); // evita repetir
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
        // 1. Verifica que haya Player
        if (playerBody == null) return;

        // 2. Verifica que el agente esté en el NavMesh
        if (!EnsureAgentOnNavMesh()) return;

        // 3. Actualiza parámetros del agente (solo si cambiaron)
        if (zombieAgent.stoppingDistance != 1.5f)
            zombieAgent.stoppingDistance = 1.5f;

        if (zombieAgent.speed != zombieSpeed)
            zombieAgent.speed = zombieSpeed;

        // 4. Fija destino al Player (posición ACTUAL en cada frame)
        bool ok = zombieAgent.SetDestination(playerBody.position);

        // 5. Animaciones según estado
        if (anim)
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Running", ok);   // correr si va hacia el Player
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", !ok);     // si no pudo moverse → muerto
        }
    }

   

    private void AttackPlayer()
    {
        // mantener posición sin provocar error si aún no está en NavMesh
        TrySetDestination(transform.position);

        // mirar al punto (usa .position si es Transform)
        if (LookPoint) transform.LookAt(LookPoint.position);

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
                        anim.SetBool("Walking", false);
                        anim.SetBool("Running", false);
                        anim.SetBool("Attacking", true);  // corregido: atacar activo
                        anim.SetBool("Died", false);
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



using UnityEngine;
using DG.Tweening;
using System.Collections;


public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] Transform[] patrolPoints;      // Points de patrouille
    [SerializeField] float patrolSpeed = -3f;          // Vitesse de déplacement
    [SerializeField] float chaseSpeed = 5f;
    private int currentPointIndex = 0;    // Index du point actuel        // Compteur pour l'attente

    [Header("Chasing Parameters")]
    private bool isChasing = false;
    [SerializeField] float detectionRadius = 4f;
    [SerializeField] float chaseTime = 4f;          // Durée de poursuite après avoir perdu le joueur
    [SerializeField] float chaseTimer = 0f;

    [Header("Obstacle Avoidance")]
    [SerializeField] float obstacleDetectionDistance = 1.0f;
    [SerializeField]float avoidanceStrength = 1.5f;

    [Header("Attack")]
    [SerializeField] float attackDelay = 5f;
    [SerializeField] float lastAttackTime = 0f;    // Pour le cooldown d'attaque
    [SerializeField] float attackDetectionRadius = 2f;
    [SerializeField] float jumpDistance = 0f;



    [Header("Idle-Tired")]
    [SerializeField]float tiredTime = 10f;

    [Header("Layers and references")]
    private Rigidbody2D rb;
    public Vector2 direction;    // Référence au Rigidbody2D, Referencé dans EnemyAnimator
    public LayerMask obstacleLayer;
    public Transform playerTransform; // Référence au joueur
    // Layer des obstacles
    [SerializeField] Animator animator;
    [SerializeField] Animator playerAnimator;// Référence au joueur
    private bool isTired = false;
    private SpriteRenderer spriteRenderer;
    private bool isAttackFinished = false;
    private bool isAttacking = false;
    [SerializeField] private EnemyCombatSystem enemyCombatSystem;
    [SerializeField] private PlayerCombatSystem playerCombatSystem;

    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Idle
    }

    private EnemyState currentState = EnemyState.Patrol;




    private void Start()
    {
        // Vérifier si des points de patrouille sont définis
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("Aucun point de patrouille défini pour " + gameObject.name);
            enabled = false;  // Désactiver ce script
        }

    }

    private void Awake()

    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyCombatSystem = GetComponent<EnemyCombatSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Chercher le joueur s'il n'est pas assigné
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
                playerCombatSystem = player.GetComponent<PlayerCombatSystem>();
        }
    }

    private void Update()
    {
        if (!playerTransform)
            return;

        // Déterminer l'état actuel en fonction de la distance au joueur
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);


        // Vérifier si le joueur est dans le rayon de détection
        if (distanceToPlayer <= detectionRadius && distanceToPlayer>attackDetectionRadius && !isTired)
        {
            // Vérifier s'il y a une ligne de vue vers le joueur
            bool hasLineOfSight = !Physics2D.Raycast(transform.position,
                (playerTransform.position - transform.position).normalized,
                distanceToPlayer, obstacleLayer);

            if (hasLineOfSight)
            {
                // Le joueur est visible, commencer la poursuite
                currentState = EnemyState.Chase;
                isChasing = true;
                chaseTimer = chaseTime;
            }
        }

        // Si on est en poursuite mais que le joueur n'est plus visible
        if (isChasing && currentState == EnemyState.Chase && distanceToPlayer > detectionRadius && distanceToPlayer> attackDetectionRadius && !isTired)
        {
            // Réduire le temps de poursuite
            chaseTimer -= Time.deltaTime;

            // Si le temps est écoulé, retourner à la patrouille
            if (chaseTimer <= 0)
            {
                isChasing = false;
                currentState = EnemyState.Patrol;
            }
        }

        //Si on est pas en idle ni Attack, que le player est dans la zone d'attack. 
        if (currentState != EnemyState.Idle && currentState != EnemyState.Attack)
        {
            if (distanceToPlayer <= attackDetectionRadius) 
            {
                if (Time.time - lastAttackTime >= attackDelay)
                {
                    currentState = EnemyState.Attack;
                }
            }
        }

        // Exécuter le comportement correspondant à l'état actuel
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
            case EnemyState.Idle:
                HandleIdle();
                break;
        }
    }

    private void HandleIdle()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("MoveMagnitude", 0);

        float pingPongValue = Mathf.PingPong(Time.time, 1f);
        spriteRenderer.color = Color.Lerp(Color.white, Color.yellow, pingPongValue);

        // Only return to chase if tired time has passed AND we're far enough from the player
        if (Time.time - lastAttackTime > tiredTime)
        {
            isAttacking = false;
            currentState = EnemyState.Chase;
            isTired = false;
            spriteRenderer.color = Color.white;
        }
    }
  

    private void HandlePatrol()
    {
        Vector2 targetPosition = patrolPoints[currentPointIndex].position;

        // Calculer la direction vers la cible
        direction = (targetPosition - (Vector2)transform.position).normalized;

        // Déplacer l'ennemi vers la cible
        rb.linearVelocity = direction * patrolSpeed;

        // Orienter l'ennemi dans la direction du mouvement
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
        }

        // Vérifier si l'ennemi est arrivé au point de patrouille
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Passer immédiatement au point suivant sans attendre
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
    private void HandleChase()
    {
        // Calculer la direction vers le joueur
        direction = (playerTransform.position - transform.position).normalized;

        // Déplacer l'ennemi vers le joueur
        rb.linearVelocity = direction * chaseSpeed;

        // Orienter l'ennemi vers le joueur
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
        }
    }

    private void HandleAttack()
    {
        // Arrêter le mouvement pendant l'attaque
        rb.linearVelocity = Vector2.zero;

        if (!playerTransform || !enemyCombatSystem)
            return; 
        // Orienter l'ennemi vers le joueur
        direction = (playerTransform.position - transform.position).normalized;
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
        }

        if (Time.time - lastAttackTime >= attackDelay)
        {
            if(isAttacking == false)
            {
                isAttacking = true;
                direction = (playerTransform.position - transform.position).normalized;
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                float maxJumpRange = jumpDistance;
                float actualJumpDistance = Mathf.Min(distanceToPlayer, maxJumpRange);
                Vector2 targetPosition = (Vector2)transform.position + direction * (actualJumpDistance - 0.2f);
                rb.DOMove(targetPosition, 0.5f, false);
                enemyCombatSystem.Attack();
            }
        }
    }

  

    private void AttackFinish()
    {
        lastAttackTime = Time.time;  // Restore this line
        isTired = true;  // Restore this line
        isAttackFinished = true;
        currentState = EnemyState.Idle;

    }

}

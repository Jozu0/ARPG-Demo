using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;      // Points de patrouille
    public float patrolSpeed = -2f;          // Vitesse de déplacement

    public float chaseSpeed = 3f;
    private int currentPointIndex = 0;    // Index du point actuel        // Compteur pour l'attente
    private Rigidbody2D rb;
    public Vector2 direction;         // Référence au Rigidbody2D
    public bool dead;

    [Header("Health Settings")]
    public int maxHealth = 5;      // Santé maximale que le joueur peut avoir
    public int currentHealth;        // Santé actuelle du joueur (initialisée dans Start) 
    // --- PARAMÈTRES DE COMBAT ---
    [Header("Combat Settings")]
    public int attackDamage = 1;    // Quantité de dégâts infligés par une attaque
    public float attackRange = 1.0f;  // Distance à laquelle le joueur peut attaquer
    public float attackCooldown1 = 1f;
    public float attackCooldown2 = 2f; // Temps minimum entre deux attaques (en secondes)

    public float invincibilityFrame = 1f;
    private float lastHit;
    // Temps minimum entre deux attaques (en secondes)
    public LayerMask enemyLayers;    // Couches (Layers) qui contiennent les ennemis
    public LayerMask obstacleLayer;   
    public Transform playerTransform;     // Référence au joueur
    // Layer des obstacles
    private Animator animator;
    public Animator playerAnimator;
       // Variables pour la poursuite
    private bool isChasing = false;  
    public float detectionRadius = 4f;
    public float chaseTime = 4f;          // Durée de poursuite après avoir perdu le joueur

    private float chaseTimer = 0f;


    private enum EnemyState
    {
        Patrol,
        Chase
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
        currentHealth = maxHealth;
        DOTween.Init();
    }

    private void Awake()

    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Chercher le joueur s'il n'est pas assigné
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (!playerTransform)
            return;

        // Déterminer l'état actuel en fonction de la distance au joueur
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Vérifier si le joueur est dans le rayon de détection
        if (distanceToPlayer <= detectionRadius)
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
        if (isChasing && currentState == EnemyState.Chase && distanceToPlayer > detectionRadius)
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

        // Exécuter le comportement correspondant à l'état actuel
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;
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
        // Utiliser la vitesse de poursuite (plus rapide)

        // Calculer la direction vers le joueur
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Déplacer l'ennemi vers le joueur
        rb.linearVelocity = direction * chaseSpeed;

        // Orienter l'ennemi vers le joueur
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamage(int damage)
    {
        // Dans cette version, les dégâts sont fixés à 10 pour simplifier
        // Réduire la santé par le montant de dégâts
        if (Time.time - lastHit < invincibilityFrame)
            return;
        lastHit = Time.time;
        currentHealth -= damage;
        // Jouer l'animation de dégâts si un Animator existe
        if (animator)
        {
            Debug.Log("Hit");
            animator.SetTrigger("Hit");
        }
        float x = playerAnimator.GetFloat("LastX");
        float y = playerAnimator.GetFloat("LastY");

        // Vérifier si le joueur est mort (santé ≤ 0)
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Vector2 knockbackDirection = new Vector2(x, y).normalized * 2f;
            Vector2 targetPosition = (Vector2)transform.position + knockbackDirection;
            rb.DOMove(targetPosition, 0.5f, false);
        }
    }

    private void Die()
    {

        // Jouer l'animation de mort si un Animator existe
        if (animator)
        {
            animator.SetTrigger("Die");

        }

        // Désactiver le contrôleur de joueur pour empêcher tout mouvement
        // Remarque: D'autres actions pourraient être ajoutées ici
        // Comme afficher un écran de game over, jouer un son, etc.
    }
    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}

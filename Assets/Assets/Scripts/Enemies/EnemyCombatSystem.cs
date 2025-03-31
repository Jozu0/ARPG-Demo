using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;



public class EnemyCombatSystem : MonoBehaviour
{
    // --- PARAMÈTRES DE SANTÉ ---
    [Header("Health Settings")]
    public int maxHealth = 5;           // Santé maximale de l'ennemi
    private int currentHealth;           // Santé actuelle (initialisée dans Start)

    // --- PARAMÈTRES DE COMBAT ---
    [Header("Combat Settings")]
    public int attackDamage = 1;         // Dégâts infligés par chaque attaque
    public float attackRange = 1.0f;     // Portée de l'attaque (en unités Unity)
    public float attackCooldown = 1.0f;  // Temps minimum entre deux attaques (en secondes)
                                         // --- PARAMETRES DE HITBOX ---
    [Header("Colliders")]
    public Collider2D SlimeWorldCollision;
    public Collider2D SlimeHitBox;
    public Collider2D SlimeDamageBox;

    [Header("Layers and references")]
    private Rigidbody2D rb;
    public Vector2 direction; // Référence au Rigidbody2D, Referencé dans EnemyAnimator
    private Animator animator;
    public Animator playerAnimator;// Référence au joueur
    [SerializeField] private EnemyCombatSystem enemyCombatSystem;
    [SerializeField] private PlayerCombatSystem playerCombatSystem;

    // --- PARAMÈTRES DE RÉCOMPENSE ---
    [Header("Reward Settings")]
    public int experienceReward = 10;    // Points d'expérience donnés au joueur à la mort
    public GameObject[] possibleDrops;   // Tableau d'objets que l'ennemi peut laisser tomber
    [Range(0, 1)]                        // [Range] limite la valeur entre 0 et 1
    public float dropChance = 0.3f;      // Probabilité de laisser tomber un objet (30% par défaut)

    // --- VARIABLES PRIVÉES ---
    private Enemy enemy;             // Référence au composant Enemy
    private float invincibilityFrame = 1f;
    private float lastAttackTime;
    private float lastHit = 0f;



    // Awake est appelé quand l'objet est initialisé, avant Start
    private void Awake()
    {
        // Récupérer les composants nécessaires sur ce même GameObject
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    // Start est appelé avant la première frame
    private void Start()
    {
        // Initialiser la santé actuelle à la santé maximale
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        DOTween.Init();
    }

    // Méthode appelée quand l'ennemi prend des dégâts
    [ContextMenu("TakeDamage")]
    public void TakeDamage(int damage)
    {

        // Réduire la santé par le montant de dégâts - Vérifier si la frame d'invinsibilité est active.
        if (Time.time - lastHit < invincibilityFrame)
            return;
        lastHit = Time.time;
        currentHealth -= damage;
        // Jouer l'animation de dégâts si un Animator existe
        if (playerAnimator)
        {
            Debug.Log("Hit");
            animator.SetTrigger("Hit");
        }
        Vector2 knockbackDirection =  new Vector2(playerAnimator.GetFloat("LastX"),  // Récupération de la direction où regarde le player
                                                  playerAnimator.GetFloat("LastY")).normalized * 2f;
        Vector2 targetPosition = (Vector2)transform.position + knockbackDirection;
        rb.DOMove(targetPosition, 0.5f, false); // L'enemy est kick dans la direction où regarde le player.
            
        // Vérifier si le joueur est mort (santé ≤ 0)
        if (currentHealth <= 0)
        {
            Die();
        }
    }      

    private void Die()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; 
        enabled = false; 

        // Jouer l'animation de mort si un Animator existe
        if (animator)
        {
            animator.SetTrigger("Die");
            SlimeWorldCollision.enabled = false;
            SlimeHitBox.enabled = false;
            SlimeDamageBox.enabled = false;
            // Désactivation de toutes les collisions avant le destroy pour pouvoir passer au travers de l'enemy mort.
        }
        PlayerCombatSystem player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();
        if (player != null)
        {
            // Si vous avez un système d'expérience, ajoutez l'expérience ici
            // player.AddExperience(experienceReward);

            // Pour l'instant, on peut juste l'afficher dans la console
            Debug.Log("Joueur reçoit " + experienceReward + " points d'expérience");
        }

        // Potentiellement faire tomber un objet
        DropItem();
    }
    public void DestroyAfterDeath() // Fonction appelé dans l'animation event
                                    // lors de la fin d'anim de mort
    {
        Destroy(gameObject);
    }
    
    

    // Méthode pour attaquer le joueur
    public void Attack()
    {
        // Vérifier si le cooldown est terminé 
        if (Time.time - lastAttackTime < attackCooldown )
            return;  // Sortir de la fonction si le cooldown n'est pas terminé

        // Mettre à jour le temps de la dernière attaque
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
    }


    // Méthode privée pour faire tomber un objet aléatoire
    private void DropItem()
    {
        // Si aucun objet possible ou le tirage au sort échoue, ne rien faire
        if (possibleDrops.Length == 0 || Random.value > dropChance)
            return;

        // Sélectionner un objet aléatoire parmi les possibles
        int randomIndex = Random.Range(0, possibleDrops.Length);
        GameObject drop = possibleDrops[randomIndex];

        // Instantier (créer) l'objet à la position de l'ennemi
        if (drop != null)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }
}

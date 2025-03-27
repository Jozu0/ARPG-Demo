using UnityEngine;
using UnityEngine.Events; 
using System.Collections.Generic;
using DG.Tweening;



public class PlayerCombatSystem : MonoBehaviour
{
   // --- PARAMÈTRES DE SANTÉ ---
    [Header("Health Settings")]
    public int maxHealth = 5;      // Santé maximale que le joueur peut avoir
    public int currentHealth;        // Santé actuelle du joueur (initialisée dans Start)
    public GameObject lifeBar;
    private List<GameObject> hearts = new List<GameObject>();
    public GameObject heart;

    public Rigidbody2D rb;
  
   // --- PARAMÈTRES DE MANA ---
   [Header("Mana Settings")]
   public int maxMana = 50;         // Mana maximale que le joueur peut avoir
   public int currentMana;          // Mana actuelle du joueur (initialisée dans Start)
  
   // --- PARAMÈTRES DE COMBAT ---
   [Header("Combat Settings")]
   public int attackDamage = 1;    // Quantité de dégâts infligés par une attaque
   public float attackRange = 1.0f;  // Distance à laquelle le joueur peut attaquer
   public float attackCooldown1 = 1f;
   public float attackCooldown2 = 2f; // Temps minimum entre deux attaques (en secondes)
    // Temps minimum entre deux attaques (en secondes)
   public LayerMask enemyLayers;    // Couches (Layers) qui contiennent les ennemis
   public Collider2D hitCollider;
  
   // --- ÉVÉNEMENTS ---
   // Les UnityEvents permettent de connecter ce script à d'autres systèmes (comme l'UI)
   // sans créer de dépendances directes
   [Header("Events")]
   public UnityEvent<int, int> onHealthChanged;  // Déclenché quand la santé change (santé actuelle, santé max)
   public UnityEvent<int, int> onManaChanged;    // Déclenché quand la mana change (mana actuelle, mana max)
   public UnityEvent onPlayerDeath;              // Déclenché quand le joueur meurt
  
   // --- VARIABLES PRIVÉES ---
    private float invincibilityFrame = 1f;
   private float lastAttackTime1;    // Moment de la dernière attaque (pour le cooldown)
   private float lastAttackTime2;    // Moment de la dernière attaque (pour le cooldown)
   private float lastHit = 0f;

   private Animator animator;       // Référence au composant Animator pour les animations
   public Animator enemyAnimator;
   public Animator hitAnimator;
  
   // Awake est appelé quand l'objet est initialisé, avant Start
   private void Awake()
   {
       // Récupérer le composant Animator attaché à ce même GameObject
       animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
   }
  
   // Start est appelé avant la première mise à jour
   private void Start()
   {
       // Initialiser la santé et la mana à leurs valeurs maximales
        for (int i = 0; i <(maxHealth); i++)
        {
            GameObject hp = Instantiate(heart, lifeBar.transform);
            hearts.Add(hp);
            currentHealth = maxHealth;    
        }
            currentMana = maxMana;
        
       // Déclencher les événements pour initialiser l'UI
       // Le ? avant Invoke est un "opérateur de propagation nulle"
       // qui vérifie si onHealthChanged n'est pas null avant d'appeler Invoke
       onHealthChanged?.Invoke(currentHealth, maxHealth);
       onManaChanged?.Invoke(currentMana, maxMana);
   }
  
   // [ContextMenu] permet de tester cette fonction directement depuis l'inspecteur Unity
   // en faisant un clic droit sur le composant
   [ContextMenu("Attack1")]
   public void Attack1()
   {
        bool hasWeaponEquipped = animator.GetBool("SwordEquipped") || animator.GetBool("PickaxeEquipped");
        if (!hasWeaponEquipped)
        return; 
       // Vérifier si le cooldown est terminé
       if (Time.time - lastAttackTime1 < attackCooldown1)
           return;  // Sortir de la fonction si le cooldown n'est pas terminé
          
       // Mettre à jour le temps de la dernière attaque
       lastAttackTime1 = Time.time;
      
       // Jouer l'animation d'attaque si un Animator existe
       if (animator)
       {
           animator.SetTrigger("Attack1");
           hitAnimator.SetTrigger("Attack1");

       }

   }   


   public void Attack2()
   {
       // Vérifier si le cooldown est terminé
       if (Time.time - lastAttackTime2 < attackCooldown2)
           return;  // Sortir de la fonction si le cooldown n'est pas terminé
          
       // Mettre à jour le temps de la dernière attaque
       lastAttackTime2 = Time.time;
      
       // Jouer l'animation d'attaque si un Animator existe
       if (animator)
       {
           animator.SetTrigger("Attack2");
           
       }
      
       // Note: Ici, il manque le code qui détecterait et infligerait des dégâts aux ennemis
       // C'est probablement intentionnel pour simplifier l'exemple ou sera ajouté plus tard
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
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        MinusXHeart(damage);
        // Jouer l'animation de dégâts si un Animator existe
        if (animator)
        {
            Debug.Log("Hit");
            animator.SetTrigger("Hit");
        }

        float x = enemyAnimator.GetFloat("LastX");
        float y = enemyAnimator.GetFloat("LastY");

        // Vérifier si le joueur est mort (santé ≤ 0)
        if (currentHealth <= 0)
        {
            Vector2 knockbackDirection = new Vector2(x, y).normalized * 2f;
            Vector2 targetPosition = (Vector2)transform.position + knockbackDirection;
            rb.DOMove(targetPosition, 0.5f, false);
            Die();
        }
        else
        {
            Vector2 knockbackDirection = new Vector2(x, y).normalized * 2f;
            Vector2 targetPosition = (Vector2)transform.position + knockbackDirection;
            rb.DOMove(targetPosition, 0.5f, false);
        }
   }
  
//    // Fonction pour restaurer de la santé (par exemple avec une potion)
   public void RestoreHealth(int amount)
   {
        // currentHealth -= damage;

        // GameObject heartToRemove = hearts.Add; 
   }
  
//    // Fonction pour restaurer de la mana (par exemple avec une potion)
//    public void RestoreMana(int amount)
//    {
//        // Augmenter la mana mais sans dépasser le maximum
//        currentMana = Mathf.Min(currentMana + amount, maxMana);
      
//        // Mettre à jour l'UI
//        onManaChanged?.Invoke(currentMana, maxMana);
//    }
  
//    // Fonction pour utiliser de la mana (par exemple pour lancer un sort)
//    // Retourne true si le joueur a assez de mana, false sinon
//    public bool UseMana(int amount)
//    {
//        // Vérifier si le joueur a assez de mana
//        if (currentMana >= amount)
//        {
//            // Réduire la mana par le montant utilisé
//            currentMana -= amount;
          
//            // Mettre à jour l'UI
//            onManaChanged?.Invoke(currentMana, maxMana);
          
//            // Le joueur avait assez de mana
//            return true;
//        }
      
//        // Le joueur n'avait pas assez de mana
//        return false;
//    }
  
//    // Fonction privée appelée quand le joueur meurt
   private void Die()
   {
       // Jouer l'animation de mort si un Animator existe
       if (animator)
       {
           animator.SetTrigger("Die");
       }
      
       // Désactiver le contrôleur de joueur pour empêcher tout mouvement
       PlayerController playerController = GetComponent<PlayerController>();
       if (playerController)
           playerController.enabled = false;
      
       // Déclencher l'événement de mort
       // Cela peut être utilisé par d'autres systèmes pour réagir à la mort du joueur
       onPlayerDeath?.Invoke();
      
       // Remarque: D'autres actions pourraient être ajoutées ici
       // Comme afficher un écran de game over, jouer un son, etc.
   }

   public void MinusXHeart(int damage){
        for(int i = 1; i<damage ;i++){
            GameObject heartToRemove = hearts[hearts.Count - 1]; 
            hearts.RemoveAt(hearts.Count - 1); 
            Destroy(heartToRemove);
        } 
          
   }
}

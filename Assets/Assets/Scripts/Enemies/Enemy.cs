using UnityEngine;

public class Enemy : MonoBehaviour
{
   [Header("Patrol Settings")]
   public Transform[] patrolPoints;      // Points de patrouille
   public float patrolSpeed = -2f;          // Vitesse de déplacement
   public float waitTime = 1f;           // Temps d'attente à chaque point
  
   private int currentPointIndex = 0;    // Index du point actuel
   private bool isWaiting = false;       // L'ennemi attend-il à un point?
   private float waitTimer = 0f;         // Compteur pour l'attente
   private Rigidbody2D rb;      
   
   public Vector2 direction;         // Référence au Rigidbody2D
  
   private void Awake()
   {
       // Récupérer le composant Rigidbody2D
       rb = GetComponent<Rigidbody2D>();
   }
  
   private void Start()
   {
       // Vérifier si des points de patrouille sont définis
       if (patrolPoints.Length == 0)
       {
           Debug.LogWarning("Aucun point de patrouille défini pour " + gameObject.name);
           enabled = false;  // Désactiver ce script
       }

   }
  
   private void Update()
   {    
        // Rest of your Update method...
       // Si l'ennemi attend à un point de patrouille
       if (isWaiting)
       {
           // Décrémenter le compteur d'attente
           waitTimer -= Time.deltaTime;
          
           // Si le temps d'attente est écoulé
           if (waitTimer <= 0)
           {
               isWaiting = false;  // Arrêter d'attendre
              
               // Passer au point suivant (en bouclant si nécessaire)
               currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
           }
          
           // Ne pas continuer le reste de la fonction pendant l'attente
           return;
       }
      
       // Récupérer la position cible actuelle
       Vector2 targetPosition = patrolPoints[currentPointIndex].position;
      
       // Calculer la direction vers la cible
       direction = (targetPosition - (Vector2)transform.position).normalized;
      
       // Déplacer l'ennemi vers la cible
       rb.linearVelocity = direction * patrolSpeed;
      
       // Orienter l'ennemi dans la direction du mouvement
       if (direction.x != 0)
       {
	// Un autre moyen de faire comme le Flip() de notre PlayerController
           transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
       }
    Debug.Log("Direction: " + direction + ", Velocity: " + rb.linearVelocity);  

       // Vérifier si l'ennemi est arrivé au point de patrouille
       if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
       {
           // Arrêter le mouvement
           rb.linearVelocity = Vector2.zero;
          
           // Commencer à attendre
           isWaiting = true;
           waitTimer = waitTime;
       }
   }
  }

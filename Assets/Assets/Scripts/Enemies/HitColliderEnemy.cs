using UnityEngine;

public class HitColliderEnemy : MonoBehaviour
{
    private BoxCollider2D myCollider;
    
    private int playerDamage;
    
    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        // Start with the collider disabled
        playerDamage = GetComponentInParent<PlayerCombatSystem>().attackDamage;
    }
    
    public void EnableCollider()
    {
        myCollider.enabled = true;
    }
    
    public void DisableCollider()
    {
        myCollider.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to an enemy
        EnemyCombatSystem enemy = other.GetComponent<EnemyCombatSystem>();
        
        if (enemy != null)
        {
            // Deal damage to the enemy
            enemy.TakeDamage(playerDamage);
            
            // You can add effects here, like particle effects or camera shake
            Debug.Log("Hit enemy: " + other.name);
        }else{
        }
    }
}
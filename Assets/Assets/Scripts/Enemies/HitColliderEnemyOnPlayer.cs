using UnityEngine;

public class HitColliderEnemyOnPlayer : MonoBehaviour
{
    private BoxCollider2D myCollider;
    
    private int enemyDamage;
    
    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        // Start with the collider disabled
        enemyDamage = GetComponentInParent<EnemyCombatSystem>().attackDamage;
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
    {        // Check if the collider belongs to an enemy
        PlayerCombatSystem player = other.GetComponentInParent<PlayerCombatSystem>();
        string name = other.gameObject.name;
      
        if ((player != null) && (name == "PlayerHitBox"))
        {
                // Deal damage to the enemy
                player.TakeDamage(enemyDamage);       
        }
    }
}
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Enemy enemyScript;

    private Vector2 lastMovementDirection;
    private Vector2 enemyMove;


    void Start()
    {
    }

    void Update()
    {
        Animate();
    }
    void Animate()
    {
        enemyMove = new Vector2(enemyScript.direction.x,enemyScript.direction.y);
        lastMovementDirection = enemyMove;
        if ((enemyMove.x != 0 || enemyMove.y != 0))
        {
            lastMovementDirection = enemyMove;
        }else{
            lastMovementDirection.x = 0;
            lastMovementDirection.y = 0;
        }
    
        enemyAnimator.SetFloat("MoveX", enemyMove.x);
        enemyAnimator.SetFloat("MoveY", enemyMove.y);
        enemyAnimator.SetFloat("LastX", lastMovementDirection.x);
        enemyAnimator.SetFloat("LastY", lastMovementDirection.y);
        enemyAnimator.SetFloat("MoveMagnitude", enemyMove.magnitude);
        
 
    }
}

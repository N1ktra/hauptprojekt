using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knightEnemy : Enemy
{
    public float attackRange = 2f;
    public float movementSpeed = 1f; //how many nodes per second
    public float attackDelay = 2f; //how long in the animation til it hits
    public override IEnumerator Behavior()
    {
        while (true)
        {
            if (!bsp_manager.PlayerIsInRoom(bsp_manager.getRoomOf(gameObject)))
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
            if(path != null && path.Count < 4)
            {
                CalculatePath();
                Debug.Log("calculatePath");
            }
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                ChangeState(EnemyState.ATTACKING);
                Debug.Log("attack");
                yield return new WaitForSeconds(0);
                attack();
                yield return new WaitForSeconds(1);
            }
            else if (path != null && path.Count > 0)
            {
                ChangeState(EnemyState.MOVING);
                Debug.Log("move");
                move();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                ChangeState(EnemyState.IDLE);
                Debug.Log("idle");
                yield return new WaitForSeconds(1f);
            }
        }
    }
    private void move()
    {
        Vector3 nextPos = path.Count > 1 ? path[1].worldPosition : path[0].worldPosition;

        transform.DOMove(nextPos, 1 / movementSpeed).SetEase(Ease.Linear);
        transform.DOLookAt(nextPos, .25f);
    }
    private void attack()
    {
        transform.DOLookAt(player.transform.position, .25f);
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            player.GetComponent<PlayerStats>().takeDamage(attackDamage);
        }
    }
}

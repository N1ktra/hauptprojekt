using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public float attackRange = 2f;
    public float movementSpeed = 1f; //how many nodes per second

    public override IEnumerator Behavior()
    {
        while(true)
        {
            if (!bsp_manager.PlayerIsInRoom(bsp_manager.getRoomOf(gameObject)))
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            CalculatePath();
            if(path != null)
            {
                Vector3 pos = transform.position;
                foreach(Node node in path)
                {
                    Debug.DrawLine(pos, node.worldPosition, Color.white, 1f);
                    pos = node.worldPosition;
                }
            }
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                ChangeState(EnemyState.ATTACKING);
                attack();
            }
            else if(path != null && path.Count > 0)
            {
                ChangeState(EnemyState.MOVING);
                move();
            }
            else
            {
                ChangeState(EnemyState.IDLE);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void move()
    {
        Vector3 nextPos = path[0].worldPosition;
        if (path.Count > 1)
            nextPos = path[1].worldPosition;

        transform.DOMove(nextPos, 1 / movementSpeed).SetEase(Ease.Linear);
        transform.DOLookAt(nextPos, .25f);
    }

    private void attack()
    {
        transform.DOLookAt(player.transform.position, .25f);
    }
}

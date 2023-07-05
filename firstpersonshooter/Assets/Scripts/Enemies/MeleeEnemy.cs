using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MeleeEnemy : Enemy
{
    public float attackRange = 2f;
    public float movementSpeed = 1f; //how many nodes per second
    public float attackDelay = .2f; //how long in the animation til it hits

    private float refreshRate = .5f;
    private bool isColliding = false;

    public override void Start()
    {
        base.Start();
        movementSpeed /= pathfinding.grid.nodeRadius;
    }

    public override IEnumerator Behavior()
    {
        while(state != EnemyState.DYING)
        {
            if (!bsp_manager.PlayerIsInRoom(bsp_manager.getRoomOf(gameObject)))
            {
                yield return new WaitForSeconds(refreshRate);
                continue;
            }
            if(state == EnemyState.HIT && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return new WaitForSeconds(.1f);
                continue;
            }

            CalculatePath();
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                stopMove();
                ChangeState(EnemyState.ATTACKING);
                yield return new WaitForSeconds(0);
                attack();
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            }
            else if(path != null && path.Count > 0 && !isColliding)
            {
                ChangeState(EnemyState.MOVING);
                move();
                yield return new WaitForSeconds(refreshRate);
            }
            else
            {
                ChangeState(EnemyState.IDLE);
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    private Tween moveTween;
    private void move()
    {
        if (isColliding) return;
        stopMove();

        Vector3 nextPos = path.Count > 1 ? path[1].worldPosition : path[0].worldPosition;

        moveTween = transform.DOMove(nextPos, 1 / movementSpeed).SetEase(Ease.Linear);
        transform.DOLookAt(nextPos, .25f);
    }

    private void stopMove()
    {
        if(moveTween != null)
        {
            moveTween.Kill();
        }
    }

    private void attack()
    {
        transform.DOLookAt(player.transform.position, .25f);
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        if(Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            player.GetComponent<PlayerStats>().takeDamage(attackDamage);
        }
    }

    public override void Update()
    {
        base.Update();

        //check if other enemies are close => if so stop moving
        int layermask = LayerMask.GetMask("enemy");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange / 2, layermask);
        foreach(var hitCollider in  hitColliders)
        {
            if(hitCollider.gameObject != gameObject && 
                Vector3.Distance(hitCollider.transform.position, player.transform.position) < Vector3.Distance(transform.position, player.transform.position))
            {
                stopMove();
                isColliding = true;
                return;
            }
        }
        isColliding = false;
    }

}

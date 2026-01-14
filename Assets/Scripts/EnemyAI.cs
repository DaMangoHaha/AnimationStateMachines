using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent ai;
    public Transform patrolPoint;
    public EnemyState enemyState;
    private Animator animator;
    private float distanceToTarget;
    private Coroutine idleToPatrol;


    public void Start()
    {
        ai = GetComponent<NavMeshAgent>();
        ai.SetDestination(target.position);
        animator = GetComponent<Animator>();
        enemyState = EnemyState.Idle;
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));
    }

    void Update()
    {
        distanceToTarget = Mathf.Abs(
            Vector3.Distance(target.position, transform.position)
        );

        switch (enemyState)
        {
            case EnemyState.Idle:
                SwitchState(0);
                ai.SetDestination(transform.position);

                if (idleToPatrol == null)
                {
                    idleToPatrol = StartCoroutine(SwitchToPatrol());
                }
                break;

            case EnemyState.Patrol:
                float distanceToPatrolPoint = Mathf.Abs(
                    Vector3.Distance(patrolPoint.position, transform.position)
                );

                if (distanceToPatrolPoint > 2f)
                {
                    SwitchState(1);
                    ai.SetDestination(patrolPoint.position);
                }
                else
                {
                    SwitchState(0);
                }
                if (distanceToTarget <= 15f)
                {
                    enemyState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                SwitchState(2);
                ai.SetDestination(target.position);
                if (distanceToTarget <= 5f)
                {
                    enemyState = EnemyState.Attack;
                }
                else if (distanceToTarget > 15f)
                {
                    enemyState = EnemyState.Idle;
                }
                break;
            case EnemyState.Attack:
                SwitchState(3);

                if (distanceToTarget > 5f && distanceToTarget <= 15f)
                {
                    enemyState = EnemyState.Chase;
                }
                else if (distanceToTarget > 15f)
                {
                    enemyState = EnemyState.Idle;
                }
                break;

            default:
                enemyState = EnemyState.Idle;
                break;
        }
    }
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    private IEnumerator SwitchToPatrol()
    {
        yield return new WaitForSeconds(5f);
        enemyState = EnemyState.Patrol;
        idleToPatrol = null;
    }

    private void SwitchState(int newState)
    {
        if (animator.GetInteger("State") != newState)
        {
            animator.SetInteger("State", newState);
        }
    }
}
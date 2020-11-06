using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class W5_AIMovement : MonoBehaviour
{
    public enum EAIState
    {
        None,
        Idle,
        MoveAround,
        MoveToPlayer,
        Attack,
        TakeDamage,
        Dead,
    }

    public float randomMoveRange = 3.0f;
    public Transform findingPlayerPoint;
    public W5_StatusSystem statusSystem;

    private W5_PlayerMovement playerTarget;
    private NavMeshAgent navAgent;
    public EAIState aiState;
    private Animator animator;

    private EAIState previosState;

    private void Start()
    {
        navAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        ChangeState(EAIState.Idle);
    }

    private void OnEnable()
    {
        statusSystem = this.GetComponent<W5_StatusSystem>();

        statusSystem.OnTakeDamage += OnTakeDamage;
        statusSystem.OnDead += OnDead;
    }

    private void OnDisable()
    {
        statusSystem.OnTakeDamage -= OnTakeDamage;
        statusSystem.OnDead -= OnDead;
    }

    public void ChangeState(EAIState toSetState)
    {
        if (aiState == toSetState)
            return;

        previosState = aiState;
        aiState = toSetState;

        switch(aiState)
        {
            case EAIState.Idle:
            {
                StartCoroutine(IEIdle());
                break;
            }
            case EAIState.MoveAround:
            {
                StartCoroutine(IEMoveAround());
                break;
            }
            case EAIState.MoveToPlayer:
            {
                StartCoroutine(IEMoveToPlayer());
                break;
            }
            case EAIState.Attack:
            {
                StartCoroutine(IEAttack());
                break;
            }
            case EAIState.TakeDamage:
            {
                StartCoroutine(IETakeDamage());
                break;
            }
            case EAIState.Dead:
            {
                StartCoroutine(IEDead());
                break;
            }
        }
    }

    private bool IsPlayerFound()
    {
        if (playerTarget)
            return true;

        RaycastHit hit;
        Ray ray = new Ray();

        ray.origin = findingPlayerPoint.position;
        ray.direction = findingPlayerPoint.forward;

        bool isHit = Physics.Raycast(ray, out hit, 100);
        Debug.DrawLine(ray.origin, ray.origin + (ray.direction * 100), Color.green);

        if(isHit)
        {
            playerTarget = hit.collider.gameObject.GetComponent<W5_PlayerMovement>();

            if(playerTarget)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator IEIdle()
    {
        animator.SetBool("IsMove", false);
        navAgent.speed = 0.0f;
        navAgent.velocity = Vector3.zero;

        float waitTimeIdle = Random.Range(1.0f, 3.0f);

        while(true)
        {
            waitTimeIdle -= Time.deltaTime;

            if(IsPlayerFound())
            {
                ChangeState(EAIState.MoveToPlayer);
                break;
            }

            if(waitTimeIdle <= 0)
            {
                ChangeState(EAIState.MoveAround);
                break;
            }

            yield return null;
        }
    }

    private IEnumerator IEMoveAround()
    {
        Vector3 randomPos = Vector3.zero;
        randomPos.x = Random.Range(this.transform.position.x - randomMoveRange, this.transform.position.x + randomMoveRange);
        randomPos.y = this.transform.position.y;
        randomPos.z = Random.Range(this.transform.position.z - randomMoveRange, this.transform.position.z + randomMoveRange);

        animator.SetBool("IsMove", true);

        float randomSpeed = Random.Range(0.25f, 1.5f);
        navAgent.speed = randomSpeed;

        while (true)
        {
            Vector3 targetPos = randomPos;

            float distFromTarget = Vector3.Distance(targetPos, this.transform.position);

            if(distFromTarget > navAgent.stoppingDistance)
            {
                navAgent.SetDestination(targetPos);

                animator.SetFloat("Movement", navAgent.velocity.magnitude / 1.5f);

                if(IsPlayerFound())
                {
                    ChangeState(EAIState.MoveToPlayer);
                    break;
                }
            }
            else
            {
                ChangeState(EAIState.Idle);
                break;
            }

            yield return null;
        }
    }

    private IEnumerator IEMoveToPlayer()
    {
        animator.SetBool("IsMove", true);

        float randomSpeed = Random.Range(0.25f, 1.5f);
        navAgent.speed = randomSpeed;

        while (true)
        {
            float distFromTarget = Vector3.Distance(this.transform.position, playerTarget.transform.position);

            if(distFromTarget > navAgent.stoppingDistance + 0.25f)
            {
                navAgent.SetDestination(playerTarget.transform.position);

                animator.SetFloat("Movement", navAgent.velocity.magnitude / 1.5f);
            }
            else
            {
                ChangeState(EAIState.Attack);
                break;
            }

            yield return null;
        }
    }

    private IEnumerator IEAttack()
    {
        int randomIndexAtk = Random.Range(0, 2);

        animator.SetInteger("IndexAttack", randomIndexAtk);

        animator.SetTrigger("IsAttack");

        yield return null;
    }

    private IEnumerator IETakeDamage()
    {
        animator.SetTrigger("IsTakeDamage");
        float tempSpd = navAgent.speed;
        navAgent.speed = 0.0f;
        yield return new WaitForSeconds(0.5f);
        navAgent.speed = tempSpd;

        ChangeState(EAIState.Idle);
        yield return null;
    }

    private IEnumerator IEDead()
    {
        animator.SetBool("IsDead", true);
        navAgent.speed = 0.0f;
        navAgent.velocity = Vector3.zero;
        Destroy(this.gameObject, 5.0f);

        yield return null;
    }

    public void OnTakeDamage(GameObject damageFrom, float inDamage)
    {
        Debug.Log("Zombie take damage : " + inDamage);

        playerTarget = damageFrom.GetComponent<W5_PlayerMovement>();

        if(statusSystem.IsAlive())
        {
            ChangeState(EAIState.TakeDamage);
        }
    }

    public void OnDead()
    {
        StopAllCoroutines();
        ChangeState(EAIState.Dead);
    }
}

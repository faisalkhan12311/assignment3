using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { PATROL, CHASE, ATTACK}
    //getter for current state
    
    public EnemyState CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;
            StopAllCoroutines();

            switch (currentState) 
            {
                case EnemyState.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case EnemyState.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case EnemyState.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
            }

        }
    }
    [SerializeField]
    public EnemyState currentState;


    private CheckMyvision checkMyvision = null;
    

    private NavMeshAgent agent = null;

    //private Health playerHealth = null;

    private Transform playerTransform = null;

    private Transform patrolDestination = null;

    //public float maxDamage = 10f;
    private void Awake()
    {
        checkMyvision = GetComponent<CheckMyvision>();
        agent = GetComponent<NavMeshAgent>();
        //playerHealth = GameObject.FindGameObjectsWithTag("Player").GetComponent<Health>();
        //playerTransform = playerHealth.GetComponent<Transform>();
    }

    void Start()
    {
        //random destination
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Destinations");
        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = EnemyState.PATROL;
    }

    public IEnumerator EnemyPatrol()
    {
        print("Patrolling");

        //yield break;
        while (currentState == EnemyState.PATROL)
        {
            checkMyvision.sensitivity = CheckMyvision.enumsensitivity.HIGH;
            agent.isStopped = false; 
            agent.SetDestination(patrolDestination.position);

            while (agent.pathPending)
                yield return null;
            

            if (checkMyvision.TargetinSight)
            {
                print("Found you");
                //agent.isStopped = true;
                //print("changing state");
                currentState = EnemyState.CHASE;
                //print("state = chasing");
                yield break;
            }
            yield break;
        }
        

    }
    public IEnumerator EnemyChase()
    {
        
        print("Chasing");
        while (currentState == EnemyState.CHASE)
        {
            print("chawhile");
            checkMyvision.sensitivity = CheckMyvision.enumsensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyvision.LastknowSight);

            while (agent.pathPending)
            {
                yield return null;
            }

            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;

                if (!checkMyvision.TargetinSight)
                {
                    Debug.Log("Target not in sight back to patrolin");
                    CurrentState = EnemyState.PATROL;

                }else
                {
                    Debug.Log("On target so attacking");
                    CurrentState = EnemyState.ATTACK;
                }    
                    
                yield break;
            }
            yield return null;
        }
    }
    
    public IEnumerator EnemyAttack()
    {
        //yield break;
        while(currentState == EnemyState.ATTACK)
        {
            Debug.Log("attacking");
            //CurrentState = EnemyState.ATTACK;
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            while (agent.pathPending)
                yield return null;
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                CurrentState = EnemyState.CHASE;
                yield break;
            }
            else
            {
                //attack
                //playerHealth.healthPoints -= maxDamage * Time.deltaTime;
                //yield break;
            }
            yield return null;
        }
        yield break;
    }
    
}

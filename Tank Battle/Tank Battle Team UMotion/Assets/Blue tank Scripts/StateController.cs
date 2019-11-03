using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    TankData tankData;
    private NavMeshAgent agent;
    public bool flocking = true;
    public states curState;
    private IState state;
    private float health;
    public GameObject bullet;
    public Transform bulletSpawnPoint;
    public float wanderTimer = 3f;
    private float timer;
    private List<GameObject> spottedEnemies=new List<GameObject>();
    private List<GameObject> allies = new List<GameObject>();
    public GameObject turret;
    private float attackTimer;

    public enum states
    {
        Wandering,
        Attacking,
        Fleeing,
        Chasing,
        Evading,
        Dead
    }

    // Start is called before the first frame update
    void Start()
    {
        tankData = new TankData();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = tankData.maxSpeed;
        agent.angularSpeed = tankData.maxRotationSpeed;
        foreach(GameObject tank in GameObject.FindGameObjectsWithTag("Blue"))
        {
            allies.Add(tank);
            allies.Remove(this.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
            switchStates();
            state.doAction();
    }

    void switchStates()
    {
       
        switch (curState)
        {
            
            case states.Wandering when !enemySpotted():
                state = new WanderState(agent, transform, canWander(),flocking) ;
                curState = states.Wandering;
                break;
            case states.Wandering when enemySpotted() && !isFleeing():
                state = new ChaseState(GetClosestEnemy(getSpottedEnemy()), agent, transform);
                curState = states.Chasing;
                break;
            case states.Chasing when enemySpotted() && !isFleeing():
                state = new ChaseState(GetClosestEnemy(getSpottedEnemy()), agent, transform);
                curState = states.Chasing;
                break;
            case states.Attacking when inAttackRange() && canFire() && !isFleeing() && inLineOfsight():
                state = new AttackState(turret, GetClosestEnemy(getSpottedEnemy()), bulletSpawnPoint, bullet, this);
                curState = states.Attacking;
                break;
            case states.Wandering when needEvade():
                state = new EvadeState(agent,transform);
                curState = states.Evading;
                break;
            case states.Evading when needEvade():
                state = new EvadeState(agent, transform);
                curState = states.Evading;
                break;
            case states.Evading when !needEvade():
                state = new WanderState(agent, transform, canWander(),flocking);
                curState = states.Wandering;
                break;
            default:
                state = new WanderState(agent, transform, canWander(),flocking);
                curState = states.Wandering;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Bullet")
        {
            hasBeenHit();
        }
    }

    bool canWander ()
    {
        timer = timer + Time.deltaTime;
        Debug.Log(timer + " " + (timer >= wanderTimer) + " " + wanderTimer);
        if (timer >= wanderTimer)
        {
            timer = 0;
            return true;
        }
        return false;
    }
    bool enemySpotted()
    {
        
        if (spottedEnemies.Count >= 1)
        {
            return true;
        }
        else return false;
    }

    bool inAttackRange()
    {
        var dist = Vector3.Distance(GetClosestEnemy(getSpottedEnemy()).transform.position, this.transform.position);
        if (enemySpotted())
        {
            if (dist < tankData.maxAttackRange && dist > tankData.minAttackRange)
            {
                return true;
            }
        }
        return false;
    }

    bool canFire()
    {
        attackTimer = attackTimer + Time.deltaTime;
        if (attackTimer >= tankData.shootRate)
        {
            return true;
        }
        return false;
    }

    void hasBeenHit()
    {
        if(health <= 0)
        {
            curState = states.Dead;
            this.gameObject.SetActive(false);
        }
        if (healthCritical())
        {
            state = new FleeState(agent, GetClosestEnemy(spottedEnemies),transform);
            curState = states.Fleeing;
        }
    }

    bool isFleeing()
    {
        if(curState == states.Fleeing)
        {
            return true;
        }
        return false;
    }

    bool healthCritical()
    {
        if(health >= tankData.criticalHealth)
        {
            return true;
        }

        return false;
    }
    
    bool onBulletPath()
    {
        return false;
    }

    bool inLineOfsight()
    {
        return false;
    }

     List<GameObject> getSpottedEnemy() {
        return spottedEnemies;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag != this.gameObject.tag && other.gameObject.tag != "Untagged" && other.gameObject.tag != "Bullet")
        {
            if (!spottedEnemies.Contains(other.gameObject))
            {
                spottedEnemies.Add(other.gameObject);
            }
        }
    }



    private bool needEvade()
    {
        bool react = false;
        foreach (GameObject allie in allies) {
            if (Vector3.Distance(transform.position, allie.transform.position) < 150)
            {
                react= true;
                
                
            }
            else react= false;
            return react;

        }return false;

        
    }

    GameObject GetClosestEnemy(List<GameObject> enemies)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
    public List<GameObject> getClosebyAllies(NavMeshAgent agent, float radius)
    {
        List<GameObject> agentList = new List<GameObject>();

        foreach (var otherAgent in allies)
        {
            if (otherAgent != agent && Vector3.Distance(agent.transform.position, otherAgent.transform.position) < radius)
            {
                agentList.Add(otherAgent);
            }
        }

        return agentList;
    }

    public void resetAttackTimer()
    {
        attackTimer = 0;
    }
}

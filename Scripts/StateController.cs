using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    TankData tankData;
    private NavMeshAgent agent;

    public states curState;
    private IState state;
    private float health;

    public float wanderTime = 3f;
    public GameObject turret;
    private float wanderTimer;
    private float attackTimer;
    public GameObject bullet;
    public Transform bulletSpawnPoint;
    private List<GameObject> spottedEnemies = new List<GameObject>();
    private List<GameObject> allies = new List<GameObject>();
    GameObject[] team;
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
        bullet = Instantiate(bullet);
        bullet.SetActive(false);
        health = tankData.maxHealth;
    }

    private void Awake()
    {
        team = GameObject.FindGameObjectsWithTag(this.gameObject.tag);

        foreach (GameObject tank in team)
        {
            allies.Add(tank);
        }
        allies.Remove(this.gameObject);
       // print(allies.Count);
    }

    // Update is called once per frame
    void Update()
    {
            switchStates();
            state.doAction();
        //  print(state);
        //print(state);
        print(inAttackRange() + " " + canFire() + " " + isFleeing());
    }

    void switchStates()
    {
        switch (curState)
        {
            case states.Wandering when !enemySpotted():
                state = new WanderState(agent, transform, canWander()) ;
                curState = states.Wandering;
                break;
            case states.Wandering when enemySpotted() && !isFleeing():
                state = new ChaseState(GetClosestEnemy(getSpottedEnemy()), agent, transform);
                curState = states.Chasing;
                break;
            case states.Chasing when enemySpotted() && !isFleeing() && !inAttackRange():
                state = new ChaseState(GetClosestEnemy(getSpottedEnemy()), agent, transform);
                curState = states.Chasing;
                break;
            case states.Chasing when inAttackRange() && canFire() && !isFleeing():
                agent.isStopped = true;
                state = new AttackState(turret, GetClosestEnemy(getSpottedEnemy()), bulletSpawnPoint, bullet, this);
                curState = states.Attacking;
                break;
            case states.Attacking when inAttackRange() && !canFire() && !isFleeing():
                agent.isStopped = false;
                state = new ChaseState(GetClosestEnemy(getSpottedEnemy()), agent, transform);
                curState = states.Chasing;
                break;
            case states.Wandering when needEvade():
                state = new EvadeState(agent, transform);
                curState = states.Evading;
                break;
            case states.Evading when needEvade():
                state = new EvadeState(agent, transform);
                curState = states.Evading;
                break;
            case states.Evading when !needEvade():
                state = new WanderState(agent, transform, canWander());
                curState = states.Wandering;
                break;
            default:
                state = new WanderState(agent, transform, canWander());
                curState = states.Wandering;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Bullet")
        {
            updateHealth();
        }
    }

    bool canWander ()
    {
        wanderTimer = wanderTimer + Time.deltaTime;
        if (wanderTimer >= wanderTime)
        {
            wanderTimer = 0;
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

    void updateHealth()
    {
        //health -= tankData.bulletDamage;
       
        if(health <= 0)
        {
            curState = states.Dead;
            Destroy(this.gameObject);
        }
        if (healthCritical())
        {
            state = new FleeState();
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != this.gameObject.tag && other.gameObject.tag != "Untagged" && other.gameObject.tag != "Bullet")
        {
            if (!spottedEnemies.Contains(other.gameObject))
            {
                spottedEnemies.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != this.gameObject.tag && other.gameObject.tag != "Untagged" && other.gameObject.tag != "Bullet")
        {
            if (spottedEnemies.Contains(other.gameObject))
            {
                spottedEnemies.Remove(other.gameObject);
            }
        }
    }

    private bool needEvade()
    {
        bool react = false;
        foreach (GameObject ally in allies)
        {
            if (Vector3.Distance(transform.position, ally.transform.position) < 2000)
            {
                react = true;
            }
        }
        return react;
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

    List<GameObject> getSpottedEnemy()
    {
        return spottedEnemies;
    }

    public void resetAttackTimer()
    {
        attackTimer = 0;
    }
}

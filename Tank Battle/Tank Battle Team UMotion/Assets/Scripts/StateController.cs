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

    public float wanderTimer = 3f;
    private float timer;
    private List<GameObject> spottedEnemies=new List<GameObject>();
    private List<GameObject> allies = new List<GameObject>();

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
                state = new WanderState(agent, transform, canWander()) ;
                curState = states.Wandering;
                break;
            case states.Wandering when enemySpotted() && !isFleeing():
                state = new ChaseState(getSpottedEnemy(), agent, transform);
                curState = states.Chasing;
                break;
            case states.Chasing when enemySpotted() && !isFleeing():
                state = new ChaseState(getSpottedEnemy(), agent, transform);
                curState = states.Chasing;
                break;
            case states.Attacking when inAttackRange() && canFire() && !isFleeing() && inLineOfsight():
                state = new AttackState();
                curState = states.Attacking;
                break;
            case states.Wandering when onBulletPath():
                state = new EvadeState(agent,transform,needEvade());
                curState = states.Evading;
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
        return false;
    }

    bool canFire()
    {
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

     List<GameObject> getSpottedEnemy() {
        return spottedEnemies;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag != this.gameObject.tag && other.gameObject.tag != "Untagged")
        {
            if (!spottedEnemies.Contains(other.gameObject))
            {
                Debug.Log(other.gameObject.tag);
                spottedEnemies.Add(other.gameObject);
            }
        }
    }

    private bool needEvade()
    {
        bool needEvade = false;
        foreach (GameObject allie in allies) {
            if (Vector3.Distance(transform.position, allie.transform.position) < 150)
            {
                needEvade = true;
            }
      }
        return needEvade;
    }
}

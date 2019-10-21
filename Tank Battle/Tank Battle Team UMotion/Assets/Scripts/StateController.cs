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
            case states.Chasing when enemySpotted() && !isFleeing():
                state = new ChaseState(getSpottedEnemy(), agent, transform);
                curState = states.Chasing;
                break;
            case states.Attacking when inAttackRange() && canFire() && !isFleeing() && inLineOfsight():
                state = new AttackState();
                curState = states.Attacking;
                break;
            case states.Evading when onBulletPath():
                state = new EvadeState();
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
        return false;
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

     GameObject getSpottedEnemy() {
        return null;
    }
}

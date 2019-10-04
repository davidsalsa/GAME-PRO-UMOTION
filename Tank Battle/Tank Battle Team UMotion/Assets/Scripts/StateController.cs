using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    TankData tankData;

    public states curState;
    private IState state;
    private int health;

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
                state = new WanderState();
                curState = states.Wandering;
                break;
            case states.Chasing when enemySpotted() && !isFleeing():
                state = new ChaseState();
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
                state = new WanderState();
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
}

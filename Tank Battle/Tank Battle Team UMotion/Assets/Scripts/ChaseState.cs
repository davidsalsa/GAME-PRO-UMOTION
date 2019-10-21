using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private GameObject target;
    private NavMeshAgent navMeshAgent;
    private Transform transform;

    public ChaseState(GameObject curTarget, NavMeshAgent agent, Transform curTransform) {
        target = curTarget;
        navMeshAgent = agent;
        transform = curTransform;
    }

    public void doAction()
    {
        navMeshAgent.SetDestination(target.transform.position);
    }

    public void Transition()
    {
        throw new System.NotImplementedException();
    }
}

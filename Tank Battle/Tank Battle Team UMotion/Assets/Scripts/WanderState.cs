using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : IState
{
    private NavMeshAgent navMeshAgent;
    private Transform currentTransform;
    private bool canWander;

    public WanderState(NavMeshAgent agent, Transform transform, bool canWander)
    {
        navMeshAgent = agent;
        currentTransform = transform;
        this.canWander = canWander;
    }

    public void doAction()
    {
        navMeshAgent.updateRotation = false;
        if (!navMeshAgent.isStopped)
        {
            var targetPosition = navMeshAgent.pathEndPosition;
            var targetPoint = new Vector3(targetPosition.x, currentTransform.position.y, targetPosition.z);
            var _direction = (targetPoint - currentTransform.position).normalized;
            var _lookRotation = Quaternion.LookRotation(_direction);

            currentTransform.rotation = Quaternion.Slerp(currentTransform.rotation, _lookRotation, 1f * Time.deltaTime);

            if (canWander)
            {
                Vector3 destination = RandomNavSphere(currentTransform.position, 100f, -1);

                navMeshAgent.SetDestination(destination);
                Debug.Log(destination);
            }
        }
        else {
            navMeshAgent.velocity = Vector3.zero;
        }

        

        
        Debug.Log(canWander);
    }

    public void Transition()
    {

        
    }

    public float wanderRadius;

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}

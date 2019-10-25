using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private GameObject target;
    private NavMeshAgent navMeshAgent;
    private Transform transform;
    private TankData tankData;

    public ChaseState(List<GameObject> curTarget, NavMeshAgent agent, Transform curTransform) {
        navMeshAgent = agent;
        transform = curTransform;
        target = GetClosestEnemy(curTarget);
        tankData = new TankData();
        
    }

    public void doAction()
    {
        Debug.Log(target.transform.position);
        navMeshAgent.updateRotation = false;
        if (!navMeshAgent.isStopped)
        {
            var targetPosition = navMeshAgent.pathEndPosition;
            var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            var _direction = (targetPoint - transform.position).normalized;
            var _lookRotation = Quaternion.LookRotation(_direction);

           transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, tankData.maxRotationSpeed * Time.deltaTime);


            Vector3 Destination = Vector3.MoveTowards(transform.position, target.transform.position, tankData.maxAttackRange);
            navMeshAgent.SetDestination(Destination);
        }
    }

    public void Transition()
    {
        throw new System.NotImplementedException();
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
}

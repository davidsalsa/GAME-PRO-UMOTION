using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeState : IState
{
    private NavMeshAgent navMeshAgent;
    private GameObject enemy;
    private Transform transform;
    private TankData tankData;

    public FleeState(NavMeshAgent agent, GameObject enemy, Transform transform)
    {
        this.navMeshAgent = agent;
        this.enemy = enemy;
        this.transform = transform;

        this.tankData = ScriptableObject.CreateInstance<TankData>();
    }

    public void doAction()
    {
        navMeshAgent.updateRotation = false;
        if (!navMeshAgent.isStopped)
        {
            var targetPosition = navMeshAgent.pathEndPosition;
            var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            var _direction = (targetPoint - transform.position).normalized;
            var _lookRotation = Quaternion.LookRotation(_direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, tankData.maxRotationSpeed * Time.deltaTime);

            Vector3 Destination = Vector3.MoveTowards(transform.position, -enemy.transform.position, tankData.maxSpeed);

            navMeshAgent.SetDestination(Destination);
        }
    }

    public void Transition()
    {
        throw new System.NotImplementedException();
    }
}

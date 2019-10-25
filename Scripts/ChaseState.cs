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


    public ChaseState(GameObject target, NavMeshAgent agent, Transform curTransform) {
        navMeshAgent = agent;
        transform = curTransform;
        this.target = target;
        tankData = ScriptableObject.CreateInstance<TankData>();

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


            Vector3 Destination = Vector3.MoveTowards(transform.position, target.transform.position, tankData.maxSpeed);
            if (Vector3.Distance(transform.position, Destination) > (tankData.maxAttackRange + tankData.minAttackRange)/2)
            {
                navMeshAgent.SetDestination(Destination);
            }
            else
            {
                Destination = transform.position;
                navMeshAgent.SetDestination(Destination);
            }
        }
    }

    public void Transition()
    {
        throw new System.NotImplementedException();
    }


}

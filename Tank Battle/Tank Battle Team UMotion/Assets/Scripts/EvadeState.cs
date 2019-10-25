﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EvadeState : IState
{
    private NavMeshAgent navMeshAgent;
    private Transform currentTransform;
    private bool needEvade;
    TankData tankData;

    public EvadeState(NavMeshAgent agent, Transform transform, bool needEvade)
    {
        navMeshAgent = agent;
        currentTransform = transform;
        this.needEvade = needEvade;
        tankData = new TankData();
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

            currentTransform.rotation = Quaternion.Slerp(currentTransform.rotation, _lookRotation, tankData.maxRotationSpeed * Time.deltaTime);

            if (needEvade)
            {
                Vector3 destination = currentTransform.position + (currentTransform.forward * 10) + (currentTransform.right * 200);

                navMeshAgent.SetDestination(destination);
                Debug.Log(destination);
            }
        }
        else
        {
            navMeshAgent.velocity = Vector3.zero;
        }
    }

    public void Transition()
    {
        throw new System.NotImplementedException();
    }
}

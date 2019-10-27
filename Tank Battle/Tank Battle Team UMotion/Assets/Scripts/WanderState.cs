using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : IState
{
    public float wanderRadius;
    public float alignmentRadius;
    public float seperationRadius;
    public float cohesionRadius;
    public float Kc;
    public float Ks;
    public float Ka;
    public float Kw;

    public float wanderJitter;
    public float wanderDistance;

    private StateController controller;

    private NavMeshAgent navMeshAgent;

    private Transform currentTransform;

    private bool canWander;

    private TankData tankData;

    private float maxFieldOfViewAngle = 120;

    public WanderState(NavMeshAgent agent, Transform transform, bool canWander)
    {
        controller = agent.gameObject.GetComponentInParent<StateController>();
        navMeshAgent = agent;
        currentTransform = transform;
        this.canWander = canWander;
        tankData = ScriptableObject.CreateInstance<TankData>();
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

            if (canWander)
            {
                Vector3 destination = RandomNavSphere(currentTransform.position, 1000f, -1);

                navMeshAgent.SetDestination(destination);
                Debug.Log(destination);
            }
        }
        else {
            navMeshAgent.velocity = Vector3.zero;
        }
    }

    public void Transition()
    {
    }

    protected virtual Vector3 Combine()
    {
        Vector3 combine = Kc * Cohesion() + Ks * Separation() + Ka * Alignment() + Kw * Wander();
        return combine.normalized;
    }

    protected Vector3 Cohesion()
    {
        Vector3 cohesion = new Vector3(0f, 0f, 0f);
        int countAgents = 0;

        var neighbours = controller.getClosebyAllies(navMeshAgent, cohesionRadius);

        // If there are no neighbours, don't do anything here
        if (neighbours.Count == 0)
        {
            return cohesion;
        }

        // Calculate the total position
        foreach (var agent in neighbours)
        {
            if (IsInFieldOfView(agent.transform.position))
            {
                cohesion += agent.transform.position;
                countAgents++;
            }
        }

        if (countAgents == 0)
        {
            return cohesion;
        }

        cohesion /= countAgents;

        // Set back to the average position
        //cohesion /= neighbours.Count;

        // Remove the current agent position
        cohesion -= currentTransform.position;

        // Normalize it to create a direction
        cohesion = Vector3.Normalize(cohesion);

        return cohesion;
    }

    protected Vector3 Separation()
    {
        Vector3 seperation = new Vector3(0f, 0f, 0f);

        var neighbours = controller.getClosebyAllies(navMeshAgent, seperationRadius);

        // If there are no neighbours, don't do anything here
        if (neighbours.Count == 0)
        {
            return seperation;
        }

        foreach (var agent in neighbours)
        {
            if (IsInFieldOfView(agent.transform.position))
            {
                Vector3 towardsMe = currentTransform.position - agent.transform.position;

                // If closer to other agent move away faster
                if (towardsMe.magnitude > 0)
                {
                    seperation += towardsMe.normalized / towardsMe.magnitude / towardsMe.magnitude;
                }
            }
        }

        return seperation.normalized;
    }

    protected Vector3 Alignment()
    {
        Vector3 alignment = new Vector3(0f, 0f, 0f);

        var allies = controller.getClosebyAllies(navMeshAgent, alignmentRadius);

        // If there are no neighbours, don't do anything here
        if (allies.Count == 0)
        {
            return alignment;
        }

        foreach (var agent in allies)
        {
            if (IsInFieldOfView(agent.transform.position))
            {
                alignment += agent.GetComponent<NavMeshAgent>().velocity;
            }
        }

        return alignment.normalized;
    }

    protected Vector3 Wander()
    {
        Vector3 wanderTarget = new Vector3(0f, 0f, 0f);

        // wander steer behavior that looks purposeful
        float jitter = wanderJitter * Time.deltaTime;

        // add a small random vector to the target's position so it jitters on the circle
        // RandomBinomial) returns a number between 0 and 1
        // The vector is only changing on the X and Z plane (2D)
        wanderTarget += new Vector3(RandomBinomial() * jitter, 0, RandomBinomial() * jitter);

        // reproject the vector back to unit circle
        wanderTarget = wanderTarget.normalized;

        // increase length to be the same as the radius of the wander circle
        wanderTarget *= wanderRadius;

        // position the target (circle) in front of the agent
        Vector3 target = wanderTarget + new Vector3(0, 0, wanderDistance);

        // project the target from local space to world space
        Vector3 targetInWorldSpace = currentTransform.TransformPoint(target);

        // steer towards it
        targetInWorldSpace -= currentTransform.position;

        return targetInWorldSpace.normalized;
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private bool IsInFieldOfView(Vector3 neighbour)
    {
        bool isIt = Vector3.Angle(navMeshAgent.velocity, neighbour - currentTransform.position) <= maxFieldOfViewAngle;
        return isIt;
    }

    private float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private GameObject turret;
    private GameObject target;
    private TankData tankData;
    private Transform bulletSpawnPoint;
    private GameObject bullet;
    private float bulletSpeed = 1500f;
    StateController stateController;


    public AttackState(GameObject turret, GameObject enemy, Transform bulletSpawnPoint, GameObject bullet, StateController stateController)
    {
        this.turret = turret;
        this.target = enemy;
        this.bulletSpawnPoint = bulletSpawnPoint;
        this.bullet = bullet;
        this.stateController = stateController;

        tankData = ScriptableObject.CreateInstance<TankData>();
    }

    public void doAction()
    {
        Vector3 direction = (target.transform.position - turret.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Debug.Log(turret);
        turret.transform.rotation = Quaternion.Slerp(
            turret.transform.rotation, lookRotation,
            tankData.maxRotationSpeed * Time.deltaTime);

        shoot();
        deleteBulletInstance();
    }

    public void Transition()
    {
        throw new System.NotImplementedException();
    }

    private void shoot()
    {
        if (lockedOn())
        {
            
            bullet.transform.position = bulletSpawnPoint.transform.position;
            bullet.transform.rotation = bulletSpawnPoint.transform.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity = turret.transform.forward * bulletSpeed;
            stateController.resetAttackTimer();
        }
    }

    private void deleteBulletInstance()
    {
        if (Vector3.Distance(turret.transform.position, bullet.transform.position) > tankData.maxAttackRange)
        {
            bullet.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
            bullet.SetActive(false);
        }
    }

    private bool lockedOn()
    {
        if (turret.transform.rotation == Quaternion.LookRotation((target.transform.position - turret.transform.position).normalized))
        {
            return true;
        }
        return false;
    }
}
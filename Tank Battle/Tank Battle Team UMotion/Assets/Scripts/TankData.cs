using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TankData", menuName = "ScriptableObjects/TankDataUmotion")]
public class TankData : MonoBehaviour
{
    public float maxSpeed = 300f;
    public float maxDamagedSpeed = 150f;
    public float maxRotationSpeed = 10f;
    public bool bDead = false;
    public float shootRate = 3f;
    public float maxHealth = 100f;
    public float criticalHealth = 50f;
    public float maxSpottingRange = 500;
    public float minAttackRange = 200;
    public float maxAttackRange = 300;
    public float bulletDamage = 25f;
    public int platoonsize = 5;
    public int maxSlope = 45;
}
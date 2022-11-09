using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class CODE_EnemyData : ScriptableObject
{
    public float patrolWaitTime;

    public float outOfRadiusDelay;

    public float patrolSpeed;

    public float chaseSpeed;

    public float attackCooldown;

    public float attackDistance;
}

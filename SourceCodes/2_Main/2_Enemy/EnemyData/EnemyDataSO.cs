using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

public enum EnemyType
{
    Melee,
    Range,
    Tank,
    Speed,
}


public abstract class EnemyDataSO : ScriptableObject
{
    // public abstract string id {get;}

    public abstract EnemyType type { get;}
    
    public float maxHp = 10;
    
    public float movementSpeed = 4;
    public float attackSpeed = 2;    
    public float attackRange = 1.5f;
    public float playerDectectionRange = 10;

    public float dmg = 10;


    public float inc_maxHp = 10;
    public float inc_movementSpeed = 0.3f;
    public float inc_dmg = 3;

    [Tooltip("시전 후 스킬이 발사되기 까지의 시간 ( 초 )")]
    public float castDelay;
    public float attackAnimationDuration=>attackAnimClip?attackAnimClip.length:0;
    public float offsetWeight = 1f;


    public EnemyProjectile prefab_enemyProjectile;

    [SerializeField]AnimationClip attackAnimClip;
    // public abstract IEnumerator CastRoutine(Enemy enemy, Vector3 targetPos);
    // public abstract void Attack(Enemy enemy, Vector3 targetPos);
    // public abstract AreaIndicator GetAttackAreaIndicator(Enemy enemy, Vector3 targetPos);
}

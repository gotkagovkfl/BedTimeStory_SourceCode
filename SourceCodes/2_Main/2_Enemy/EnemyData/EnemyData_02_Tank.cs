using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

[CreateAssetMenu(fileName = "EnemyInitData_Tank", menuName = "SO/EnemyData/02_Tank", order = int.MaxValue)]
public class EnemyData_02_Tank : EnemyDataSO
{
    public override EnemyType type => EnemyType.Tank;


    public EnemyData_02_Tank()
    {
        maxHp = 120;
    
        movementSpeed = 3000;
        attackSpeed = 3;    
        attackRange = 8f;
        playerDectectionRange = 40;

        dmg = 10;


        inc_maxHp = 10;
        inc_movementSpeed = 0.3f;
        inc_dmg = 3;


        castDelay = 0.1f;
        offsetWeight = 1.5f;
    }
    
    



    //===============================================================================

}

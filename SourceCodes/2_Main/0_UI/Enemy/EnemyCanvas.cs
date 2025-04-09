using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvas : DestroyableSingleton<EnemyCanvas>
{
    public Transform t_canvas;
    
    [SerializeField] GameObject prefab_enemyHpBar;

    void Start()
    {
        t_canvas = transform;
    }


    public Slider_EnemyHp Generate_EnemyHpBar()
    {
        return Instantiate(prefab_enemyHpBar, t_canvas).GetComponent<Slider_EnemyHp>();
    }
}

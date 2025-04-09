using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerHpSliderHandle : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform t;


    void Start()
    {
        t=transform;
        
    }

    void Update()
    {
        transform.position = target.position;
    }
}

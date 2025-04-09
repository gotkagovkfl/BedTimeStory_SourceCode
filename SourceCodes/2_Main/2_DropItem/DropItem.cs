using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DropItem : MonoBehaviour
{
    Collider _collider;
    
    void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Get();
            DestroyItem();
        }
    }

    public abstract void Get();

    public void DestroyItem()
    {
        _collider.enabled =false;
        Destroy(gameObject);
    }
}

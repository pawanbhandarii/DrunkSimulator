using System;
using UnityEngine;

public class Beer : MonoBehaviour
{
    public float AlcoholAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventBus<DrankAlcoholEvent>.Raise(new DrankAlcoholEvent()
            {
                alcholAmount =  AlcoholAmount,
            });
            
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : Drink
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHead>(out PlayerHead player))
        {
            StartDrink();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerHead>(out PlayerHead player))
        {
            StopDrink();
        }
    }
}

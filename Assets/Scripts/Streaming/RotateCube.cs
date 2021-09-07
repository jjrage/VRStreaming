using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    private Vector3 landscapeScale= new Vector3(2.5f, 2.5f, 2.5f);
    private Vector3 portraitScale= new Vector3(2f, 2f, 2f);
    DeviceOrientation oldOrientation = DeviceOrientation.Unknown;

    void Update()
    {
        gameObject.transform.Rotate(Vector3.up, 30 * Time.deltaTime);
    }
}

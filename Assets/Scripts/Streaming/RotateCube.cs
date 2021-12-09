using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotator;
    public MeshRenderer meshRenderer;

    void Update()
    {
        meshRenderer.material.SetFloat("_UVSec", 0);
    }
}

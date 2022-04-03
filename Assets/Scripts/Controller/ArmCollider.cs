using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCollider : MonoBehaviour
{
    public Action onCollide = null;

    private void OnCollisionEnter(Collision collision)
    {
        onCollide?.Invoke();
    }
}

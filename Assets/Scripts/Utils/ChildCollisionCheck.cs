using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionCheck : MonoBehaviour
{
    public Action<Collision> onCollisionEnter = null;

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }
}

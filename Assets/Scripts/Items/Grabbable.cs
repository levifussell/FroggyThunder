using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Action onDestroy = null;

    private void OnDestroy()
    {
        onDestroy?.Invoke();
    }
}

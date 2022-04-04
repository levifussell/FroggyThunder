using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Transform trackHand;
    public ConfigurableJoint trackJoint;

    public Rigidbody rigidbody;
    public Light light;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        light = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        if(trackHand != null)
        {
            Vector3 diff = trackHand.position - rigidbody.position;
            if (diff.magnitude > 0.5f)
                rigidbody.position = trackHand.position;
        }
    }

    public void TurnOff()
    {
        light.enabled = false;
    }

    public void TurnOn()
    {
        light.enabled = true;
    }

    public void Drop()
    {
        if(trackJoint != null)
            Destroy(trackJoint);

        trackHand = null;
        trackJoint = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralAnimation
{
    public class ArmBuilder : MonoBehaviour
    {
        [SerializeField]
        public Transform m_phyShoulder = null;

        [SerializeField]
        public GameObject m_flashlightPrefab = null;

        [SerializeField]
        int m_numArms = 2;

        [SerializeField]
        float m_armOffset = 0.5f;

        [SerializeField]
        float m_upperArmLength = 0.2f;

        [SerializeField]
        float m_upperArmRadius = 0.1f;

        [SerializeField]
        Material m_armMaterial = null;

        public ArmController armController = null;

        public GameObject[] physHands;

        public Flashlight flashlightObj = null;

        private void Awake()
        {
            /* Upper Arm */

            physHands = new GameObject[m_numArms];

            for (int i = 0; i < m_numArms; ++i)
            {
                /* Kinematic Object */

                float posAngle = ((2.0f * Mathf.PI) / m_numArms) * i;
                GameObject m_upperArmObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(m_upperArmObj.GetComponent<MeshRenderer>());
                Destroy(m_upperArmObj.GetComponent<MeshFilter>());
                Destroy(m_upperArmObj.GetComponent<Collider>());
                m_upperArmObj.transform.parent = transform;
                m_upperArmObj.transform.localPosition = new Vector3(m_armOffset * Mathf.Cos(posAngle), -m_upperArmLength, m_armOffset * Mathf.Sin(posAngle));
                m_upperArmObj.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.down);
                m_upperArmObj.transform.localScale = Vector3.one * m_upperArmRadius;
                m_upperArmObj.layer = gameObject.layer;
                m_upperArmObj.name = "Hand";

                Rigidbody armRb = m_upperArmObj.AddComponent<Rigidbody>();
                armRb.isKinematic = true;

                /* Physics Object */

                GameObject armObjectPhy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                armObjectPhy.transform.position = m_upperArmObj.transform.position;
                armObjectPhy.transform.localScale = m_upperArmRadius * Vector3.one;
                //ConfigurableJoint joint = armObjectPhy.AddComponent<ConfigurableJoint>();
                //joint.connectedBody = armRb;
                //joint.SetPdParamters(1000.0f, 10.0f, 1000.0f, 10.0f, float.MaxValue);
                VelocityController velController = armObjectPhy.AddComponent<VelocityController>();
                velController.targetTransform = armRb.transform;
                armObjectPhy.layer = gameObject.layer;
                armObjectPhy.name = "phy_Hand";
                Rigidbody rbPhy = armObjectPhy.GetComponent<Rigidbody>();
                rbPhy.position = m_upperArmObj.transform.position;
                //rbPhy.freezeRotation = true;
                physHands[i] = armObjectPhy;

                /* Create Shoulder Attach Point */
                GameObject shoulderAttachObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(shoulderAttachObj.GetComponent<Collider>());
                shoulderAttachObj.transform.parent = m_phyShoulder.transform;
                shoulderAttachObj.transform.position = m_phyShoulder.transform.position + new Vector3(m_armOffset * Mathf.Cos(posAngle), 0.0f, m_armOffset * Mathf.Sin(posAngle));
                shoulderAttachObj.transform.localScale = m_upperArmRadius * Vector3.one;
                shoulderAttachObj.layer = gameObject.layer;
                shoulderAttachObj.name = "Shoulder";

                /* Build Arm Connection */

                WalkLeg armJoint = WalkLeg.Build(transform, shoulderAttachObj.transform, armObjectPhy.transform, m_upperArmRadius * 0.8f, m_armMaterial);
                armJoint.gameObject.layer = gameObject.layer;
                armJoint.name = "Leg";

                /* Add Control Arm */

                if(i == 1)
                {
                    armController = m_upperArmObj.AddComponent<ArmController>();
                    ArmCollider armCollider = armObjectPhy.AddComponent<ArmCollider>();
                    armController.m_armCollider = armCollider;

                    armController.m_armMaterial = armJoint.GetComponent<MeshRenderer>().material;
                    armController.m_handMaterial = armObjectPhy.GetComponent<MeshRenderer>().material;
                }
                else if(i == 0)
                {
                    /* Add Flashlight Arm */

                    GameObject flashLight = GameObject.Instantiate(m_flashlightPrefab);
                    //flashLight.transform.position = physHands[0].transform.position;
                    //VelocityController vc = flashLight.AddComponent<VelocityController>();
                    //vc.targetTransform = physHands[0].transform;
                    //Rigidbody rb = flashLight.GetComponent<Rigidbody>();
                    //rb.freezeRotation = true;
                    Rigidbody flashLightRb = flashLight.GetComponent<Rigidbody>();
                    //flashLightRb.position = rbPhy.position;

                    ConfigurableJoint flashJoint = physHands[0].AddComponent<ConfigurableJoint>();
                    flashJoint.SetAllJointMotions(ConfigurableJointMotion.Locked);
                    flashJoint.autoConfigureConnectedAnchor = false;
                    flashJoint.connectedBody = flashLightRb;
                    flashJoint.anchor = Vector3.zero;
                    flashJoint.connectedAnchor = Vector3.zero;
                    flashJoint.connectedMassScale = 100.0f;

                    flashlightObj = flashLight.GetComponent<Flashlight>();
                    flashlightObj.trackHand = physHands[0].transform;
                    flashlightObj.trackJoint = flashJoint;
                }

            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

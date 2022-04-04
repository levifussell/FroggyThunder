using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtils;
using MathUtils.Editor;

public class ArmController : MonoBehaviour
{
    [SerializeField]
    float m_speed = 5.0f;

    [SerializeField]
    float m_maxDistance = 4.0f;

    [SerializeField]
    public ArmCollider m_armCollider = null;

    public Material m_handMaterial = null;
    public Material m_armMaterial = null;

    Color m_baseArmColor;
    //Color m_selectColor = new Color(190.0f / 255.0f, 242.0f / 255.0f, 60.0f / 255.0f);
    Color m_selectColor = new Color(255.0f / 255.0f, 232.0f / 255.0f, 60.0f / 255.0f);

    Transform m_originParent;
    Vector3 m_originArmPosLocal;
    Vector3 originArmPosGlobal { get => m_originParent.TransformPoint(m_originArmPosLocal); }

    BezierCurve3 m_curveToTarget;
    float m_curveTimer;

    GameObject m_cameraFollowIndicator;

    int m_noPlayerMask;

    private void Awake()
    {
        m_cameraFollowIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(m_cameraFollowIndicator.GetComponent<Collider>());
        m_cameraFollowIndicator.transform.localScale = 0.1f * Vector3.one;

        m_originParent = transform.parent;
        m_originArmPosLocal = transform.localPosition;

        m_noPlayerMask = ~LayerMask.GetMask("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_baseArmColor = m_armMaterial.color;

        m_armCollider.onCollide += LaunchReturnArm;
        GoToNewTargetGlobal(transform.position);
    }

    private void OnDestroy()
    {
        m_armCollider.onCollide -= LaunchReturnArm;
        Destroy(m_armCollider);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (m_armCollider.isGrabbing)
            {
                m_armCollider.DropGrabbedObject();
                m_armMaterial.color = m_baseArmColor;
                m_handMaterial.color = m_baseArmColor;
            }
            else
            {
                Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(clickRay, out RaycastHit hit, 100.0f, m_noPlayerMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3 hitDiff = hit.point - originArmPosGlobal;
                    Vector3 targetPos = originArmPosGlobal + hitDiff.normalized * Mathf.Min(hitDiff.magnitude, m_maxDistance);
                    GoToNewTargetGlobal(targetPos);
                    m_cameraFollowIndicator.transform.position = targetPos;

                    m_armCollider.detectGrabbing = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        E_BezierCurves.EditorSceneDrawBezierCurve3(m_curveToTarget);
#endif
    }

    #region custom methods
    void GoToNewTargetGlobal(Vector3 pos)
    {
        transform.parent = null;
        Vector3 midPoint = transform.position + (pos - transform.position) * 0.5f;
        m_curveToTarget = new BezierCurve3(pos, midPoint, transform.position);
        m_curveTimer = 0.0f;

        StopAllCoroutines();
        StartCoroutine(GoToTarget());
    }

    IEnumerator GoToTarget()
    {
        m_armMaterial.color = m_selectColor;
        m_handMaterial.color = m_selectColor;

        while(m_curveTimer < 1.0f)
        {
            m_curveTimer = Mathf.Clamp01(m_curveTimer + m_speed * Time.deltaTime);
            transform.position = m_curveToTarget.Evaluate(m_curveTimer);
            yield return new WaitForFixedUpdate();
        }

        Vector3 diff;
        do
        {
            diff = transform.position - m_armCollider.transform.position;
            yield return new WaitForFixedUpdate();
        } while (diff.sqrMagnitude > 1e-1f);

        StartCoroutine(ReturnToArm());
    }

    void LaunchReturnArm()
    {
        m_cameraFollowIndicator.transform.position = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(ReturnToArm());
    }


    IEnumerator ReturnToArm()
    {
        yield return new WaitForSeconds(0.5f);

        if(!m_armCollider.isGrabbing)
        {
            m_armMaterial.color = m_baseArmColor;
            m_handMaterial.color = m_baseArmColor;
        }

        m_armCollider.detectGrabbing = false;

        transform.parent = m_originParent;
        Vector3 midPoint = transform.localPosition + (m_originArmPosLocal - transform.localPosition) * 0.5f;
        m_curveToTarget = new BezierCurve3(m_originArmPosLocal, midPoint, transform.localPosition);
        m_curveTimer = 0.0f;

        while(m_curveTimer < 1.0f)
        {
            m_curveTimer = Mathf.Clamp01(m_curveTimer + m_speed * Time.deltaTime);
            transform.localPosition = m_curveToTarget.Evaluate(m_curveTimer);
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}

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
    float m_maxDistance = 1.0f;

    [SerializeField]
    public ArmCollider m_armCollider = null;

    Transform m_originParent;
    Vector3 m_originArmPosLocal;
    Vector3 originArmPosGlobal { get => m_originParent.TransformPoint(m_originArmPosLocal); }

    BezierCurve3 m_curveToTarget;
    float m_curveTimer;

    GameObject m_cameraFollowIndicator;

    private void Awake()
    {
        m_cameraFollowIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(m_cameraFollowIndicator.GetComponent<Collider>());
        m_cameraFollowIndicator.transform.localScale = 0.1f * Vector3.one;

        m_originParent = transform.parent;
        m_originArmPosLocal = transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_armCollider.onCollide += LaunchReturnArm;
        GoToNewTargetGlobal(transform.position);
    }

    private void OnDestroy()
    {
        m_armCollider.onCollide -= LaunchReturnArm;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(clickRay, out RaycastHit hit, 100.0f, ~0, QueryTriggerInteraction.Collide))
            {
                Vector3 hitDiff = hit.point - originArmPosGlobal;
                Vector3 targetPos = originArmPosGlobal + hitDiff.normalized * Mathf.Min(hitDiff.magnitude, m_maxDistance);
                GoToNewTargetGlobal(targetPos);
                m_cameraFollowIndicator.transform.position = targetPos;
            }
        }
    }

    private void OnDrawGizmos()
    {
        E_BezierCurves.EditorSceneDrawBezierCurve3(m_curveToTarget);
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
        StopAllCoroutines();
        StartCoroutine(ReturnToArm());
    }


    IEnumerator ReturnToArm()
    {
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

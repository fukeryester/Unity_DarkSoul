using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    
    public float horizentalSpeed = 100f;
    public float verticalSpeed = 100f;
    public bool DynamicBlur = true;
    public Image lockDot;
    public bool lockState;
    public bool isAI = false;

    private GameObject CameraHandle;
    private GameObject PlayerHandle;
    private GameObject model;
    private IUserInput PI;
    private GameObject mainCamera;
    [SerializeField]
    private LockTarget lockTarget;
    private float tempEulerX;

    // Start is called before the first frame update
    void Start()
    {
        CameraHandle = transform.parent.gameObject;
        PlayerHandle = CameraHandle.transform.parent.gameObject;
        tempEulerX = 20f;
        ActorController ac = PlayerHandle.GetComponent<ActorController>();
        model = ac.model;
        PI = ac.PI;

        if (!isAI)
        {
            mainCamera = Camera.main.gameObject;
            lockDot.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        lockState = false; 
    }

    private void Update()
    {
        if (lockTarget != null)
        {
            lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            if(Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10f)
            {
                lockTargetProcess(null, false, false, isAI);
            }
        }
    }

    void FixedUpdate()
    {
        //非锁定模式
        if (lockTarget == null)
        {
            var tempModelEuler = model.transform.eulerAngles;
            //水平旋转
            PlayerHandle.transform.Rotate(Vector3.up, horizentalSpeed * PI.CRight * Time.fixedDeltaTime);

            //垂直旋转
            tempEulerX -= verticalSpeed * PI.CUp * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40f, 60f);
            CameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            //模型朝向不随镜头水平旋转变动
            model.transform.eulerAngles = tempModelEuler;
        }

        //锁定模式
        else
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            PlayerHandle.transform.forward = tempForward;
            CameraHandle.transform.LookAt(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight / 2, 0));
        }

        //惰性相机追踪
        if (!isAI)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position, DynamicBlur ? 0.1f : 0.3f);
            mainCamera.transform.LookAt(CameraHandle.transform);
        }
    }

    public void ToggleLock()
    {
        //锁定
        if (lockTarget == null)
        {
            Vector3 LockStart = model.transform.position;
            Vector3 BoxCenter = LockStart + PlayerHandle.transform.forward * 4f;
            Collider[] cols = Physics.OverlapBox(BoxCenter, new Vector3(2f, 2f, 5f), PlayerHandle.transform.rotation, LayerMask.GetMask(isAI ? "Player" : "Enemy"));
            //DrawBox(BoxCenter, new Vector3(2f, 2f, 5f), PlayerHandle.transform.rotation, Color.red, 3f);

            if (cols.Length == 0)
            {
                lockTargetProcess(null, false, false, isAI);
            }
                
            foreach (Collider col in cols)
            {
                lockTargetProcess(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }

        //解锁
        else
        {
            lockTargetProcess(null, false, false, isAI);
        }
    }

    private void lockTargetProcess(LockTarget _lockTarget, bool _lockDotEnabled, bool _lockState, bool isAI)
    {
        lockTarget = _lockTarget;
        lockState = _lockState;
        if (!isAI)
        {
            lockDot.enabled = _lockDotEnabled;
        }
    }

    private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;
        public LockTarget(GameObject obj, float halfHeight)
        {
            this.obj = obj;
            this.halfHeight = halfHeight;
        }
    }

    void DrawBox(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration = 2f)
    {
        Vector3[] vertices = new Vector3[8];
        Vector3 halfSize = size / 2;

        vertices[0] = center + rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        vertices[1] = center + rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        vertices[2] = center + rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        vertices[3] = center + rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z);
        vertices[4] = center + rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        vertices[5] = center + rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        vertices[6] = center + rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        vertices[7] = center + rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z);

        Debug.DrawLine(vertices[0], vertices[1], color, duration);
        Debug.DrawLine(vertices[0], vertices[2], color, duration);
        Debug.DrawLine(vertices[1], vertices[3], color, duration);
        Debug.DrawLine(vertices[2], vertices[3], color, duration);

        Debug.DrawLine(vertices[4], vertices[5], color, duration);
        Debug.DrawLine(vertices[4], vertices[6], color, duration);
        Debug.DrawLine(vertices[5], vertices[7], color, duration);
        Debug.DrawLine(vertices[6], vertices[7], color, duration);

        Debug.DrawLine(vertices[0], vertices[4], color, duration);
        Debug.DrawLine(vertices[1], vertices[5], color, duration);
        Debug.DrawLine(vertices[2], vertices[6], color, duration);
        Debug.DrawLine(vertices[3], vertices[7], color, duration);
    }
}
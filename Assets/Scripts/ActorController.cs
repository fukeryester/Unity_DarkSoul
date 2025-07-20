using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    // 获取组件
    [Header("===== 获取组件 =====")]
    [SerializeField]
    private Animator anim;
    public GameObject model;
    public IUserInput PI;
    private Rigidbody rb;
    private CapsuleCollider PlayerCapsule;
    public CameraController cameraController;

    // 移动控制量
    [Header("===== 移动控制量 =====")]
    public float WalkSpeed = 2.4f;
    public float RunMultiplier = 2.0f;
    public float JumpSpeed = 3.0f;
    public float RollForwardSpeed = 1.0f;
    public float RollUpSpeed = 1.0f;
    public float JabMultiplier = 3f;
    public float Fall_To_Roll_Y_Speed = 3.5f;
    private Vector3 ThrustVec;

    // 攻击滑动量
    [Header("===== 攻击滑动量/根运动控制 =====")]
    //public float AttackRollMultiplier = 0.01f;
    private float AttackRollMultipliertemp;
    public float AttackForwardForce = 0.4f;
    public float AttackMovMultiplier = 0.3f;
    private Vector3 deltaPos;
    private bool canAttack = true;
    public bool leftIsShield = true;

    // 摩擦力
    [Header("===== 摩擦力 =====")]
    public PhysicMaterial frictionDefault;
    public PhysicMaterial frictionZero;

    // 其他
    [Header("===== 其他 =====")]
    [SerializeField]
    private Vector3 moveVec;
    private bool moveVec_locker = false;
    private bool trackDirection = false;
    private float temp_Dmag;

      
    // 加载实例时获取组件
    void Awake()
    {
        IUserInput[] userInputs = GetComponents<IUserInput>();
        foreach (IUserInput user_Input in userInputs)
        {
            if(user_Input.enabled == true)
            {
                PI = user_Input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        PlayerCapsule = GetComponent<CapsuleCollider>();
        cameraController = GetComponentInChildren<CameraController>();
    }

    // Update处理动画、朝向、移动量
    void Update()
    {
        // 镜头锁定控制
        if (PI.lockon)
        {
            cameraController.ToggleLock();
        }

        // 动画控制（走跑跳滚打挡）

        // 非锁定走跑
        if(cameraController.lockState == false)
        {
            anim.SetFloat("Forward", PI.Dmag * Mathf.Lerp(anim.GetFloat("Forward"), (PI.run ? 2 : 1.2f), 0.08f));
            anim.SetFloat("Right", 0);
        }
        // 锁定走跑
        else
        {
            // 世界坐标系转换回模型坐标系
            Vector3 localDvec = transform.InverseTransformVector(PI.Dvec);
            anim.SetFloat("Forward", localDvec.z * (PI.run ? 2 : 1.2f));
            anim.SetFloat("Right", localDvec.x * (PI.run ? 2 : 1.2f));
        }
        
        // 跳
        if (PI.jump)
        {
            anim.SetTrigger("Jump");
        }

        // 高处落下翻滚
        if(rb.velocity.y > Fall_To_Roll_Y_Speed)
        {
            anim.SetTrigger("Roll");
        }

        // 手动翻滚
        if (PI.roll)
        {
            if (Mathf.Abs(PI.TargetDup) > 0 || Mathf.Abs(PI.TargetDright) > 0)
            {
                anim.SetTrigger("Roll");
            }
            else
            {
                anim.SetTrigger("Jab");
            }
        }

        // 左右手轻攻击
        if ((PI.rb || PI.lb) && canAttack)
        {
            // 右手
            if (PI.rb)
            {
                anim.SetBool("mirror", false);
                anim.SetTrigger("Attack");
            }
            // 左手
            else if (PI.lb && !leftIsShield)
            {
                anim.SetBool("mirror", true);
                anim.SetTrigger("Attack");
            }
        }

        // 左右手重攻击
        if ((PI.rt || PI.lt) && canAttack)
        {
            // 右手重攻击
            if (PI.rt)
            {
                anim.SetBool("mirror", false);
                anim.SetTrigger("Attack");
            }
            // 左手
            else
            {
                if(!leftIsShield)
                {

                }
                else
                {
                    anim.SetTrigger("counterBack");
                }
            }
        }

        // 防御
        if ((CheckState("Ground") || CheckState("Blocked")) && leftIsShield && canAttack)
        {
            anim.SetBool("Defence", PI.defence);
            if (PI.defence) anim.SetLayerWeight(anim.GetLayerIndex("Defence"), 1);
            else anim.SetLayerWeight(anim.GetLayerIndex("Defence"), 0);
        }
        else
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Defence"), 0);
        }
        
        // 未锁定的朝向与移动控制
        if(cameraController.lockState == false)
        {
            if (PI.Dmag > 0.01f)
            {
                model.transform.forward = Vector3.Slerp(model.transform.forward, PI.Dvec, 0.2f);
            }
            if (moveVec_locker == false)
            {
                // 移动控制
                moveVec = ((PI.run) ? RunMultiplier : 1f) * PI.Dmag * WalkSpeed * model.transform.forward;
            }
        }

        // 锁定的朝向与移动控制
        else
        {
            // 锁定状态下非跳滚
            if (trackDirection == false)
            {
                model.transform.forward = Vector3.Slerp(model.transform.forward, transform.forward, 0.2f);
            }
            // 锁定状态下跳滚
            else
            {
                moveVec = moveVec * 0.999f + transform.forward * 0.001f;
                model.transform.forward = Vector3.Slerp(model.transform.forward, moveVec.normalized, 0.2f);
            }
            if (moveVec_locker == false)
            {
                moveVec = PI.Dvec * ((PI.run) ? RunMultiplier : 1f) * WalkSpeed;
            } 
        }
    }

    // FiexedUpdate处理运动
    void FixedUpdate()
    {
        //刚体赋予根运动移动量
        rb.position += deltaPos;

        //刚体速度赋值moveVec移动量以及(翻滚/后跳/攻击)冲量
        rb.velocity = new Vector3(moveVec.x, rb.velocity.y, moveVec.z) + ThrustVec;
        ThrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }


    /// 
    /// 实用函数
    /// 

    //检查当前状态是否是输入状态
    public bool CheckState(string StateName, string LayerName = "Base Layer")
    {
        int LayerIndex = anim.GetLayerIndex(LayerName);
        var StateInfo = anim.GetCurrentAnimatorStateInfo(LayerIndex);
        bool result = StateInfo.IsName(StateName);
        return result;
    }

    public bool CheckStateTag(string tagName, string LayerName = "Base Layer")
    {
        int LayerIndex = anim.GetLayerIndex(LayerName);
        var StateInfo = anim.GetCurrentAnimatorStateInfo(LayerIndex);
        bool result = StateInfo.IsTag(tagName);
        return result;
    }


    /// 
    /// 动画信号处理
    /// 

    // 跳跃信号处理
    public void OnJumpEnter()
    {
        ThrustVec = new Vector3(0, JumpSpeed, 0);
        trackDirection = true;
    }

    // 翻滚信号处理
    public void OnRollEnter()
    {
        if (cameraController.lockState == false)
        {
            moveVec = ((PI.run) ? RunMultiplier : 1f) * WalkSpeed * model.transform.forward;
        }
           
        moveVec_locker = true;
        trackDirection = true;
        PI.inputEnabled = false;
        canAttack = false;
        temp_Dmag = PI.Dmag;
        ThrustVec = new Vector3(0, RollUpSpeed, 0);
    }

    public void OnRollUpdate()
    {
        if (cameraController.lockState == false)
        {
            ThrustVec = model.transform.forward * RollForwardSpeed;
        }
        else
        {
            ThrustVec = moveVec  * RollForwardSpeed * 0.3f;
        }
    }


    public void OnRollExit()
    {
        moveVec_locker = false;
        canAttack = true;
        PI.Dmag = temp_Dmag;
    }


    // 后跳信号处理
    public void OnJabEnter()
    {
        moveVec_locker = true;
        PI.inputEnabled = false;
        canAttack = false;
        temp_Dmag = PI.Dmag;
    }

    public void OnJabUpdate()
    {
        ThrustVec = model.transform.forward * anim.GetFloat("jabVelocity") * JabMultiplier;
    }

    public void OnJabExit()
    {
        moveVec_locker = false;
        canAttack = true;
        PI.Dmag = temp_Dmag;
    }

    // 着陆/离地、进入地面/离开地面信号处理
    public void isOnGround()
    {
        anim.SetBool("isGround", true);
    }

    public void isNotOnGround()
    {
        anim.SetBool("isGround", false);
    }
    public void OnGroundEnter()
    {
        PlayerCapsule.material = frictionDefault;
        trackDirection = false;
        PI.inputEnabled = true;
    }
    public void OnGroundExit()
    {
        PlayerCapsule.material = frictionZero;
    }

    // 攻击信号处理
    public void OnAttack1hEnter()
    {
        temp_Dmag = PI.Dmag;
        PI.inputEnabled = false;
    }

    public void OnAttack1hUpdate()
    {
        moveVec_locker = true;
        moveVec = AttackMovMultiplier * PI.Dmag * WalkSpeed * model.transform.forward;
        ThrustVec = model.transform.forward * AttackForwardForce;
    }

    public void OnAttack1hExit()
    {
        moveVec_locker = false;
        PI.Dmag = temp_Dmag;
        PI.inputEnabled = true;
    }

    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnHitEnter()
    {
        PI.inputEnabled = false;
        moveVec = new Vector3(0, moveVec.y, 0);
    }

    public void OnDieEnter()
    {
        PI.inputEnabled = false;
        moveVec = new Vector3(0, moveVec.y, 0);
    }

    public void OnBlockedEnter()
    {
        PI.inputEnabled = false;
    }

    public void OnStunnedEnter()
    {
        PI.inputEnabled = false;
        moveVec = new Vector3(0, moveVec.y, 0);
    }

    public void OnCounterBackEnter()
    {
        PI.inputEnabled = false;
        moveVec = new Vector3(0, moveVec.y, 0);
    }

    public void UpdateRootMotion(object Input_deltaPos)
    {
        //物理引擎未更新时累加根运动移动量
        if(CheckStateTag("attackR") || CheckStateTag("attackL"))
        {
            deltaPos += (Vector3)Input_deltaPos;
        }
    }

    public void IssueTrigger(string  Trigger_Name, Collider hit_weapon = null)
    {
        anim.SetTrigger(Trigger_Name);
        if(hit_weapon != null )
        {
            ThrustVec = hit_weapon.transform.forward * 2f;
        }
    }
}

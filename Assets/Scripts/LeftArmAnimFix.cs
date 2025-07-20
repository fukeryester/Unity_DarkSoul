using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    private Animator anim;
    private ActorController ac;
    public Vector3 RotateAngle;

    void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }

    void OnAnimatorIK()
    {
        if (ac.leftIsShield)
        {
            if (anim.GetBool("Defence") == false)
            {
                // 获取左下臂的骨骼变换
                Transform LeftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);

                // 获取当前的旋转四元数
                Quaternion currentRotation = LeftLowerArm.rotation;

                // 将旋转角度转换为四元数
                Quaternion additionalRotation = Quaternion.Euler(RotateAngle);

                // 通过乘法合成旋转
                LeftLowerArm.rotation = currentRotation * additionalRotation;

                // 将旋转应用到骨骼
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, LeftLowerArm.localRotation);
            }
        }   
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class DynamicRagdollCreator : MonoBehaviour
{
    public void CreateRagdoll(GameObject npc)
    {
        Animator animator = npc.GetComponent<Animator>();
        if (animator == null || !animator.isHuman || animator.avatar == null)
        {
            Debug.LogWarning("NPC does not have a valid humanoid animator.");
            return;
        }

        AddRagdollComponents(animator, HumanBodyBones.Head);
        AddRagdollComponents(animator, HumanBodyBones.Chest);
        AddRagdollComponents(animator, HumanBodyBones.Spine);
        AddRagdollComponents(animator, HumanBodyBones.Hips);
        AddRagdollComponents(animator, HumanBodyBones.LeftUpperArm);
        AddRagdollComponents(animator, HumanBodyBones.LeftLowerArm);
        AddRagdollComponents(animator, HumanBodyBones.RightUpperArm);
        AddRagdollComponents(animator, HumanBodyBones.RightLowerArm);
        AddRagdollComponents(animator, HumanBodyBones.LeftUpperLeg);
        AddRagdollComponents(animator, HumanBodyBones.LeftLowerLeg);
        AddRagdollComponents(animator, HumanBodyBones.RightUpperLeg);
        AddRagdollComponents(animator, HumanBodyBones.RightLowerLeg);

        ConnectBones(animator, HumanBodyBones.Head, HumanBodyBones.Chest);
        ConnectBones(animator, HumanBodyBones.Chest, HumanBodyBones.Spine);
        ConnectBones(animator, HumanBodyBones.Spine, HumanBodyBones.Hips);
        ConnectBones(animator, HumanBodyBones.LeftUpperLeg, HumanBodyBones.Hips);
        ConnectBones(animator, HumanBodyBones.RightUpperLeg, HumanBodyBones.Hips);
        ConnectBones(animator, HumanBodyBones.LeftUpperArm, HumanBodyBones.Chest);
        ConnectBones(animator, HumanBodyBones.RightUpperArm, HumanBodyBones.Chest);
    }

    private void AddRagdollComponents(Animator animator, HumanBodyBones bone)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform != null)
        {
            Rigidbody rb = boneTransform.gameObject.AddComponent<Rigidbody>();
            rb.mass = GetBoneMass(bone);

            if (bone == HumanBodyBones.Spine || bone == HumanBodyBones.Chest || bone == HumanBodyBones.Hips)
            {
                BoxCollider collider = boneTransform.gameObject.AddComponent<BoxCollider>();
                ConfigureBoxCollider(collider, boneTransform, bone);
            }
            else
            {
                CapsuleCollider collider = boneTransform.gameObject.AddComponent<CapsuleCollider>();
                ConfigureCapsuleCollider(collider, boneTransform, bone);
            }

            if (boneTransform.parent != null)
            {
                Rigidbody parentRb = boneTransform.parent.GetComponent<Rigidbody>();
                if (parentRb == null)
                {
                    parentRb = boneTransform.parent.GetComponentInParent<Rigidbody>();
                }

                if (parentRb != null)
                {
                    CharacterJoint joint = boneTransform.gameObject.AddComponent<CharacterJoint>();
                    joint.connectedBody = parentRb;
                    ConfigureJoint(joint, bone);
                }
            }
        }
    }

    private void ConfigureCapsuleCollider(CapsuleCollider collider, Transform boneTransform, HumanBodyBones bone)
    {
        if (collider != null)
        {
            collider.direction = 1; // Axe Y
            switch (bone)
            {
                case HumanBodyBones.Head:
                    collider.height = 0.2f;
                    collider.radius = 0.1f;
                    collider.center = new Vector3(0, 0.1f, 0);
                    break;
                case HumanBodyBones.LeftUpperArm:
                case HumanBodyBones.RightUpperArm:
                    collider.height = 0.3f;
                    collider.radius = 0.05f;
                    collider.center = new Vector3(0, 0.15f, 0);
                    break;
                case HumanBodyBones.LeftLowerArm:
                case HumanBodyBones.RightLowerArm:
                    collider.height = 0.25f;
                    collider.radius = 0.05f;
                    collider.center = new Vector3(0, 0.125f, 0);
                    break;
                case HumanBodyBones.LeftUpperLeg:
                case HumanBodyBones.RightUpperLeg:
                    collider.height = 0.4f;
                    collider.radius = 0.07f;
                    collider.center = new Vector3(0, 0.2f, 0);
                    break;
                case HumanBodyBones.LeftLowerLeg:
                case HumanBodyBones.RightLowerLeg:
                    collider.height = 0.35f;
                    collider.radius = 0.07f;
                    collider.center = new Vector3(0, 0.175f, 0);
                    break;
            }
        }
    }

    private void ConfigureBoxCollider(BoxCollider collider, Transform boneTransform, HumanBodyBones bone)
    {
        if (collider != null)
        {
            switch (bone)
            {
                case HumanBodyBones.Chest:
                    collider.size = new Vector3(0.2f, 0.5f, 0.2f);
                    collider.center = new Vector3(0, 0.25f, 0);
                    break;
                case HumanBodyBones.Spine:
                    collider.size = new Vector3(0.2f, 0.4f, 0.2f);
                    collider.center = new Vector3(0, 0.2f, 0);
                    break;
                case HumanBodyBones.Hips:
                    collider.size = new Vector3(0.3f, 0.2f, 0.2f);
                    collider.center = new Vector3(0, 0.1f, 0);
                    break;
            }
        }
    }

    private void ConfigureJoint(CharacterJoint joint, HumanBodyBones bone)
    {
        joint.axis = new Vector3(1, 0, 0);
        joint.swingAxis = new Vector3(0, 1, 0);
        joint.enableProjection = true; 
        SoftJointLimit limit = new SoftJointLimit();

        switch (bone)
        {
            case HumanBodyBones.Head:
                limit.limit = 0.1f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                break;
            case HumanBodyBones.Chest:
            case HumanBodyBones.Spine:
            case HumanBodyBones.Hips:
                limit.limit = 0.2f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                break;
            case HumanBodyBones.LeftUpperArm:
            case HumanBodyBones.RightUpperArm:
                limit.limit = 0.3f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                break;
            case HumanBodyBones.LeftLowerArm:
            case HumanBodyBones.RightLowerArm:
                limit.limit = 0.2f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                break;
            case HumanBodyBones.LeftUpperLeg:
            case HumanBodyBones.RightUpperLeg:
                limit.limit = 0.4f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                break;
            case HumanBodyBones.LeftLowerLeg:
            case HumanBodyBones.RightLowerLeg:
                limit.limit = 0.2f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                break;
        }
    }

    private void ConnectBones(Animator animator, HumanBodyBones boneA, HumanBodyBones boneB)
    {
        Transform boneATransform = animator.GetBoneTransform(boneA);
        Transform boneBTransform = animator.GetBoneTransform(boneB);

        if (boneATransform != null && boneBTransform != null)
        {
            CharacterJoint joint = boneATransform.gameObject.AddComponent<CharacterJoint>();
            Rigidbody connectedBody = boneBTransform.GetComponent<Rigidbody>();
            if (connectedBody != null)
            {
                joint.connectedBody = connectedBody;
                ConfigureJoint(joint, boneA);
            }
        }
    }

    private float GetBoneMass(HumanBodyBones bone)
    {
        switch (bone)
        {
            case HumanBodyBones.Head:
                return 1f;
            case HumanBodyBones.Spine:
            case HumanBodyBones.Chest:
                return 5f;
            case HumanBodyBones.Hips:
                return 7f;
            case HumanBodyBones.LeftUpperArm:
            case HumanBodyBones.RightUpperArm:
                return 2f;
            case HumanBodyBones.LeftLowerArm:
            case HumanBodyBones.RightLowerArm:
                return 1.5f;
            case HumanBodyBones.LeftUpperLeg:
            case HumanBodyBones.RightUpperLeg:
                return 3f;
            case HumanBodyBones.LeftLowerLeg:
            case HumanBodyBones.RightLowerLeg:
                return 2.5f;
            default:
                return 1f;
        }
    }
}
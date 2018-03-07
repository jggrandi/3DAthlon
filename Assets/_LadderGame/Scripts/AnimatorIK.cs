using UnityEngine;
using System.Collections;

namespace Contest3DUI
{

    /// <summary>
    /// built-in Animator IK control
    /// </summary>
    public class AnimatorIK : MonoBehaviour
    {

        [Header("References")]
        public Animator animator;

        [Header("Root")]
        public Transform rootTransform;
        public Transform controlledAvatarTransform;
        public Transform rootTarget;
        //[Range(0f, 1f)] public float rootPositionWeight = 0f;
        //[Range(0f, 1f)] public float rootRotationWeight = 0f;


        // Look At
        [Header("Look At")]
        //public Transform lookAtTargetBiped;
        public Transform lookAtTarget;
        [Range(0f, 1f)] public float lookAtWeight = 1f;
        [Range(0f, 1f)] public float lookAtBodyWeight = 1f;
        [Range(0f, 1f)] public float lookAtHeadWeight = 1f;
        [Range(0f, 1f)] public float lookAtEyesWeight = 1f;
        [Range(0f, 1f)] public float lookAtClampWeight = 0.5f;
        [Range(0f, 1f)] public float lookAtClampWeightHead = 0.5f;
        [Range(0f, 1f)] public float lookAtClampWeightEyes = 0.5f;

        // Feet
        [Header("Left Foot")]
        public Transform leftFootTarget;
        [Range(0f, 1f)] public float leftFootPositionWeight = 0f;
        [Range(0f, 1f)] public float leftFootRotationWeight = 0f;
        [Header("Right Foot")]
        public Transform rightFootTarget;
        [Range(0f, 1f)] public float rightFootPositionWeight = 0f;
        [Range(0f, 1f)] public float rightFootRotationWeight = 0f;

        // Hands
        [Header("Left Hand")]
        public Transform leftHandTarget;
        [Range(0f, 1f)] public float leftHandPositionWeight = 0f;
        [Range(0f, 1f)] public float leftHandRotationWeight = 0f;
        [Header("Right Hand")]
        public Transform rightHandTarget;
        [Range(0f, 1f)] public float rightHandPositionWeight = 0f;
        [Range(0f, 1f)] public float rightHandRotationWeight = 0f;

        private void Start()
        {
            rootTarget.localPosition += controlledAvatarTransform.position - rootTransform.position;
        }

        void OnAnimatorIK(int layer)
        {


            //animator.transform.rotation = bipedIK.transform.rotation;
            //Vector3 offset = animator.transform.position - bipedIK.transform.position;

            // Look At
            //lookAtTarget.position = lookAtTargetBiped.position + offset;

            //bipedIK.SetLookAtPosition(lookAtTargetBiped.position);
            //bipedIK.SetLookAtWeight(lookAtWeight, lookAtBodyWeight, lookAtHeadWeight, lookAtEyesWeight, lookAtClampWeight, lookAtClampWeightHead, lookAtClampWeightEyes);

            animator.SetLookAtPosition(lookAtTarget.position);
            animator.SetLookAtWeight(lookAtWeight, lookAtBodyWeight, lookAtHeadWeight, lookAtEyesWeight, lookAtClampWeight);

            // Foot
            //footTarget.position = footTargetBiped.position + offset;
            //footTarget.rotation = footTargetBiped.rotation;

            //bipedIK.SetIKPosition(AvatarIKGoal.LeftFoot, footTargetBiped.position);
            //bipedIK.SetIKRotation(AvatarIKGoal.LeftFoot, footTargetBiped.rotation);
            //bipedIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footPositionWeight);
            //bipedIK.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footRotationWeight);

            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);

            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootRotationWeight);

            // Hand
            //handTarget.position = handTargetBiped.position + offset;
            //handTarget.rotation = handTargetBiped.rotation;

            //bipedIK.SetIKPosition(AvatarIKGoal.LeftHand, handTargetBiped.position);
            //bipedIK.SetIKRotation(AvatarIKGoal.LeftHand, handTargetBiped.rotation);
            //bipedIK.SetIKPositionWeight(AvatarIKGoal.LeftHand, handPositionWeight);
            //bipedIK.SetIKRotationWeight(AvatarIKGoal.LeftHand, handRotationWeight);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandRotationWeight);

            controlledAvatarTransform.position = rootTarget.position;
            controlledAvatarTransform.rotation = rootTarget.rotation;
        }
    }
}

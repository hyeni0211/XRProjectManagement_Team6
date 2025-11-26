using System;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [Serializable, CreateAssetMenu(fileName = "Override Animation", menuName = "Viven/Create Override Animation")]
    public class SDKOverrideAnimation : ScriptableObject
    {
        public AnimationClip Idle;

        // sit
        public AnimationClip Sitting_Idle;

        // turn
        public AnimationClip Turn_Left;
        public AnimationClip Turn_Right;

        // jump
        public AnimationClip Jump;

        // walk start
        public AnimationClip Walk_Start_Backward;
        public AnimationClip Walk_Start_Forward;
        public AnimationClip Walk_Start_Left;
        public AnimationClip Walk_Start_Right;

        // walk
        public AnimationClip Walk_Backward;
        public AnimationClip Walk_Backward_Left;
        public AnimationClip Walk_Backward_Right;
        public AnimationClip Walk_Forward;
        public AnimationClip Walk_Forward_Left;
        public AnimationClip Walk_Forward_Right;
        public AnimationClip Walk_Left;
        public AnimationClip Walk_Right;

        // walk jump
        public AnimationClip Walk_Jump_Backward;
        public AnimationClip Walk_Jump_Forward;
        public AnimationClip Walk_Jump_Left;
        public AnimationClip Walk_Jump_Right;

        // run
        public AnimationClip Run_Forward;
        public AnimationClip Run_Forward_Left;
        public AnimationClip Run_Forward_Right;
        public AnimationClip Run_Backward;
        public AnimationClip Run_Backward_Left;
        public AnimationClip Run_Backward_Right;
        public AnimationClip Run_Left;
        public AnimationClip Run_Right;

        // run jump
        public AnimationClip Run_Jump_Forward;
        public AnimationClip Run_Jump_Left;
        public AnimationClip Run_Jump_Right;

        // hand
        public AnimationClip Left_Grab_And_Trigger;
        public AnimationClip Left_Grab;
        public AnimationClip Left_Handle;
        public AnimationClip Left_Point;
        public AnimationClip Left_Rock;
        public AnimationClip Left_Thumb_Down;
        public AnimationClip Right_Grab_And_Trigger;
        public AnimationClip Right_Grab;
        public AnimationClip Right_Handle;
        public AnimationClip Right_Point;
        public AnimationClip Right_Rock;
        public AnimationClip Right_Thumb_Down;
    }
}
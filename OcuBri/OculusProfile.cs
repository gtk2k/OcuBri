using Ovr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcuBri
{
    class OculusProfile
    {
        public string cmd = "p";
        public string hmdType;
        //public string User;
        //public string Name;
        //public string Gender;
        public float playerHeight;
        public float eyeHeight;
        public float ipd;
        public float[] neckToEyeDistance;
        public byte eyeReliefDial;
        public float[] eyeToNoseDistance;
        public float[] maxEyeToPlateDistanse;
        public char eyeCup;
        public bool customEyeRender;
        public float[] cameraPosition;
        public Dictionary<string, FOVDeg> fov;

        static int left = (int)Eye.Left;
        static int right = (int)Eye.Right;

        public OculusProfile(Hmd hmd, HmdDesc? hmdDesc)
        {
            // Oculus Riftデバイスのタイプ
            hmdType = hmdDesc.Value.Type.ToString();

            // Oculus Riftのユーザープロファイルの各データを取得
            //user = hmd.GetString(Hmd.OVR_KEY_USER, "unknown");
            //name = hmd.GetString(Hmd.OVR_KEY_NAME, "unknown");
            //gender = hmd.GetString(Hmd.OVR_KEY_GENDER, "unknown");
            playerHeight = hmd.GetFloat(Hmd.OVR_KEY_PLAYER_HEIGHT, Hmd.OVR_DEFAULT_PLAYER_HEIGHT);
            eyeHeight = hmd.GetFloat(Hmd.OVR_KEY_EYE_HEIGHT, Hmd.OVR_DEFAULT_EYE_HEIGHT);
            ipd = hmd.GetFloat(Hmd.OVR_KEY_IPD, Hmd.OVR_DEFAULT_IPD);
            neckToEyeDistance = hmd.GetFloatArray(Hmd.OVR_KEY_NECK_TO_EYE_DISTANCE, new[] { 0f, 0f });
            eyeReliefDial = (byte)hmd.GetInt(Hmd.OVR_KEY_EYE_RELIEF_DIAL, Hmd.OVR_DEFAULT_EYE_RELIEF_DIAL);
            eyeToNoseDistance = hmd.GetFloatArray(Hmd.OVR_KEY_EYE_TO_NOSE_DISTANCE, new[] { 0f, 0f });
            maxEyeToPlateDistanse = hmd.GetFloatArray(Hmd.OVR_KEY_MAX_EYE_TO_PLATE_DISTANCE, new[] { 0f, 0f });
            eyeCup = hmd.GetString(Hmd.OVR_KEY_EYE_CUP, "C")[0];
            customEyeRender = hmd.GetBool(Hmd.OVR_KEY_CUSTOM_EYE_RENDER, false);
            cameraPosition = hmd.GetFloatArray(Hmd.OVR_KEY_CAMERA_POSITION, Hmd.OVR_DEFAULT_CAMERA_POSITION);
            var leftFov = hmdDesc.Value.DefaultEyeFov[left];
            var rightFov = hmdDesc.Value.DefaultEyeFov[right];            
            fov = new Dictionary<string, FOVDeg>{
                {"left", new FOVDeg{
                    upDegrees = (float)Math.Atan(leftFov.UpTan) * 180 / (float)Math.PI,
                    downDegrees = (float)Math.Atan(leftFov.DownTan) * 180 / (float)Math.PI,
                    leftDegrees = (float)Math.Atan(leftFov.LeftTan) * 180 / (float)Math.PI,
                    rightDegrees = (float)Math.Atan(leftFov.RightTan) * 180 / (float)Math.PI}
                },
                {"right", new FOVDeg{
                    upDegrees = (float)Math.Atan(rightFov.UpTan) * 180 / (float)Math.PI,
                    downDegrees = (float)Math.Atan(rightFov.DownTan) * 180 / (float)Math.PI,
                    leftDegrees = (float)Math.Atan(rightFov.LeftTan) * 180 / (float)Math.PI,
                    rightDegrees = (float)Math.Atan(rightFov.RightTan) * 180 / (float)Math.PI}
                }
            };
        }
    }

    class FOVDeg
    {
        public float upDegrees;
        public float downDegrees;
        public float leftDegrees;
        public float rightDegrees;
    }
}

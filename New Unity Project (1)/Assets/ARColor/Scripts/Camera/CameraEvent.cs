using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCenter
{
    public class CameraEvent : MonoBehaviour
    {
        /// <summary>
        /// Triggered when the camera is rotated to a certain angle
        /// </summary>
        /// <param name="angleType">Angle type</param>
        /// <param name="reach">Whether to reach the point of rotation</param>
        public delegate void CameraReachAngleHandler(int angleType, bool reach);

        public static event CameraReachAngleHandler CameraReachAngleEvent;

        public static void RaiseCameraReachAngle(int angleType, bool reach)
        {
            if (CameraReachAngleEvent != null)
            {
                CameraReachAngleEvent(angleType, reach);
            }
        }

    }
}


using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Lockout_2_core.Custom_Player_Behavior
{
    public class ThirdPersonCamTarget : MonoBehaviour
    {
        public ThirdPersonCamTarget(IntPtr value) : base(value) { }

        public void Update()
        {
            transform.eulerAngles = Vector3.zero;
        }
    }
}

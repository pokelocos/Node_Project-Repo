using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RA.InputExtencion
{
    public static class InputExtencions
    {
        public static bool GetDoubleMouseButton(this Input input)
        {
            /*
            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.unscaledTime - lastLeftClickTime;

                if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
                {
                    leftDoubleClick = true;
                }

                lastLeftClickTime = Time.unscaledTime;
            }
            */
            return true;
        }

        public static bool GetDoubleMouseButtonDown()
        {
            return true;
        }

        public static bool GetDoubleMouseButtonUp()
        {
            return true;
        }
    }
}
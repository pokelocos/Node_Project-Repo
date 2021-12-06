using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace RA.InputManager
{
    public class InputManager : MonoBehaviour
    {
        private const float DOUBLE_CLICK_TIME = .2f;

        private SelectableObject overObject;
        private SelectableObject[] overObjects;
        private SelectableObject draggedObject;
        private bool leftDoubleClick;
        private bool rigthDoubleClick;
        private float lastLeftClickTime;
        private float lastRightClickTime;

        public bool isDebugMode = false;

        public bool LeftDoubleClick
        {
            get
            {
                return leftDoubleClick;
            }
        }

        public bool RightDoubleClick
        {
            get
            {
                return rigthDoubleClick;
            }
        }

        /// <summary>
        /// Get the first selectable object in the raycast.
        /// </summary>
        public SelectableObject OverObject
        {
            get
            {
                return overObject;
            }
        }

        /// <summary>
        /// Get all selectable objects in the raycast.
        /// </summary>
        public SelectableObject[] OverObjects
        {
            get
            {
                return overObjects;
            }
        }

        /// <summary>
        /// Get current dragged object.
        /// </summary>
        public SelectableObject DraggedObject
        {
            get
            {
                return draggedObject;
            }
        }

        void Update()
        {
            //Left double click
            leftDoubleClick = false;

            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.unscaledTime - lastLeftClickTime;

                if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
                {
                    leftDoubleClick = true;
                }

                lastLeftClickTime = Time.unscaledTime;
            }

            //Right double click
            rigthDoubleClick = false;

            if (Input.GetMouseButtonDown(1))
            {
                float timeSinceLastClick = Time.unscaledTime - lastRightClickTime;

                if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
                {
                    rigthDoubleClick = true;
                }

                lastRightClickTime = Time.unscaledTime;
            }

            if (Input.GetMouseButtonUp(1))
            {
                draggedObject = null;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                overObject = null;
                overObjects = null;
                return;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);

            List<SelectableObject> matches = new List<SelectableObject>();

            foreach (var hit in hits)
            {
                var selectable = hit.collider.gameObject.GetComponentsInChildren<SelectableObject>();

                if (selectable.Length > 0)
                {
                    if (selectable != null)
                    {
                        matches.Add(selectable.First());
                    }
                }
            }

            if (matches.Count > 0)
            {
                //NodeController objects set to the top
                matches = matches.OrderByDescending(x => (x is NodeController)? 1 : 0).ToList();

                overObject = matches.First();
                overObjects = matches.ToArray();
            }
            else
            {
                overObject = null;
                overObject = null;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (overObject != null)
                {
                    draggedObject = overObject;
                }
            }
        }

        private void OnGUI()
        {
            if (isDebugMode)
            {
                var width = Screen.width / 3;
                var height = Screen.height / 7;

                var overObjectsLenght = overObjects != null ? overObjects.Length : 0;

                string log = "Over object: " + overObject + "\n Over objects count: " + overObjectsLenght + "\n Dragged Object: " + draggedObject;

                GUIStyle style = new GUIStyle();

                style.fontSize = 20;
                style.alignment = TextAnchor.MiddleLeft;

                GUI.Box(new Rect(20, Screen.height - height, width, height), log, style);
            }
        }
    }
}

public interface SelectableObject
{

}
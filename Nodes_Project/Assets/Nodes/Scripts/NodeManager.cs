
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private NodeView overNode;  
    private NodeView originNode;
    private NodeView destNode;
    private bool drag = false;
    [Header("References")]
    [SerializeField]
    private ConectionView auxiliarConection;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = .2f;

    void Update()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);

        NodeView hitNode = null;
        ConectionView hitConnection = null;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if(hitNode == null)
                {
                    try
                    {
                        hitNode = hit.collider.GetComponentInChildren<NodeView>();
                    }
                    catch
                    {
                        hitNode = null;
                    }
                }

                if (hitConnection == null)
                {
                    try
                    {
                        hitConnection = hit.collider.GetComponentInChildren<ConectionView>();
                    }
                    catch
                    {
                        hitConnection = null;
                    }
                }
            }
        }

        overNode = hitNode;


        if (Input.GetMouseButtonDown(1))
        {
            originNode = overNode;

            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                if (hitConnection!= null)
                {
                    hitConnection.Disconnect();
                }
            }

            lastClickTime = Time.time;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (originNode != null && overNode != null && originNode != overNode)
            {
                destNode = overNode;

                originNode.TryConnectWith(destNode);
            }

            originNode = null;
            destNode = null;
        }

        if (originNode != null && destNode == null)
        {
            Vector3 originPos = originNode.transform.position;
            Vector3 destPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            destPos.z = originPos.z;

            auxiliarConection.gameObject.SetActive(true);

            auxiliarConection.FollowPositions(originPos, destPos);
        }
        else
        {
            auxiliarConection.gameObject.SetActive(false);
        }

    }    
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private NodeView overNode;
    private NodeView originNode;
    private NodeView destNode;

    [Header("References")]
    [SerializeField]
    private ConectionView auxiliarConection;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);

        if (hit.collider != null)
        {
            var node = hit.collider.GetComponentInChildren<NodeView>();

            if (node != null)
            {
                overNode = node;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            originNode = overNode;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (originNode != null && originNode != overNode)
            {
                destNode = overNode;

                var conection = Instantiate(auxiliarConection, null);
                conection.SetNodes(originNode, destNode);
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

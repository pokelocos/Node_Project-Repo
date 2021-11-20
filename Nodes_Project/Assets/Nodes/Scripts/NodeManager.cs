using System.Linq;
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

    [SerializeField]
    private SpriteRenderer mouseIcon;
    [SerializeField]
    private SpriteRenderer mouseIcon_background;
    [SerializeField]
    private SpriteRenderer mouseIcon_ring;
    [SerializeField]
    private Sprite invalidIngredient;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = .2f;

    void Update()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);

        NodeView hitNode = null;
        ConectionView hitConnection = null;

        mouseIcon.color = Color.clear;
        mouseIcon_background.color = Color.clear;
        mouseIcon_ring.color = Color.clear;

        Vector3 destPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.7f, 0.7f, 0); 

        destPos.z = 0;

        mouseIcon.transform.position = destPos;

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

        if (hitConnection)
        {
            if (hitConnection.GetIngredient() != null)
            {
                mouseIcon.sprite = hitConnection.GetIngredient().icon;
                mouseIcon.color = hitConnection.GetIngredient().color;

                Color darker = Color.Lerp(hitConnection.GetIngredient().color, Color.black, 0.5f);

                mouseIcon_background.color = darker;
                mouseIcon_ring.color = new Color(0, 0, 0, 0.15f);

                if (!Input.GetMouseButton(1))
                {
                    mouseIcon.transform.position = hitConnection.GetMiddlePoint();
                }
            }
        }



        if (Input.GetMouseButtonDown(1))
        {
            originNode = overNode;

            float timeSinceLastClick = Time.unscaledTime - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                if (hitConnection!= null)
                {
                    hitConnection.Disconnect();
                }
            }

            lastClickTime = Time.unscaledTime;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (originNode != null && overNode != null && originNode != overNode)
            {
                destNode = overNode;

                if (originNode.CanConnectWith(destNode) == 2)
                {
                    originNode.ConnectWith(destNode);
                }
            }

            var connections = FindObjectsOfType<ConectionView>().Where(x => x != auxiliarConection).ToArray(); ;

            foreach (var connection in connections)
            {
                if (connection.GetIngredient() != null)
                {
                    connection.SetDotColor(connection.GetIngredient().color);
                }

                connection.SetLineColor(connection.body_color, connection.border_color);
            }

            foreach (var node in FindObjectsOfType<NodeView>().Where(x => x != originNode).ToArray())
            {
                node.Paint(node.GetColor());
                node.SetBright(Color.clear);
            }

            originNode = null;
            destNode = null;
        }

        if (originNode != null && destNode == null)
        {
            Vector3 originPos = originNode.transform.position;
            destPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (originNode.GetCurrentRecipe() != null)
            {
                for (int i = 0; i < originNode.GetOutputs().Length; i++)
                {
                    if (originNode.GetOutputs()[i] == null)
                    {
                        var ingredient = originNode.GetCurrentRecipe().GetOutputs()[i];

                        mouseIcon.sprite = ingredient.icon;
                        mouseIcon.color = ingredient.color;

                        Color darker = Color.Lerp(ingredient.color, Color.black, 0.5f);

                        mouseIcon_background.color = darker;
                        mouseIcon_ring.color = new Color(0, 0, 0, 0.15f);

                        break;
                    }
                }
            }
            else
            {
                mouseIcon.sprite = invalidIngredient;
                mouseIcon.color = Color.red;

                Color darker = Color.Lerp(Color.red, Color.black, 0.5f);

                mouseIcon_background.color = darker;
                mouseIcon_ring.color = new Color(0, 0, 0, 0.15f);
            }

            destPos.z = originPos.z;

            auxiliarConection.gameObject.SetActive(true);

            auxiliarConection.FollowPositions(originPos, destPos);

            var connections = FindObjectsOfType<ConectionView>().Where(x => x != auxiliarConection).ToArray();

            foreach (var connection in connections)
            {
                connection.SetDotColor(Color.grey);

                Color lightGray = Color.Lerp(Color.gray, Color.white, 0.5f);

                connection.SetLineColor(lightGray, Color.gray);
            }

            var nodes = FindObjectsOfType<NodeView>().Where(x => x != originNode).ToArray();

            if (hitNode)
            {
                if (hitNode != originNode)
                {
                    int affinity = originNode.CanConnectWith(hitNode);

                    switch (affinity)
                    {
                        case 0:
                            Color lightGray = Color.Lerp(Color.gray, Color.white, 0.5f);
                            auxiliarConection.SetLineColor(lightGray, Color.gray);
                            break;

                        case 1:
                            Color lightOrange = Color.Lerp(new Color(1, 0.6f, 0, 1), Color.white, 0.5f);
                            auxiliarConection.SetLineColor(lightOrange, new Color(1, 0.6f, 0, 1));
                            break;

                        case 2:
                            Color lightGreen = Color.Lerp(new Color(0.7f, 0.9f, 0.3f), Color.white, 0.5f);
                            auxiliarConection.SetLineColor(lightGreen, new Color(0.7f, 0.9f, 0.3f));
                            break;
                    }
                }
            }
            else
            {
                Color lightGray = Color.Lerp(Color.gray, Color.white, 0.5f);
                auxiliarConection.SetLineColor(lightGray, Color.gray);
            }

            foreach (var node in nodes)
            {
                int affinity = originNode.CanConnectWith(node);

                switch (affinity)
                {
                    case 0:
                        node.Paint(Color.gray);
                        break;

                    case 1:
                        node.SetBright(new Color(1, 0.6f, 0, 1));
                        break;

                    case 2:
                        node.SetBright(new Color(0.7f, 0.9f, 0.3f));
                        break;
                }
            }
            
        }
        else
        {
            auxiliarConection.gameObject.SetActive(false);
        }

    }    
}

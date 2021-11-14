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

            var nodes = FindObjectsOfType<NodeView>().Where(x => x != originNode).ToArray();

            List<NodeView> validNodes = new List<NodeView>();
            List<NodeView> possibleNodes = new List<NodeView>();

            //Show nodes afinnity with selected Recipe

            if (originNode.GetCurrentRecipe() != null)
            {
                foreach (var node in nodes)
                {
                    if (node.GetNodeName() == "Supermarket")
                    {
                        validNodes.Add(node);

                        continue;
                    }

                    if (!validNodes.Contains(node))
                    {
                        foreach (var nodeRecipe in node.GetRecipes())
                        {
                            if (nodeRecipe.InputIngredientsMatchCount(originNode.GetCurrentRecipe().GetOutputs()) > 0)
                            {
                                bool filledInputs = true;

                                foreach (var input in node.GetInputs())
                                {
                                    if (input == null)
                                    {
                                        filledInputs = false;
                                        break;
                                    }
                                }

                                if (filledInputs)
                                {
                                    if (!possibleNodes.Contains(node))
                                    {
                                        possibleNodes.Add(node);
                                    }

                                    continue;
                                }
                                else
                                {
                                    validNodes.Add(node);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            foreach (var node in nodes)
            {
                if (!validNodes.Contains(node) && !possibleNodes.Contains(node))
                {
                    node.Paint(Color.gray);
                    continue;
                }

                node.SetBright(new Color(1, 0.6f, 0, 1));
                //node.Paint(new Color(1, 0.6f, 0, 1));
            }

            if (originNode.GetCurrentRecipe() != null)
            {
                var currentIngredientIndex = 0;
                var occupiedOutputs = 0;

                for (int i = 0; i < originNode.GetOutputs().Length; i++)
                {
                    if (originNode.GetOutputs()[i] == null)
                    {
                        currentIngredientIndex = i;
                        break;
                    }
                    else
                    {
                        occupiedOutputs++;
                    }
                }

                var currentIngredient = originNode.GetCurrentRecipe().GetOutputs()[currentIngredientIndex];

                if (occupiedOutputs == originNode.GetOutputs().Length)
                {
                    foreach (var node in nodes)
                    {
                        node.Paint(Color.gray);
                    }
                }
                else
                {
                    foreach (var node in validNodes)
                    {
                        if (node.GetNodeName() == "Supermarket")
                        {
                            node.SetBright(Color.green);
                            //node.Paint(Color.green);
                            continue;
                        }

                        bool validNode = false;

                        foreach (var recipe in node.GetRecipes())
                        {
                            if (recipe.InputIngredientsMatchCount(new Ingredient[1] { currentIngredient }) > 0)
                            {
                                validNode = true;
                                break;
                            }
                        }

                        if (validNode)
                        {
                            foreach (var input in node.GetInputs())
                            {
                                if (input != null && currentIngredient == input)
                                {
                                    validNode = false;
                                    break;
                                }
                            }
                        }

                        if (validNode)
                        {
                            node.SetBright(Color.green);
                            //node.Paint(Color.green);
                        }
                    }
                }

                
            }
        }
        else
        {
            auxiliarConection.gameObject.SetActive(false);
        }

    }    
}

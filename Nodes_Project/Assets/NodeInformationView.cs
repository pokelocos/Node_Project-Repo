using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInformationView : MonoBehaviour
{
    private const float DOUBLE_CLICK_TIME = .2f;

    [SerializeField] private GameObject DisplayPrefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform canvas;
    private GameObject spawnedDisplay;

    private float lastClickTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.unscaledTime - lastClickTime;

            if(timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                //Double
                Spawn();
            }

            lastClickTime = Time.unscaledTime;
        }
    }

    private void Spawn()
    {
        NodeView node;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
        try
        {
            node = hit.collider.GetComponentInChildren<NodeView>();
        }
        catch
        {
            node = null;
        }

        if (node != null)
        {
            if (spawnedDisplay == null)
            {
                spawnedDisplay = Instantiate(DisplayPrefab,canvas);
                spawnedDisplay.GetComponent<NodeInformationDisplay>().SetData(node.GetNodeData());
                spawnedDisplay.GetComponent<NodeInformationDisplay>().SetRecipes(node);
            }
            else
            {
                spawnedDisplay.SetActive(true);
                spawnedDisplay.transform.SetParent(canvas);
                spawnedDisplay.GetComponent<NodeInformationDisplay>().SetData(node.GetNodeData());
                spawnedDisplay.GetComponent<NodeInformationDisplay>().SetRecipes(node);

            }
        }
    }


}

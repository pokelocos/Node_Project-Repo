using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public Vector2 start, end;
    public float speed;
    public Transform infoPanel;

    private Rect infoRect;
    private Rect pivotRect;

    // Start is called before the first frame update
    void Start()
    {
        infoRect = infoPanel.GetComponent<RectTransform>().rect;
        pivotRect = GetComponent<RectTransform>().rect;
    }

    // Update is called once per frame
    void Update()
    {
        infoPanel.transform.position += new Vector3(0, speed * Time.deltaTime, 0);

        if(infoRect.yMin > (start + pivotRect.position).y)
        {
            infoPanel.transform.position = end + pivotRect.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var s = start + pivotRect.position;
        Gizmos.DrawLine(s + new Vector2(300, 0), s + new Vector2(-300, 0));
        Gizmos.DrawSphere(s + new Vector2(300, 0), 5f);
        Gizmos.DrawSphere(s + new Vector2(-300, 0), 5f);
        
        Gizmos.color = Color.red;
        var e = end + pivotRect.position;
        Gizmos.DrawLine(e + new Vector2(300, 0), e + new Vector2(-300, 0));
        Gizmos.DrawSphere(e + new Vector2(300, 0), 5f);
        Gizmos.DrawSphere(e + new Vector2(-300, 0), 5f);
    }
}

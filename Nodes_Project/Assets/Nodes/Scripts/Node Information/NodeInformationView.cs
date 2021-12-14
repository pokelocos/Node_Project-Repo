using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInformationView : MonoBehaviour
{
    private bool isDragging;
    private Vector3 dragPoint;
    private int currentPageIndex;

    [Header("UI")]
    [SerializeField] private RectTransform[] pageButtons;
    [SerializeField] private GameObject[] pages;

    public void DisplayInformation(NodeController node)
    {
        gameObject.SetActive(true);
        transform.GetChild(0).localPosition = Vector3.zero;
        ChangePage(0);
    }

    private void Update()
    {
        ManagePageButtons();
    }

    public void BeginDrag()
    {
        isDragging = true;

        dragPoint = transform.GetChild(0).position - Input.mousePosition;
    }

    private void ManagePageButtons()
    {
        for (int i = 0; i < pageButtons.Length; i++)
        {
            if (i == currentPageIndex)
            {
                pageButtons[i].sizeDelta = new Vector2(pageButtons[i].sizeDelta.x, Mathf.Lerp(pageButtons[i].sizeDelta.y, 70, Time.unscaledDeltaTime * 5));
            }
            else
            {
                pageButtons[i].sizeDelta = new Vector2(pageButtons[i].sizeDelta.x, Mathf.Lerp(pageButtons[i].sizeDelta.y, 50, Time.unscaledDeltaTime * 5));
            }
        }
    }

    public void ChangePage(int index)
    {
        pages[currentPageIndex].SetActive(false);
        currentPageIndex = index;
        pages[currentPageIndex].SetActive(true);
    }

    public void Drag()
    {
        transform.GetChild(0).position = dragPoint + Input.mousePosition;
    }

    public void StopDrag()
    {
        isDragging = true;
    }
}

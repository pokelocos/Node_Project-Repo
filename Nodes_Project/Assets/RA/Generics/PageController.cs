using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RA.Generic
{
    public class PageController : MonoBehaviour
    {
        [SerializeField] private Button nextBtn;
        [SerializeField] private Button prevBtn;

        [SerializeField] private bool loopeable;

        private List<GameObject> pages = new List<GameObject>();
        private int actual = 0;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                pages.Add(transform.GetChild(i).gameObject);
            }
        }

        private void Start()
        {
            ActivePage(0);
        }

        public void NextPage()
        {
            actual = (actual + 1) % pages.Count;
            SetInteractableBtn(actual);
            ActivePage(actual);
        }

        public void PrevPage()
        {
            actual = (actual - 1) % pages.Count;
            SetInteractableBtn(actual);
            ActivePage(actual);
        }

        private void SetInteractableBtn(int n)
        {
            nextBtn.interactable = true;
            prevBtn.interactable = true;

            if (!loopeable)
            {
                if (actual <= 0)
                {
                    prevBtn.interactable = false;
                }

                if (actual >= pages.Count - 1)
                {
                    nextBtn.interactable = false;
                }
            }
        }

        private void ActivePage(int n)
        {
            foreach (var page in pages)
            {
                page.SetActive(false);
            }
            pages[n].SetActive(true);
        }
    }
}

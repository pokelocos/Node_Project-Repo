using RA.DialogSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public DialogView diagView;
    public Button btn_next;
    public List<Dialog> dialogs;

    [Header("cancercito de referencias")]
    public NodeView N1;
    public NodeView N2;
    public NodeView N3;

    public bool active;

    private int i_sentence = 0;
    private int n_diag = 0;

    private void Start()
    {
        btn_next.onClick.AddListener(() => {
            if (n_diag < dialogs.Count)
                NextSentence(n_diag);
            else
                diagView.gameObject.SetActive(false);
        });

        if(active)
        {
            diagView.gameObject.SetActive(true);
            if(n_diag < dialogs.Count)
                NextSentence(n_diag);
            else
                diagView.gameObject.SetActive(false);
        }
        else
        {
            // spawn 3 nodes
            // start X misiones
        }
    }

    public void NextSentence(int n)
    {
        var d = dialogs[n];
        diagView.ShowSentence(d.GetSentence(i_sentence), 0.05f);

        i_sentence = (i_sentence + 1)%d.Count;

        if (i_sentence == 0) 
            n_diag = n_diag + 1;
    }
}

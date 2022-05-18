using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRadio : MonoBehaviour
{
    private static MainRadio instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

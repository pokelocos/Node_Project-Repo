using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class InformationView : MonoBehaviour
{
    // target // <-HERE

    public abstract void DisplayInformation(object target);

}
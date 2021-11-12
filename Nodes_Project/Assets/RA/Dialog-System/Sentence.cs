using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RA.DialogSystem
{
    [System.Serializable]
    public class Sentence
    {
        [TextArea]
        public string text;
        public Actor actor;
        public Condicion a;
    }

    public enum Emotion //Ccambiar esto por un sistema que utilice string (!!!)
    {
        Default = -1,
        Happy = 0,
        Angry = 1,
        Sad = 2,
        Surprised = 3
    }
}

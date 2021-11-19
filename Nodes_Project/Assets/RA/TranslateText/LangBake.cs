using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RA.LangUtils
{
    // https://gist.github.com/grimmdev/979877fcdc943267e44c
    // This package is based on the link above

    [RequireComponent(typeof(Text))]
    public class LangBake : MonoBehaviour
    {
        private Text textDisplay;
        private string _default = "";
        //public List<Tuple<string, string>> texts;
        [SerializeField]
        public Dictionary<string, string> texts = new Dictionary<string, string>();

        private void Awake()
        {
            textDisplay = this.GetComponent<Text>();
            _default = textDisplay.text;
            TrySetLeguage(LangUtils.DefaultLenguage);
        }

        public void TrySetLeguage(string lenguage)
        {
            var s = "";
            texts.TryGetValue(lenguage, out s);
            /*
            if (!s.Equals(""))
            {
                textDisplay.text = s;
            }
            else
            {
                // do nothing
            }
            */
        }

        public void AddTranslation(string lang, string text)
        {
            texts.Add(lang,text);
        }

    }
}

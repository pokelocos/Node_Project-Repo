using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RA.Generic
{
    public class DontDestroyOnLoadElement : MonoBehaviour
    {
        private static List<string> _objects;

        private void Awake()
        {
            if (_objects.Contains(this.name))
            {
                Destroy(this.gameObject);
            }
            else
            {
                _objects.Add(this.name);
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}
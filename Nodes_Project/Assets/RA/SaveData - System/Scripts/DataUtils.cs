using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DataSystem
{
    public static class DataUtils
    {
        [RuntimeInitializeOnLoadMethod]
        public static void LoadDataOnStartGame()
        {
            var d = DataManager.LoadData<Data>();

            // aqui podria ir lo de la sincronizacion con "SteamCloud"

            if (d == null)
            {
                Debug.Log("there is no 'data' on this pc previously, creating new 'data'");
                DataManager.NewData<Data>("");
                SceneManager.LoadScene("Animation");
            }
        }

    }
}
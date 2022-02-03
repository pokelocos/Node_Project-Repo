using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    /// <summary>
    /// Keep all the information with static references so that you have access to the saved 
    /// information from any part of the code.
    /// Access this information through the "singleton" object of the "Data" object.
    /// </summary>
    public static class StaticData // Arreglar este nombre de clase (!!!)
    {
        /// <summary>
        /// Singleton of "Data" object.
        /// </summary>
        private static Data data;
        public static Data Data
        {
            get{
                if (data != null){
                    return data;
                }
                else {
                    var d = DataManager.LoadData<Data>();
                    if (d != null) {
                        return data = d;
                    }
                    else{
                        return data = DataManager.NewData<Data>("");
                    }
                }
            }

            set{
                data = DataManager.SaveData<Data>(value);
            }
        }
    }
}
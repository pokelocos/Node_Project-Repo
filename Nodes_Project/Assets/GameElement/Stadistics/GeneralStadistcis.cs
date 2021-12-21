using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GeneralStadistcis
{
    private static GeneralStadistcis instance;
    private static GeneralStadistcis Instance
    {
        get
        {
            if (instance == null)
            {
                // var data = LoadData();
                // instance = data.GeneralStadistics;
            }
            return instance;
        }

        set
        {
            // instance = value;
            // var data = LoadData();
            // data.GeneralStadistics = instance;
            // SaveData(data);
        }
    }


    public int gamesPlayed = 0;
    public int gameWin = 0;
    public int totalMoneyEarned = 0;
    public Dictionary<string, int> amountOfNodesPurchased = new Dictionary<string, int>();

}

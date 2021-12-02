using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodeLogic : MonoBehaviour
{
    void Start()
    {
        var words = new string[8]{"Vision", "Ensalada", "Momia", "Rata", "Tejon", "Sombrero", "Lagarto", "Papaya"};

        List<List<string>> allCombinations = new List<List<string>>();

        for(int i = 1; i < words.Length; i++)
        {
            List<List<string>> combinations = Combinations<string>.GetCombinations(words.ToList(), i);
            allCombinations.AddRange(combinations);

            foreach(var phrase in combinations)
            {
                string log = "Phrase: ";

                foreach(var word in phrase)
                {
                    log += word + " + ";
                }

                Debug.Log(log);
            }
        }

        Debug.Log(allCombinations.Count);
    }
}

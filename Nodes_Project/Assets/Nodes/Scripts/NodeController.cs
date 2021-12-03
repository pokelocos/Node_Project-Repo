using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RA.InputManager;

public class NodeController : MonoBehaviour, SelectableObject
{
    private List<ConnectionController> inputConnections = new List<ConnectionController>();
    private List<ConnectionController> outputConnections = new List<ConnectionController>();

    void Start()
    {
        //var words = new string[8]{"Vision", "Ensalada", "Momia", "Rata", "Tejon", "Sombrero", "Lagarto", "Papaya"};

        //List<List<string>> allCombinations = new List<List<string>>();

        //for(int i = 1; i < words.Length; i++)
        //{
        //    List<List<string>> combinations = Combinations<string>.GetCombinations(words.ToList(), i);
        //    allCombinations.AddRange(combinations);

        //    foreach(var phrase in combinations)
        //    {
        //        string log = "Phrase: ";

        //        foreach(var word in phrase)
        //        {
        //            log += word + " + ";
        //        }

        //        Debug.Log(log);
        //    }
        //}

        //Debug.Log(allCombinations.Count);
    }

    public void ConnectionUpdated(ConnectionView connection)
    {

    }

    public void AddInput(ConnectionController connection)
    {
        inputConnections.Add(connection);
    }

    public void AddOutput(ConnectionController connection)
    {
        outputConnections.Add(connection);
    }

    public void RemoveInput(ConnectionController connection)
    {
        inputConnections.Remove(connection);
    }

    public void RemoveOutput(ConnectionController connection)
    {
        outputConnections.Remove(connection);
    }

    /// <summary>
    /// Evaluate if this node can connect with another node. Returns:
    /// 0 - Can't connect
    /// 1 - Can Connect
    /// 2 - It's possible but not right now.
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public int CanConnectWith(NodeController candidate, Ingredient ingredient)
    {
        //Check if this node is not the same
        if (this == candidate)
        {
            return 0;
        }

        //Default can connect
        return 1;
    }

    public Ingredient GetNextOutputIngredient()
    {
        return null;
    }

    //private void OnDestroy()
    //{
    //    foreach (var input in inputConnections)
    //    {
    //        var connectionView = input.GetComponent<ConnectionView>();

    //        connectionView.onConnectionCreated -= ConnectionUpdated;
    //        connectionView.onConnectionDestroyed -= ConnectionUpdated;
    //    }

    //    foreach (var output in outputConnections)
    //    {
    //        var connectionView = output.GetComponent<ConnectionView>();

    //        connectionView.onConnectionCreated -= ConnectionUpdated;
    //        connectionView.onConnectionDestroyed -= ConnectionUpdated;
    //    }
    //}
}

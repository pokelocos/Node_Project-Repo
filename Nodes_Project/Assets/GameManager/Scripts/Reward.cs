using MicroFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Reward
{
    public NodeData[] nodes;
    public EffectData[] effects;
    public int price;

    public Reward(NodeData[] nodes, EffectData[] effects, int difficulty)
    {
        this.nodes = nodes;
        this.effects = effects;
        price = CalculePrice(1,difficulty);
    }

    private int CalcEffectsAmout(int difficulty)
    {
        int effectsCount = 0;
        if (UnityEngine.Random.Range(0, 1f) > 0.5f)
        {
            effectsCount++;
            if (UnityEngine.Random.Range(0, 1f) > 0.75f)
                effectsCount++;
        }
        return effectsCount;
    }

    private int CalculePrice(int MaintainMultyply, int difficulty)
    {
        var price = 0;
        for (int i = 0; i < this.nodes.Length; i++)
        {
            price += this.nodes[i].maintainCost * MaintainMultyply;
        }
        price /= effects.Length + 1;

        price += UnityEngine.Random.Range(1, 3) * difficulty * 5;

        return price;
    }

}
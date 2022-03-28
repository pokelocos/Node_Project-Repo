using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "RA/Nodes/Actions/Sell Produts")]
public class SellProducts : NodeActions
{
    public ShopProductSettings[] shopProductSettings;

    public override void CallAction(NodeController node)
    {
        var manifest = node.GetProductionManifest();

        float totalMoney = 0;

        foreach (var productionReport in manifest)
        {
            bool itsSold = false;

            var price = 0;

            foreach (var setting in shopProductSettings)
            {
                if (productionReport.Product.data.tags.Contains(setting.tag) || setting.tag == "ANY")
                {
                    price = (int)(productionReport.Product.currentValue * setting.multiplier);
                    itsSold = true;
                    break;
                }
            }

            if (!itsSold)
            {
                price = productionReport.Product.currentValue;
            }

            totalMoney += price;

            //GameManager.AddMoney(price);
        }

        node.NodeView.DisplayMiscText("$" + totalMoney, Color.green);
    }

    [System.Serializable]
    public class ShopProductSettings
    {
        public string tag;
        public float multiplier = 1;
    }
}

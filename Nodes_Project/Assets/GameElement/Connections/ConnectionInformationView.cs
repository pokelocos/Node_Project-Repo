using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionInformationView : MonoBehaviour
{
    [SerializeField] private Image ingredientBackground_image;
    [SerializeField] private Image ingredient_image;
    [SerializeField] private Image node_image;
    [SerializeField] private Image nodeBackground_image;
    [SerializeField] private Image connection_image;

    private Port currentPort;

    public void Display(Port port, NodeController node)
    {
        ingredient_image.sprite = port.Product.data.icon;
        ingredient_image.color = port.Product.data.color.Darker();
        ingredientBackground_image.color = port.Product.data.color;

        node_image.sprite = node.GetData().icon;
        node_image.color = node.GetData().color.Darker();
        nodeBackground_image.color = node.GetData().color;

        connection_image.color = port.connection.GetQueueColor();

        currentPort = port;
    }

    private void Update()
    {
        connection_image.color = currentPort.connection.GetQueueColor();
    }
}
using UnityEngine;

public class Node
{
    private bool isUsable;
    private GameObject symbol;

    public bool IsUsable => isUsable;
    public GameObject Symbol => symbol;

    public Node(bool isUsable, GameObject symbol)
    {
        this.isUsable = isUsable;
        this.symbol = symbol;
    }
}

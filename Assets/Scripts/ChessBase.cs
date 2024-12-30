using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessBase : MonoBehaviour
{
    //private int baseCost;
    private int level; //当前棋子等级，从一开始
    private ChessProperty properties = null;

    public int quantity
    { get
        {
            int result = 1;
            for (int i = 1; i < level; i++)
            {
                result *= ChessProperty.NUM_OF_UPGRADE;
            }
            return result;
        } 
    }

    public ChessBase()
    {
        level = 1;
    }

    public void SetProperties(ChessProperty properties)
    {
        this.properties = properties;
    }

    public void SetProperties(JsonPropertiy jsonPropertiy)
    {
        if (properties == null)
        {
            properties = new ChessProperty();
        }
        properties.Load(jsonPropertiy);
    }

    public int GetCost()
    {
        return properties.GetCost(level);
    }

    public void Upgrade()
    {
        level++;
    }
}

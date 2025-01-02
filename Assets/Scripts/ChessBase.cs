using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class ChessBase : MonoBehaviour
{
    //private int baseCost;
    private ChessProperty property = null;

    public int level { get; private set; } //当前棋子等级，从一开始
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

    public void CopyProperties(ChessBase chessBase)
    {
        this.property = Util.DeepCopy(chessBase.property);
    }

    public void SetProperty(ChessProperty properties)
    {
        this.property = properties;
    }

    //public void SetProperties(JsonPropertiy jsonPropertiy)
    //{
    //    if (property == null)
    //    {
    //        property = new ChessProperty();
    //    }
    //    property.Load(jsonPropertiy);
    //}

    public string GetName()
    {
        return property.Name;
    }

    public int GetCost()
    {
        return property.GetCost(level);
    }

    public int GetBaseCost()
    {
        return property.GetCost(1);
    }

    public void Upgrade()
    {
        level++;
    }

    public bool isMaxLevel()
    {
        return level <= ChessProperty.MAX_LEVEL;
    }
}

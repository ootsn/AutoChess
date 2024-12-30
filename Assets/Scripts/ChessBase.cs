using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class ChessBase : MonoBehaviour
{
    //private int baseCost;
    private ChessProperty properties = null;

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
        this.properties = DeepCopy(chessBase.properties);
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

    public bool isMaxLevel()
    {
        return level <= ChessProperty.MAX_LEVEL;
    }

    public static T DeepCopy<T>(T obj)
    {
        if (obj == null)
        {
            return obj;
        }
        var type = obj.GetType();
        if (obj is string || type.IsValueType)
        {
            return obj;
        }

        var result = Activator.CreateInstance(type);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        foreach (var field in fields)
        {
            field.SetValue(result, field.GetValue(obj));
        }
        return (T)result;
    }
}

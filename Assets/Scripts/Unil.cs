using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

class Util
{
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

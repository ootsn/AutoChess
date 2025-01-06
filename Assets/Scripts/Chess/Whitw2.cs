using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whitw2 : ChessBase
{
    public Whitw2 () 
    {
        ChessProperty property = new ChessProperty ();
        property.SetBaseCost(3);
        property.Name = "whitw2";
        property.Hp = new int[] { 900, 1450, 2000 };
        property.Atk = new int[] { 45, 90, 160 };
        property.Def = new int[] { 30, 50, 100 };
        property.MagicResistance = new float[] { 0.2f, 0.4f, 0.5f };
        property.AttackRange = 5;
        property.AttackSpeed = 0.95f;

        SetProperty(property);
    }

    public override void Skill()
    {
        throw new System.NotImplementedException();
    }
}

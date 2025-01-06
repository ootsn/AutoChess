using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisdel : ChessBase
{
    public Wisdel()
    {
        ChessProperty property = new ChessProperty();
        property.SetBaseCost(3);
        property.Name = "wisdel";
        property.Hp = new int[] { 800, 1400, 2000 };
        property.Atk = new int[] { 50, 100, 150 };
        property.Def = new int[] { 30, 50, 100 };
        property.MagicResistance = new float[] { 0.1f, 0.2f, 0.3f };
        property.AttackRange = 5;
        property.AttackSpeed = 0.8f;

        SetProperty(property);
    }

    public override void Skill()
    {
        throw new System.NotImplementedException();
    }
}

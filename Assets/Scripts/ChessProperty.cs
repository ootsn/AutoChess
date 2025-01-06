using Mirror.BouncyCastle.Asn1.Cmp;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 抗性类型
/// </summary>
public enum AbnormalResistanceType
{
    SILENCE, //沉默
    STUN, //晕眩
    SLEEP, //沉睡
    FROZEN, //冻结
    FLOAT, //浮空
    SHIVER, //战栗
    FEAR, //恐惧
    COUNT //枚举长度
}

/// <summary>
/// 棋子的属性
/// </summary>
public class ChessProperty
{
    public static readonly int MAX_LEVEL = 3; //最高等级
    public static readonly int NUM_OF_UPGRADE = 3; //升级所需棋子个数

    //属性用多少写多少，直接写太多太乱

    //基础属性
    //数组表示每个等级不同数值，单个值表示所有等级共用
    //棋子等级从1开始，从数组中取值要先减一
    public string Name;
    //[SerializeField]
    public int[] Hp; //基础生命值
    //[SerializeField]
    public int[] Atk; //基础攻击力
    //[SerializeField]
    public int[] Def; //基础防御力
    //[SerializeField]
    public float[] MagicResistance; //基础法术抗性
    //[SerializeField]
    public float[] EpDamageResistance; //基础元素抗性
    //[SerializeField]
    public float[] EpResistance; //基础损伤抗性
    //[SerializeField]
    public int AttackRange; //攻击范围
    //[SerializeField]
    public float AttackSpeed; //攻击速度，默认为1.0
    //[SerializeField]
    public float AttackTime; //基础攻击间隔(s)，实际攻击间隔=基础攻击间隔/攻击速度
    //[SerializeField]
    public float MoveSpeed; //移动速度，每秒移动多少格
    //[SerializeField]
    public int TauntLevel; //嘲讽等级，优先攻击嘲讽等级高的
    //[SerializeField]
    public float EvasionPhysical; //物理伤害闪避率
    //[SerializeField]
    public float EvasionMagical; //法术伤害闪避率
    //[SerializeField]
    public float DamageHitratePhysical; //物理命中率
    //[SerializeField]
    public float DamageHitrateMagical; //法术命中率
    //[SerializeField]
    public int HpRecoverySpeed; //生命值回复速度（每秒回复多少点生命值），默认为0
    //[SerializeField]
    public float HpPercentRecoverySpeed; // 生命百分比回复速度（每秒回复百分之多少的最大生命值），默认为0
    //[SerializeField]
    public int MassLevel; //重量等级
    //[SerializeField]
    public int ForceLevel; //力度等级
    //[SerializeField]
    public float SkillRecoveryBonus; //技力回复加成
    //[SerializeField]
    public bool[] AbnormalResistance; //异常抗性，有(true)或无(false)
    //[SerializeField]
    public float[] AbnormalResistanceRatio; //抵抗倍率，当进入某种异常状态时，实际异常时间=设定的时间/该倍率，默认为1.0

    //[SerializeField]
    private int[] cost; //费用

    //修饰区，可以考虑放到ChessBase
    private int atkDirectPlus; //攻击力的直接加算
    private int atkDirectMultiply; //攻击力的直接乘算
    private int atkFinalPlus; //攻击力的最终加算
    private int atkFinalMultiply; //攻击力的最终乘算，攻击力倍率也放这
    //剩下的先不写，用到再写，不然太乱


    //先这样，后续属性加上攻击力直接加/乘算，攻击力最终加/乘算，攻击力倍率，各类伤害倍率，治疗倍率，无视百分比/数值的防御/法抗，各类抗性减少百分比/数值。函数中要写技能，各种异常状态的处理，写成抽象方法
    //属性里面估计还得加目前锁定的敌人，索敌算法写在别的类，可以用广度优先锁定敌人，后续用A*算法寻路
    public ChessProperty()
    {
        AttackSpeed = 1f;
        AbnormalResistance = new bool[(int)AbnormalResistanceType.COUNT];
        AbnormalResistanceRatio = new float[(int)AbnormalResistanceType.COUNT];
        for (int i = 0; i < AbnormalResistanceRatio.Length; i++)
        {
            AbnormalResistanceRatio[i] = 1f;
        }
    }

    //public void Load(JsonPropertiy jsonPropertiy)
    //{
    //    SetBaseCost(jsonPropertiy.baseCost);
    //    Hp = jsonPropertiy.hp;
    //    Atk = jsonPropertiy.atk;
    //    Def = jsonPropertiy.def;
    //    MagicResistance = jsonPropertiy.magicResistance;
    //    AttackRange = jsonPropertiy.attackRange;
    //    AttackSpeed = jsonPropertiy.attackSpeed;
    //}

    public int GetCost(int level)
    {
        return cost[level - 1];
    }

    public void SetBaseCost(int baseCost)
    {
        cost = new int[MAX_LEVEL];
        cost[0] = baseCost;
        int numOfUpgrade = NUM_OF_UPGRADE;
        for (int i = 1; i < cost.Length; i++)
        {
            cost[i] = baseCost * numOfUpgrade - 1;
            numOfUpgrade *= NUM_OF_UPGRADE;
        }
    }
}
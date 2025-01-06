using Mirror.BouncyCastle.Asn1.Cmp;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public enum AbnormalResistanceType
{
    SILENCE, //��Ĭ
    STUN, //��ѣ
    SLEEP, //��˯
    FROZEN, //����
    FLOAT, //����
    SHIVER, //ս��
    FEAR, //�־�
    COUNT //ö�ٳ���
}

/// <summary>
/// ���ӵ�����
/// </summary>
public class ChessProperty
{
    public static readonly int MAX_LEVEL = 3; //��ߵȼ�
    public static readonly int NUM_OF_UPGRADE = 3; //�����������Ӹ���

    //�����ö���д���٣�ֱ��д̫��̫��

    //��������
    //�����ʾÿ���ȼ���ͬ��ֵ������ֵ��ʾ���еȼ�����
    //���ӵȼ���1��ʼ����������ȡֵҪ�ȼ�һ
    public string Name;
    //[SerializeField]
    public int[] Hp; //��������ֵ
    //[SerializeField]
    public int[] Atk; //����������
    //[SerializeField]
    public int[] Def; //����������
    //[SerializeField]
    public float[] MagicResistance; //������������
    //[SerializeField]
    public float[] EpDamageResistance; //����Ԫ�ؿ���
    //[SerializeField]
    public float[] EpResistance; //�������˿���
    //[SerializeField]
    public int AttackRange; //������Χ
    //[SerializeField]
    public float AttackSpeed; //�����ٶȣ�Ĭ��Ϊ1.0
    //[SerializeField]
    public float AttackTime; //�����������(s)��ʵ�ʹ������=�����������/�����ٶ�
    //[SerializeField]
    public float MoveSpeed; //�ƶ��ٶȣ�ÿ���ƶ����ٸ�
    //[SerializeField]
    public int TauntLevel; //����ȼ������ȹ�������ȼ��ߵ�
    //[SerializeField]
    public float EvasionPhysical; //�����˺�������
    //[SerializeField]
    public float EvasionMagical; //�����˺�������
    //[SerializeField]
    public float DamageHitratePhysical; //����������
    //[SerializeField]
    public float DamageHitrateMagical; //����������
    //[SerializeField]
    public int HpRecoverySpeed; //����ֵ�ظ��ٶȣ�ÿ��ظ����ٵ�����ֵ����Ĭ��Ϊ0
    //[SerializeField]
    public float HpPercentRecoverySpeed; // �����ٷֱȻظ��ٶȣ�ÿ��ظ��ٷ�֮���ٵ��������ֵ����Ĭ��Ϊ0
    //[SerializeField]
    public int MassLevel; //�����ȼ�
    //[SerializeField]
    public int ForceLevel; //���ȵȼ�
    //[SerializeField]
    public float SkillRecoveryBonus; //�����ظ��ӳ�
    //[SerializeField]
    public bool[] AbnormalResistance; //�쳣���ԣ���(true)����(false)
    //[SerializeField]
    public float[] AbnormalResistanceRatio; //�ֿ����ʣ�������ĳ���쳣״̬ʱ��ʵ���쳣ʱ��=�趨��ʱ��/�ñ��ʣ�Ĭ��Ϊ1.0

    //[SerializeField]
    private int[] cost; //����

    //�����������Կ��Ƿŵ�ChessBase
    private int atkDirectPlus; //��������ֱ�Ӽ���
    private int atkDirectMultiply; //��������ֱ�ӳ���
    private int atkFinalPlus; //�����������ռ���
    private int atkFinalMultiply; //�����������ճ��㣬����������Ҳ����
    //ʣ�µ��Ȳ�д���õ���д����Ȼ̫��


    //���������������Լ��Ϲ�����ֱ�Ӽ�/���㣬���������ռ�/���㣬���������ʣ������˺����ʣ����Ʊ��ʣ����Ӱٷֱ�/��ֵ�ķ���/���������࿹�Լ��ٰٷֱ�/��ֵ��������Ҫд���ܣ������쳣״̬�Ĵ���д�ɳ��󷽷�
    //����������ƻ��ü�Ŀǰ�����ĵ��ˣ������㷨д�ڱ���࣬�����ù�������������ˣ�������A*�㷨Ѱ·
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
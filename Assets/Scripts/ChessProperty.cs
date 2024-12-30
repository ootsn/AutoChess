using Mirror.BouncyCastle.Asn1.Cmp;
using System;
using System.Reflection;
using UnityEngine;

//��������
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

public class ChessProperty
{
    public static readonly int MAX_LEVEL = 3; //��ߵȼ�
    public static readonly int NUM_OF_UPGRADE = 3; //�����������Ӹ���

    //�����ö���д���٣�ֱ��д̫��̫��

    //��������
    //�����ʾÿ���ȼ���ͬ��ֵ������ֵ��ʾ���еȼ�����
    //���ӵȼ���1��ʼ����������ȡֵҪ�ȼ�һ
    //[SerializeField]
    private int[] cost; //����
    //[SerializeField]
    private int[] hp; //��������ֵ
    //[SerializeField]
    private int[] atk; //����������
    //[SerializeField]
    private int[] def; //����������
    //[SerializeField]
    private float[] magicResistance; //������������
    //[SerializeField]
    private float[] epDamageResistance; //����Ԫ�ؿ���
    //[SerializeField]
    private float[] epResistance; //�������˿���
    //[SerializeField]
    private int attackRange; //������Χ
    //[SerializeField]
    private float attackSpeed; //�����ٶȣ�Ĭ��Ϊ1.0
    //[SerializeField]
    private float attackTime; //�����������(s)��ʵ�ʹ������=�����������/�����ٶ�
    //[SerializeField]
    private float moveSpeed; //�ƶ��ٶȣ�ÿ���ƶ����ٸ�
    //[SerializeField]
    private int tauntLevel; //����ȼ������ȹ�������ȼ��ߵ�
    //[SerializeField]
    private float evasionPhysical; //�����˺�������
    //[SerializeField]
    private float evasionMagical; //�����˺�������
    //[SerializeField]
    private float damageHitratePhysical; //����������
    //[SerializeField]
    private float damageHitrateMagical; //����������
    //[SerializeField]
    private int hpRecoverySpeed; //����ֵ�ظ��ٶȣ�ÿ��ظ����ٵ�����ֵ����Ĭ��Ϊ0
    //[SerializeField]
    private float hpPercentRecoverySpeed; // �����ٷֱȻظ��ٶȣ�ÿ��ظ��ٷ�֮���ٵ��������ֵ����Ĭ��Ϊ0
    //[SerializeField]
    private int massLevel; //�����ȼ�
    //[SerializeField]
    private int forceLevel; //���ȵȼ�
    //[SerializeField]
    private float skillRecoveryBonus; //�����ظ��ӳ�
    //[SerializeField]
    private bool[] abnormalResistance; //�쳣���ԣ���(true)����(false)
    //[SerializeField]
    private float[] abnormalResistanceRatio; //�ֿ����ʣ�������ĳ���쳣״̬ʱ��ʵ���쳣ʱ��=�趨��ʱ��/�ñ��ʣ�Ĭ��Ϊ1.0

    //������
    private int atkDirectPlus; //��������ֱ�Ӽ���
    private int atkDirectMultiply; //��������ֱ�ӳ���
    private int atkFinalPlus; //�����������ռ���
    private int atkFinalMultiply; //�����������ճ��㣬����������Ҳ����
    //ʣ�µ��Ȳ�д���õ���д����Ȼ̫��


    //���������������Լ��Ϲ�����ֱ�Ӽ�/���㣬���������ռ�/���㣬���������ʣ������˺����ʣ����Ʊ��ʣ����Ӱٷֱ�/��ֵ�ķ���/���������࿹�Լ��ٰٷֱ�/��ֵ��������Ҫд���ܣ������쳣״̬�Ĵ���д�ɳ��󷽷�
    //����������ƻ��ü�Ŀǰ�����ĵ��ˣ������㷨д�ڱ���࣬�����ù�������������ˣ�������A*�㷨Ѱ·
    public ChessProperty()
    {
        attackSpeed = 1f;
        abnormalResistance = new bool[(int)AbnormalResistanceType.COUNT];
        abnormalResistanceRatio = new float[(int)AbnormalResistanceType.COUNT];
        for (int i = 0; i < abnormalResistanceRatio.Length; i++)
        {
            abnormalResistanceRatio[i] = 1f;
        }
    }

    public void Load(JsonPropertiy jsonPropertiy)
    {
        SetBaseCost(jsonPropertiy.baseCost);
        hp = jsonPropertiy.hp;
        atk = jsonPropertiy.atk;
        def = jsonPropertiy.def;
        magicResistance = jsonPropertiy.magicResistance;
        attackRange = jsonPropertiy.attackRange;
        attackSpeed = jsonPropertiy.attackSpeed;
    }

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
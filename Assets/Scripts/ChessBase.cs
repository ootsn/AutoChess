using System;
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

public abstract class ChessBase : MonoBehaviour
{
    //�����ö���д���٣�ֱ��д̫��̫��

    //��������
    //�����ʾÿ���Ǽ���ͬ��ֵ������ֵ��ʾ�����Ǽ�����
    [SerializeField]
    private int[] cost; //����
    [SerializeField]
    private int[] hp; //��������ֵ
    [SerializeField]
    private int[] atk; //����������
    [SerializeField]
    private int[] def; //����������
    [SerializeField]
    private int[] magicResistance; //������������
    [SerializeField]
    private int[] epDamageResistance; //����Ԫ�ؿ���
    [SerializeField]
    private int[] epResistance; //�������˿���
    [SerializeField]
    private int attackRange; //������Χ
    [SerializeField]
    private float attackSpeed; //�����ٶȣ�Ĭ��Ϊ1.0
    [SerializeField]
    private float attackTime; //�����������(s)��ʵ�ʹ������=�����������/�����ٶ�
    [SerializeField]
    private float moveSpeed; //�ƶ��ٶȣ�ÿ���ƶ����ٸ�
    [SerializeField]
    private int tauntLevel; //����ȼ������ȹ�������ȼ��ߵ�
    [SerializeField]
    private float evasionPhysical; //�����˺�������
    [SerializeField]
    private float evasionMagical; //�����˺�������
    [SerializeField]
    private float damageHitratePhysical; //����������
    [SerializeField]
    private float damageHitrateMagical; //����������
    [SerializeField]
    private int hpRecoverySpeed; //����ֵ�ظ��ٶȣ�ÿ��ظ����ٵ�����ֵ����Ĭ��Ϊ0
    [SerializeField]
    private float hpPercentRecoverySpeed; // �����ٷֱȻظ��ٶȣ�ÿ��ظ��ٷ�֮���ٵ��������ֵ����Ĭ��Ϊ0
    [SerializeField]
    private int massLevel; //�����ȼ�
    [SerializeField]
    private int forceLevel; //���ȵȼ�
    [SerializeField]
    private float skillRecoveryBonus; //�����ظ��ӳ�
    [SerializeField]
    private bool[] abnormalResistance; //�쳣���ԣ���(true)����(false)
    [SerializeField]
    private float[] abnormalResistanceRatio; //�ֿ����ʣ�������ĳ���쳣״̬ʱ��ʵ���쳣ʱ��=�趨��ʱ��/�ñ��ʣ�Ĭ��Ϊ1.0

    //������
    private int atkDirectPlus; //��������ֱ�Ӽ���
    private int atkDirectMultiply; //��������ֱ�ӳ���
    private int atkFinalPlus; //�����������ռ���
    private int atkFinalMultiply; //�����������ճ��㣬����������Ҳ����
    //ʣ�µ��Ȳ�д���õ���д����Ȼ̫��


    //���������������Լ��Ϲ�����ֱ�Ӽ�/���㣬���������ռ�/���㣬���������ʣ������˺����ʣ����Ʊ��ʣ����Ӱٷֱ�/��ֵ�ķ���/���������࿹�Լ��ٰٷֱ�/��ֵ��������Ҫд���ܣ������쳣״̬�Ĵ���д�ɳ��󷽷�
    //����������ƻ��ü�Ŀǰ�����ĵ��ˣ������㷨д�ڱ���࣬�����ù�������������ˣ�������A*�㷨Ѱ·
    public ChessBase()
    {
        attackSpeed = 1f;
        abnormalResistance = new bool[(int)AbnormalResistanceType.COUNT];
        abnormalResistanceRatio = new float[(int)AbnormalResistanceType.COUNT];
        for (int i = 0; i < abnormalResistanceRatio.Length; i++)
        {
            abnormalResistanceRatio[i] = 1f;
        }
    }
}
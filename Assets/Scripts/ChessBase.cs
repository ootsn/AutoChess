using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ���ӵĻ���
/// </summary>
[RequireComponent(typeof(ChessMove))]
public abstract class ChessBase : MonoBehaviour
{
    //private int baseCost;

    /// <summary>
    /// ���ӵ�����
    /// </summary>
    private ChessProperty property;

    /// <summary>
    /// ���ӵ�״̬��
    /// </summary>
    private FSMBuilder<ChessStateEnum, ChessBase> fsmBuilder;

    /// <summary>
    /// ��ǰ���ӵȼ�����һ��ʼ
    /// </summary>
    public int level { get; private set; }

    /// <summary>
    /// ��ǰ����������һ����������Ϊ1��������������Ϊ3��������������Ϊ9���Դ�����
    /// </summary>
    public int quantity
    { 
        get
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
        property = null;
        InitFSM();
    }


    /// <summary>
    /// ��ʼ��״̬��
    /// </summary>
    private void InitFSM()
    {
        //��������״̬���ȴ���״̬����Ϊ��Ȼ����״̬�󶨣������ӵ�״̬����
        //����
        HTNPlanBuilder idleHTNBuilder = new HTNPlanBuilder();
        idleHTNBuilder.CompoundTask()
                    .Method(() => true)
                        .Idle(this)
                    .End();
        ChessState idleState = new ChessState(idleHTNBuilder, ChessStateEnum.IDLE);
        fsmBuilder.AddState(idleState);

        //Ѱ�ҵ���
        HTNPlanBuilder findEnemyHTNBuilder = new HTNPlanBuilder();
        findEnemyHTNBuilder.CompoundTask()
                    .Method(() => true)
                        .FindEnemy(this, null)
                    .End();
        ChessState findEnemyState = new ChessState(findEnemyHTNBuilder, ChessStateEnum.FIND_ENEMY);
        fsmBuilder.AddState(findEnemyState);

        //�ƶ�
        HTNPlanBuilder moveHTNBuilder = new HTNPlanBuilder();


        //��������״̬ת������


        //����״̬��
        fsmBuilder = new FSMBuilder<ChessStateEnum, ChessBase>();

    }

    /// <summary>
    /// ������ӵ�����
    /// </summary>
    /// <param name="chessBase">�������������������</param>
    public void CopyProperty(ChessBase chessBase)
    {
        this.property = Util.DeepCopy(chessBase.property);
    }

    /// <summary>
    /// ǳ�������ӵ�����
    /// </summary>
    /// <param name="chessBase">ǳ��������������������</param>
    public void SetProperty(ChessBase chessBase)
    {
        this.property = chessBase.property;
    }

    /// <summary>
    /// ǳ�������ӵ�����
    /// </summary>
    /// <param name="properties">ǳ����������</param>
    public void SetProperty(ChessProperty properties)
    {
        this.property = properties;
    }

    /// <summary>
    /// ��ȡ���ӵ�����
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        return property.Name;
    }

    /// <summary>
    /// ��ȡ���ӵķ��ã��������ӵȼ�
    /// </summary>
    /// <returns></returns>
    public int GetCost()
    {
        return property.GetCost(level);
    }

    /// <summary>
    /// ��ȡ���ӵĻ�������
    /// </summary>
    /// <returns></returns>
    public int GetBaseCost()
    {
        return property.GetCost(1);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Upgrade()
    {
        level++;
    }

    /// <summary>
    /// �����Ƿ�ﵽ��ߵȼ�
    /// </summary>
    /// <returns></returns>
    public bool IsMaxLevel()
    {
        return level == ChessProperty.MAX_LEVEL;
    }

    /// <summary>
    /// ���ӵļ���
    /// </summary>
    public abstract void Skill();


}

/// <summary>
/// ���ӵ�״̬ö��
/// </summary>
public enum ChessStateEnum
{
    IDLE, //����
    FIND_ENEMY, //Ѱ�ҵ���
    MOVE, //�ƶ�
    ATTACK, //����
    SKILL, //�ͷż���
    DEAD, //����
    ABNORMAL //�쳣
}

/// <summary>
/// ���ӵ�״̬
/// </summary>
public class ChessState : FSMState<ChessStateEnum>
{
    private HTNPlanBuilder htnPlanBuilder;

    public ChessState(HTNPlanBuilder htnPlanBuilder, ChessStateEnum chessStateEnum) : base(chessStateEnum)
    {
        this.htnPlanBuilder = htnPlanBuilder;
    }
    public override void Execute()
    {
        htnPlanBuilder.RunPlan();
    }
}

/// <summary>
/// ����״̬ת������
/// </summary>
public class ChessStateTransitCondition : FSMCondition<ChessBase>
{
    public delegate bool MeetConditionDelegate(ChessBase chessBase);

    private MeetConditionDelegate meetCondition;

    public ChessStateTransitCondition(MeetConditionDelegate meetCondition)
    {
        this.meetCondition = meetCondition;
    }

    public bool MeetCondition(ChessBase chessBase)
    {
        return meetCondition(chessBase);
    }
}

/// <summary>
/// �������ӵ���Ϊ
/// </summary>
public partial class HTNPlanBuilder
{
    /// <summary>
    /// ���ӿ���ʱ����Ϊ
    /// </summary>
    /// <param name="chess">�ж�������</param>
    /// <returns></returns>
    public HTNPlanBuilder Idle(ChessBase chess)
    {
        throw new NotImplementedException();
        return this;
    }

    /// <summary>
    /// ����Ѱ�ҵ���ʱ����Ϊ
    /// </summary>
    /// <param name="chess">�ж�������</param>
    /// <param name="controller">�������������̿�����</param>
    /// <returns></returns>
    public HTNPlanBuilder FindEnemy(ChessBase chess, ChessControl controller)
    {
        //controller.FindNearestOpponentPlace()
        return this;
    }
}

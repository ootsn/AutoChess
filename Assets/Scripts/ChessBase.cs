using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 棋子的基类
/// </summary>
[RequireComponent(typeof(ChessMove))]
public abstract class ChessBase : MonoBehaviour
{
    //private int baseCost;

    /// <summary>
    /// 棋子的属性
    /// </summary>
    private ChessProperty property;

    /// <summary>
    /// 棋子的状态机
    /// </summary>
    private FSMBuilder<ChessStateEnum, ChessBase> fsmBuilder;

    /// <summary>
    /// 当前棋子等级，从一开始
    /// </summary>
    public int level { get; private set; }

    /// <summary>
    /// 当前棋子数量，一级棋子数量为1，二级棋子数量为3，三级棋子数量为9，以此类推
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
    /// 初始化状态机
    /// </summary>
    private void InitFSM()
    {
        //创建棋子状态。先创建状态的行为，然后与状态绑定，最后添加到状态机中
        //空闲
        HTNPlanBuilder idleHTNBuilder = new HTNPlanBuilder();
        idleHTNBuilder.CompoundTask()
                    .Method(() => true)
                        .Idle(this)
                    .End();
        ChessState idleState = new ChessState(idleHTNBuilder, ChessStateEnum.IDLE);
        fsmBuilder.AddState(idleState);

        //寻找敌人
        HTNPlanBuilder findEnemyHTNBuilder = new HTNPlanBuilder();
        findEnemyHTNBuilder.CompoundTask()
                    .Method(() => true)
                        .FindEnemy(this, null)
                    .End();
        ChessState findEnemyState = new ChessState(findEnemyHTNBuilder, ChessStateEnum.FIND_ENEMY);
        fsmBuilder.AddState(findEnemyState);

        //移动
        HTNPlanBuilder moveHTNBuilder = new HTNPlanBuilder();


        //创建棋子状态转换条件


        //创建状态机
        fsmBuilder = new FSMBuilder<ChessStateEnum, ChessBase>();

    }

    /// <summary>
    /// 深拷贝棋子的属性
    /// </summary>
    /// <param name="chessBase">深拷贝的属性所属的棋子</param>
    public void CopyProperty(ChessBase chessBase)
    {
        this.property = Util.DeepCopy(chessBase.property);
    }

    /// <summary>
    /// 浅拷贝棋子的属性
    /// </summary>
    /// <param name="chessBase">浅拷贝的属性所属的棋子</param>
    public void SetProperty(ChessBase chessBase)
    {
        this.property = chessBase.property;
    }

    /// <summary>
    /// 浅拷贝棋子的属性
    /// </summary>
    /// <param name="properties">浅拷贝的属性</param>
    public void SetProperty(ChessProperty properties)
    {
        this.property = properties;
    }

    /// <summary>
    /// 获取棋子的名字
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        return property.Name;
    }

    /// <summary>
    /// 获取棋子的费用，根据棋子等级
    /// </summary>
    /// <returns></returns>
    public int GetCost()
    {
        return property.GetCost(level);
    }

    /// <summary>
    /// 获取棋子的基础费用
    /// </summary>
    /// <returns></returns>
    public int GetBaseCost()
    {
        return property.GetCost(1);
    }

    /// <summary>
    /// 升级棋子
    /// </summary>
    public void Upgrade()
    {
        level++;
    }

    /// <summary>
    /// 棋子是否达到最高等级
    /// </summary>
    /// <returns></returns>
    public bool IsMaxLevel()
    {
        return level == ChessProperty.MAX_LEVEL;
    }

    /// <summary>
    /// 棋子的技能
    /// </summary>
    public abstract void Skill();


}

/// <summary>
/// 棋子的状态枚举
/// </summary>
public enum ChessStateEnum
{
    IDLE, //空闲
    FIND_ENEMY, //寻找敌人
    MOVE, //移动
    ATTACK, //攻击
    SKILL, //释放技能
    DEAD, //死亡
    ABNORMAL //异常
}

/// <summary>
/// 棋子的状态
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
/// 棋子状态转换条件
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
/// 补充棋子的行为
/// </summary>
public partial class HTNPlanBuilder
{
    /// <summary>
    /// 棋子空闲时的行为
    /// </summary>
    /// <param name="chess">行动的棋子</param>
    /// <returns></returns>
    public HTNPlanBuilder Idle(ChessBase chess)
    {
        throw new NotImplementedException();
        return this;
    }

    /// <summary>
    /// 棋子寻找敌人时的行为
    /// </summary>
    /// <param name="chess">行动的棋子</param>
    /// <param name="controller">棋子所属的棋盘控制器</param>
    /// <returns></returns>
    public HTNPlanBuilder FindEnemy(ChessBase chess, ChessControl controller)
    {
        //controller.FindNearestOpponentPlace()
        return this;
    }
}

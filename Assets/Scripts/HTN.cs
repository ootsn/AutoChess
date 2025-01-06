using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//网上抄的（划掉）借鉴的代码，用于实现HTN规划

//世界状态只有一个即可，我们将其设为静态类
public static class HTNWorld
{
    //读 世界状态的字典
    private static readonly Dictionary<string, Func<object>> get_WorldState;
    //写 世界状态的字典
    private static readonly Dictionary<string, Action<object>> set_WorldState;

    static HTNWorld()
    {
        get_WorldState = new Dictionary<string, Func<object>>();
        set_WorldState = new Dictionary<string, Action<object>>();
    }
    //添加一个状态，需要传入状态名、读取函数和写入函数
    public static void AddState(string key, Func<object> getter, Action<object> setter)
    {
        get_WorldState[key] = getter;
        set_WorldState[key] = setter;
    }
    //根据状态名移除某个世界状态
    public static void RemoveState(string key)
    {
        get_WorldState.Remove(key);
        set_WorldState.Remove(key);
    }
    //修改某个状态的值
    public static void UpdateState(string key, object value)
    {
        //就是通过写入字典修改的
        set_WorldState[key].Invoke(value);
    }
    //读取某个状态的值，利用泛型，可以将获取的object转为指定的类型
    public static T GetWorldState<T>(string key)
    {
        return (T)get_WorldState[key].Invoke();
    }
    //复制一份当前世界状态的值（这个主要是用在规划中）
    public static Dictionary<string, object> CopyWorldState()
    {
        var copy = new Dictionary<string, object>();
        foreach (var state in get_WorldState)
        {
            copy.Add(state.Key, state.Value.Invoke());
        }
        return copy;
    }
}

//用于描述运行结果的枚举（如果有看上一篇行为树的话，也可以直接用行为树的EStatus）
public enum EStatus
{
    Failure, Success, Running,
}

//任务接口，包含判断是否满足条件和添加子任务的方法
public interface IBaseTask
{
    //判断是否满足条件
    bool MetCondition(Dictionary<string, object> worldState);
    //添加子任务
    void AddNextTask(IBaseTask nextTask);
}

//原子任务的抽象类，继承自IBaseTask接口
public abstract class PrimitiveTask : IBaseTask
{
    //原子任务不可以再分解为子任务，所以AddNextTask方法不必实现
    void IBaseTask.AddNextTask(IBaseTask nextTask)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 执行前判断条件是否满足，传入null时直接修改HTNWorld
    /// </summary>
    /// <param name="worldState">用于plan的世界状态副本</param>
    public bool MetCondition(Dictionary<string, object> worldState = null)
    {
        if (worldState == null)//实际运行时
        {
            return MetCondition_OnRun();
        }
        else//模拟规划时，若能满足条件就直接进行Effect
        {
            if (MetCondition_OnPlan(worldState))
            {
                Effect_OnPlan(worldState);
                return true;
            }
            return false;
        }
    }
    protected virtual bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        return true;
    }
    protected virtual bool MetCondition_OnRun()
    {
        return true;
    }

    //任务的具体运行逻辑，交给具体类实现
    public abstract EStatus Operator();

    /// <summary>
    /// 执行成功后的影响，传入null时直接修改HTNWorld
    /// </summary>
    /// <param name="worldState">用于plan的世界状态副本</param>
    public void Effect(Dictionary<string, object> worldState = null)
    {
        Effect_OnRun();
    }
    protected virtual void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        ;
    }
    protected virtual void Effect_OnRun()
    {
        ;
    }
}

//方法，继承自IBaseTask接口
public class Method : IBaseTask
{
    //子任务列表，可以是复合任务，也可以是原点任务
    public List<IBaseTask> SubTask { get; private set; }
    //方法的前提条件
    private readonly Func<bool> condition;

    public Method(Func<bool> condition)
    {
        SubTask = new List<IBaseTask>();
        this.condition = condition;
    }
    //方法条件满足的判断=方法本身前提条件满足+所有子任务条件满足
    public bool MetCondition(Dictionary<string, object> worldState = null)
    {
        /*
        再复制一遍世界状态，用于追踪每个子任务的Effect。方法有多个子任务，
        只要其中一个不满足条件，那整个方法不满足条件，之前子任务进行Effect也不算数
        因此用tpWorld记录，待验证了方法满足条件后（所有子任务均满足条件），再复制回worldState
        */
        var tpWorld = new Dictionary<string, object>(worldState);
        if (condition())//方法自身的前提条件是否满足
        {
            for (int i = 0; i < SubTask.Count; ++i)
            {
                //一旦有一个子任务的条件不满足，这个方法就不满足了
                if (!SubTask[i].MetCondition(tpWorld))
                {
                    return false;
                }
            }
            //最终满足条件后，再将各Effect导致的新世界状态（tpWorld）给worldState
            worldState = tpWorld;
            return true;//如果子任务全都满足了，那就成了！
        }
        return false;
    }
    //添加子任务
    public void AddNextTask(IBaseTask nextTask)
    {
        SubTask.Add(nextTask);
    }
}

//复合任务，继承自IBaseTask接口
public class CompoundTask : IBaseTask
{
    //选中的方法
    public Method ValidMethod { get; private set; }
    //子任务（方法）列表
    private readonly List<Method> methods;

    public CompoundTask()
    {
        methods = new List<Method>();
    }

    public void AddNextTask(IBaseTask nextTask)
    {
        //要判断添加进来的是不是方法类，是的话才添加
        if (nextTask is Method m)
        {
            methods.Add(m);
        }
    }

    public bool MetCondition(Dictionary<string, object> worldState)
    {
        for (int i = 0; i < methods.Count; ++i)
        {
            //只要有一个方法满足前提条件就可以
            if (methods[i].MetCondition(worldState))
            {
                //记录下这个满足的方法
                ValidMethod = methods[i];
                return true;
            }
        }
        return false;
    }
}

//HTN规划类
public class HTNPlanner
{
    //最终分解完成的所有原子任务存放的列表
    public Stack<PrimitiveTask> FinalTasks { get; private set; }
    //分解过程中，用来缓存被分解出的任务的栈，因为类型各异，故用IBaseTask类型
    private readonly Stack<IBaseTask> taskOfProcess;
    private readonly CompoundTask rootTask;//根任务

    public HTNPlanner(CompoundTask rootTask)
    {
        this.rootTask = rootTask;
        taskOfProcess = new Stack<IBaseTask>();
        FinalTasks = new Stack<PrimitiveTask>();
    }
    //规划（核心）
    public void Plan()
    {
        //先复制一份世界状态
        var worldState = HTNWorld.CopyWorldState();
        //将存储列表清空，避免上次计划结果的影响
        FinalTasks.Clear();
        //将根任务压进栈中，准备分解
        taskOfProcess.Push(rootTask);
        //只要栈还没空，就继续分解
        while (taskOfProcess.Count > 0)
        {
            //拿出栈顶的元素
            var task = taskOfProcess.Pop();
            //如果这个元素是复合任务
            if (task is CompoundTask cTask)
            {
                //判断是否可以执行
                if (cTask.MetCondition(worldState))
                {
                    /*如果可以执行，就肯定有可用的方法，
                    就将该方法的子任务都压入栈中，以便继续分解*/
                    var subTask = cTask.ValidMethod.SubTask;
                    foreach (var t in subTask)
                    {
                        taskOfProcess.Push(t);
                    }
                    /*通过上面的步骤我们知道，能被压进栈中的只有
                    复合任务和原子任务，方法本身并不会入栈*/
                }
            }
            else //否则，这个元素就是原子任务
            {
                //将该元素转为原子任务，因为原本是IBaseTask类型
                var pTask = task as PrimitiveTask;
                //再将该原子任务加入存放分解完成的任务列表
                FinalTasks.Push(pTask);
            }
        }
    }
}

//执行HTN规划的类
public class HTNPlanRunner
{
    //当前运行状态
    private EStatus curState;
    //直接将规划器包含进来，方便重新规划
    private readonly HTNPlanner planner;
    //当前执行的原子任务
    private PrimitiveTask curTask;
    //标记「原子任务列表是否还有元素、能够继续」
    private bool canContinue;

    public HTNPlanRunner(HTNPlanner planner)
    {
        this.planner = planner;
        curState = EStatus.Failure;
    }

    public void RunPlan()
    {
        //如果当前运行状态是失败（一开始默认失败）
        if (curState == EStatus.Failure)
        {
            //就规划一次
            planner.Plan();
        }
        //如果当前运行状态是成功，就表示当前任务完成了
        if (curState == EStatus.Success)
        {
            //让当前原子任务造成影响
            curTask.Effect();
        }
        /*如果当前状态不是「正在执行」，就取出新一个原子任务作为当前任务
        无论失败还是成功，都要这么做。因为如果是失败，肯定在代码运行到这
        之前，已经进行了一次规划，理应获取新规划出的任务来运行；如果是因
        为成功，那也要取出新任务来运行*/
        if (curState != EStatus.Running)
        {
            //用TryPop的返回结果判断规划器的FinalTasks是否为空
            //canContinue = planner.FinalTasks.TryPop(out curTask);
            canContinue = planner.FinalTasks.Count > 0;
            if (canContinue)
            {
                curTask = planner.FinalTasks.Pop();
            }
        }
        /*如果canContinue为false，那curTask会为null也视作失败（其实应该是「全部
        完成」，但全部完成和失败是一样的，都要重新规划）。所以只有当canContinue && curTask.MetCondition()都满足时，才读取当前原子任务的运行状态，否则就失败。*/
        curState = canContinue && curTask.MetCondition() ? curTask.Operator() : EStatus.Failure;
    }
}

//构造器
public partial class HTNPlanBuilder
{
    private HTNPlanner planner;
    private HTNPlanRunner runner;
    private readonly Stack<IBaseTask> taskStack;

    public HTNPlanBuilder()
    {
        taskStack = new Stack<IBaseTask>();
    }

    private void AddTask(IBaseTask task)
    {
        if (planner != null)//当前计划器不为空
        {
            //将新任务作为构造栈顶元素的子任务
            taskStack.Peek().AddNextTask(task);
        }
        else //如果计划器为空，意味着新任务是根任务，进行初始化
        {
            planner = new HTNPlanner(task as CompoundTask);
            runner = new HTNPlanRunner(planner);
        }
        //如果新任务是原子任务，就不需要进栈了，因为原子任务不会有子任务
        if (!(task is PrimitiveTask))
        {
            taskStack.Push(task);
        }
    }
    //剩下的代码都很简单，我相信能直接看得懂
    public void RunPlan()
    {
        runner.RunPlan();
    }
    public HTNPlanBuilder Back()
    {
        taskStack.Pop();
        return this;
    }
    public HTNPlanner End()
    {
        taskStack.Clear();
        return planner;
    }
    public HTNPlanBuilder CompoundTask()
    {
        var task = new CompoundTask();
        AddTask(task);
        return this;
    }
    public HTNPlanBuilder Method(System.Func<bool> condition)
    {
        var task = new Method(condition);
        AddTask(task);
        return this;
    }
}

//使用方法
//hTN.CompoundTask()
//        .Method(() => isHurt)
//            .Enemy_Hurt(this)
//            .Enemy_Die(this)
//            .Back()
//        .Method(() => curHp <= trigger)
//            .Enemy_Combo(this, 3)
//            .Enemy_Rest(this, "victory")
//            .Back()
//        .Method(() => HTNWorld.GetWorldState<float>("PlayerHp") > 0)
//            .Enemy_Check(this)
//            .Enemy_Track(this, PlayerTrans)
//            .Enemy_Atk(this)
//            .Back()
//        .Method(() => true)
//            .Enemy_Idle(this, 3f)
//        .End();
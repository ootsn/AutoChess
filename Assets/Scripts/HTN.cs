using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ϳ��ģ�����������Ĵ��룬����ʵ��HTN�滮

//����״ֻ̬��һ�����ɣ����ǽ�����Ϊ��̬��
public static class HTNWorld
{
    //�� ����״̬���ֵ�
    private static readonly Dictionary<string, Func<object>> get_WorldState;
    //д ����״̬���ֵ�
    private static readonly Dictionary<string, Action<object>> set_WorldState;

    static HTNWorld()
    {
        get_WorldState = new Dictionary<string, Func<object>>();
        set_WorldState = new Dictionary<string, Action<object>>();
    }
    //���һ��״̬����Ҫ����״̬������ȡ������д�뺯��
    public static void AddState(string key, Func<object> getter, Action<object> setter)
    {
        get_WorldState[key] = getter;
        set_WorldState[key] = setter;
    }
    //����״̬���Ƴ�ĳ������״̬
    public static void RemoveState(string key)
    {
        get_WorldState.Remove(key);
        set_WorldState.Remove(key);
    }
    //�޸�ĳ��״̬��ֵ
    public static void UpdateState(string key, object value)
    {
        //����ͨ��д���ֵ��޸ĵ�
        set_WorldState[key].Invoke(value);
    }
    //��ȡĳ��״̬��ֵ�����÷��ͣ����Խ���ȡ��objectתΪָ��������
    public static T GetWorldState<T>(string key)
    {
        return (T)get_WorldState[key].Invoke();
    }
    //����һ�ݵ�ǰ����״̬��ֵ�������Ҫ�����ڹ滮�У�
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

//�����������н����ö�٣�����п���һƪ��Ϊ���Ļ���Ҳ����ֱ������Ϊ����EStatus��
public enum EStatus
{
    Failure, Success, Running,
}

//����ӿڣ������ж��Ƿ��������������������ķ���
public interface IBaseTask
{
    //�ж��Ƿ���������
    bool MetCondition(Dictionary<string, object> worldState);
    //���������
    void AddNextTask(IBaseTask nextTask);
}

//ԭ������ĳ����࣬�̳���IBaseTask�ӿ�
public abstract class PrimitiveTask : IBaseTask
{
    //ԭ�����񲻿����ٷֽ�Ϊ����������AddNextTask��������ʵ��
    void IBaseTask.AddNextTask(IBaseTask nextTask)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// ִ��ǰ�ж������Ƿ����㣬����nullʱֱ���޸�HTNWorld
    /// </summary>
    /// <param name="worldState">����plan������״̬����</param>
    public bool MetCondition(Dictionary<string, object> worldState = null)
    {
        if (worldState == null)//ʵ������ʱ
        {
            return MetCondition_OnRun();
        }
        else//ģ��滮ʱ����������������ֱ�ӽ���Effect
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

    //����ľ��������߼�������������ʵ��
    public abstract EStatus Operator();

    /// <summary>
    /// ִ�гɹ����Ӱ�죬����nullʱֱ���޸�HTNWorld
    /// </summary>
    /// <param name="worldState">����plan������״̬����</param>
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

//�������̳���IBaseTask�ӿ�
public class Method : IBaseTask
{
    //�������б������Ǹ�������Ҳ������ԭ������
    public List<IBaseTask> SubTask { get; private set; }
    //������ǰ������
    private readonly Func<bool> condition;

    public Method(Func<bool> condition)
    {
        SubTask = new List<IBaseTask>();
        this.condition = condition;
    }
    //��������������ж�=��������ǰ����������+������������������
    public bool MetCondition(Dictionary<string, object> worldState = null)
    {
        /*
        �ٸ���һ������״̬������׷��ÿ���������Effect�������ж��������
        ֻҪ����һ������������������������������������֮ǰ���������EffectҲ������
        �����tpWorld��¼������֤�˷�������������������������������������ٸ��ƻ�worldState
        */
        var tpWorld = new Dictionary<string, object>(worldState);
        if (condition())//���������ǰ�������Ƿ�����
        {
            for (int i = 0; i < SubTask.Count; ++i)
            {
                //һ����һ������������������㣬��������Ͳ�������
                if (!SubTask[i].MetCondition(tpWorld))
                {
                    return false;
                }
            }
            //���������������ٽ���Effect���µ�������״̬��tpWorld����worldState
            worldState = tpWorld;
            return true;//���������ȫ�������ˣ��Ǿͳ��ˣ�
        }
        return false;
    }
    //���������
    public void AddNextTask(IBaseTask nextTask)
    {
        SubTask.Add(nextTask);
    }
}

//�������񣬼̳���IBaseTask�ӿ�
public class CompoundTask : IBaseTask
{
    //ѡ�еķ���
    public Method ValidMethod { get; private set; }
    //�����񣨷������б�
    private readonly List<Method> methods;

    public CompoundTask()
    {
        methods = new List<Method>();
    }

    public void AddNextTask(IBaseTask nextTask)
    {
        //Ҫ�ж���ӽ������ǲ��Ƿ����࣬�ǵĻ������
        if (nextTask is Method m)
        {
            methods.Add(m);
        }
    }

    public bool MetCondition(Dictionary<string, object> worldState)
    {
        for (int i = 0; i < methods.Count; ++i)
        {
            //ֻҪ��һ����������ǰ�������Ϳ���
            if (methods[i].MetCondition(worldState))
            {
                //��¼���������ķ���
                ValidMethod = methods[i];
                return true;
            }
        }
        return false;
    }
}

//HTN�滮��
public class HTNPlanner
{
    //���շֽ���ɵ�����ԭ�������ŵ��б�
    public Stack<PrimitiveTask> FinalTasks { get; private set; }
    //�ֽ�����У��������汻�ֽ���������ջ����Ϊ���͸��죬����IBaseTask����
    private readonly Stack<IBaseTask> taskOfProcess;
    private readonly CompoundTask rootTask;//������

    public HTNPlanner(CompoundTask rootTask)
    {
        this.rootTask = rootTask;
        taskOfProcess = new Stack<IBaseTask>();
        FinalTasks = new Stack<PrimitiveTask>();
    }
    //�滮�����ģ�
    public void Plan()
    {
        //�ȸ���һ������״̬
        var worldState = HTNWorld.CopyWorldState();
        //���洢�б���գ������ϴμƻ������Ӱ��
        FinalTasks.Clear();
        //��������ѹ��ջ�У�׼���ֽ�
        taskOfProcess.Push(rootTask);
        //ֻҪջ��û�գ��ͼ����ֽ�
        while (taskOfProcess.Count > 0)
        {
            //�ó�ջ����Ԫ��
            var task = taskOfProcess.Pop();
            //������Ԫ���Ǹ�������
            if (task is CompoundTask cTask)
            {
                //�ж��Ƿ����ִ��
                if (cTask.MetCondition(worldState))
                {
                    /*�������ִ�У��Ϳ϶��п��õķ�����
                    �ͽ��÷�����������ѹ��ջ�У��Ա�����ֽ�*/
                    var subTask = cTask.ValidMethod.SubTask;
                    foreach (var t in subTask)
                    {
                        taskOfProcess.Push(t);
                    }
                    /*ͨ������Ĳ�������֪�����ܱ�ѹ��ջ�е�ֻ��
                    ���������ԭ�����񣬷�������������ջ*/
                }
            }
            else //�������Ԫ�ؾ���ԭ������
            {
                //����Ԫ��תΪԭ��������Ϊԭ����IBaseTask����
                var pTask = task as PrimitiveTask;
                //�ٽ���ԭ����������ŷֽ���ɵ������б�
                FinalTasks.Push(pTask);
            }
        }
    }
}

//ִ��HTN�滮����
public class HTNPlanRunner
{
    //��ǰ����״̬
    private EStatus curState;
    //ֱ�ӽ��滮�������������������¹滮
    private readonly HTNPlanner planner;
    //��ǰִ�е�ԭ������
    private PrimitiveTask curTask;
    //��ǡ�ԭ�������б��Ƿ���Ԫ�ء��ܹ�������
    private bool canContinue;

    public HTNPlanRunner(HTNPlanner planner)
    {
        this.planner = planner;
        curState = EStatus.Failure;
    }

    public void RunPlan()
    {
        //�����ǰ����״̬��ʧ�ܣ�һ��ʼĬ��ʧ�ܣ�
        if (curState == EStatus.Failure)
        {
            //�͹滮һ��
            planner.Plan();
        }
        //�����ǰ����״̬�ǳɹ����ͱ�ʾ��ǰ���������
        if (curState == EStatus.Success)
        {
            //�õ�ǰԭ���������Ӱ��
            curTask.Effect();
        }
        /*�����ǰ״̬���ǡ�����ִ�С�����ȡ����һ��ԭ��������Ϊ��ǰ����
        ����ʧ�ܻ��ǳɹ�����Ҫ��ô������Ϊ�����ʧ�ܣ��϶��ڴ������е���
        ֮ǰ���Ѿ�������һ�ι滮����Ӧ��ȡ�¹滮�������������У��������
        Ϊ�ɹ�����ҲҪȡ��������������*/
        if (curState != EStatus.Running)
        {
            //��TryPop�ķ��ؽ���жϹ滮����FinalTasks�Ƿ�Ϊ��
            //canContinue = planner.FinalTasks.TryPop(out curTask);
            canContinue = planner.FinalTasks.Count > 0;
            if (canContinue)
            {
                curTask = planner.FinalTasks.Pop();
            }
        }
        /*���canContinueΪfalse����curTask��ΪnullҲ����ʧ�ܣ���ʵӦ���ǡ�ȫ��
        ��ɡ�����ȫ����ɺ�ʧ����һ���ģ���Ҫ���¹滮��������ֻ�е�canContinue && curTask.MetCondition()������ʱ���Ŷ�ȡ��ǰԭ�����������״̬�������ʧ�ܡ�*/
        curState = canContinue && curTask.MetCondition() ? curTask.Operator() : EStatus.Failure;
    }
}

//������
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
        if (planner != null)//��ǰ�ƻ�����Ϊ��
        {
            //����������Ϊ����ջ��Ԫ�ص�������
            taskStack.Peek().AddNextTask(task);
        }
        else //����ƻ���Ϊ�գ���ζ���������Ǹ����񣬽��г�ʼ��
        {
            planner = new HTNPlanner(task as CompoundTask);
            runner = new HTNPlanRunner(planner);
        }
        //�����������ԭ�����񣬾Ͳ���Ҫ��ջ�ˣ���Ϊԭ�����񲻻���������
        if (!(task is PrimitiveTask))
        {
            taskStack.Push(task);
        }
    }
    //ʣ�µĴ��붼�ܼ򵥣���������ֱ�ӿ��ö�
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

//ʹ�÷���
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
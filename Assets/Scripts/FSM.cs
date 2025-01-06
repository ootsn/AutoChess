using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;


/// <summary>
/// ����״̬��������ͼ��ai���ɵ�
/// </summary>
/// <typeparam name="T1">״̬</typeparam>
/// <typeparam name="T2">����</typeparam>
public class FSMDirectedGraph<T1, T2>
{
    private Dictionary<T1, Dictionary<T2, T1>> graph = new Dictionary<T1, Dictionary<T2, T1>>();
    public void AddNode(T1 node)
    {
        if (!graph.ContainsKey(node))
        {
            graph[node] = new Dictionary<T2, T1>();
        }
    }
    public void RemoveNode(T1 node)
    {
        if (graph.ContainsKey(node))
        {
            graph.Remove(node);
        }
    }
    public void AddEdge(T1 from, T1 to, T2 condition)
    {
        if (!graph.ContainsKey(from))
        {
            AddNode(from);
        }
        if (!graph.ContainsKey(to))
        {
            AddNode(to);
        }
        graph[from].Add(condition, to);
    }
    public void RemoveEdge(T1 from, T1 to)
    {
        if (graph.ContainsKey(from))
        {
            foreach (var edge in graph[from])
            {
                if (edge.Value.Equals(to))
                {
                    graph[from].Remove(edge.Key);
                    break;
                }
            }
        }
    }
    public List<T1> GetNeighbors(T1 node)
    {
        List<T1> neighbors = new List<T1>();
        foreach (var edge in graph[node])
        {
            neighbors.Add(edge.Value);
        }
        return neighbors;
    }
    public List<T1> GetNodes()
    {
        return new List<T1>(graph.Keys);
    }
    public T2 GetAction(T1 from, T1 to)
    {
        foreach (var edge in graph[from])
        {
            if (edge.Value.Equals(to))
            {
                return edge.Key;
            }
        }
        return default(T2);
    }
    public Dictionary<T2, T1> this[T1 key]
    {
        get
        {
            return graph[key];
        }
        private set
        {
            graph[key] = value;
        }
    }
}

/// <summary>
/// ����״̬����״̬���Լ�д��
/// </summary>
/// <typeparam name="T">״̬�ı�ʾ��enum�ɣ�enum�õ�ʱ���Լ�д</typeparam>
public abstract class FSMState<T>
{
    private T state;

    public FSMState(T state)
    {
        this.state = state;
    }

    public T GetState()
    {
        return state;
    }

    private void SetState(T state)
    {
        this.state = state;
    }

    public abstract void Execute();

    public void Transit(T nextState)
    {
        SetState(nextState);
    }

    public static bool operator ==(FSMState<T> a, FSMState<T> b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(FSMState<T> a, FSMState<T> b)
    {
        return !a.Equals(b);
    }

    public override bool Equals(object obj)
    {
        return GetState().Equals((obj as FSMState<T>).GetState());
    }

    public override int GetHashCode()
    {
        return GetState().GetHashCode();
    }
}

/// <summary>
/// ����״̬����ת���������Լ�д��
/// </summary>
/// <typeparam name="T">�����ж����ⲿ����</typeparam>
public interface FSMCondition<T>
{
    public abstract bool MeetCondition(T condition);
}

/// <summary>
/// ����״̬��
/// </summary>
/// <typeparam name="T1">״̬�ı�ʾenum</typeparam>
/// <typeparam name="T2">����ǰ���������ж�����</typeparam>
public class FSM<T1, T2>
{
    private FSMDirectedGraph<FSMState<T1>, FSMCondition<T2>> graph;
    private FSMState<T1> currentState;

    public FSM(FSMDirectedGraph<FSMState<T1>, FSMCondition<T2>> graph = null, FSMState<T1> initialState = null)
    {
        this.graph = graph == null ? new FSMDirectedGraph<FSMState<T1>, FSMCondition<T2>>() : graph;
        this.currentState = initialState;
    }

    public void SetInitialState(FSMState<T1> initialState)
    {
        currentState = initialState;
    }

    public void AddState(FSMState<T1> state)
    {
        graph.AddNode(state);
    }

    public void AddTransition(FSMState<T1> from, FSMState<T1> to, FSMCondition<T2> fsmCondition)
    {
        graph.AddEdge(from, to, fsmCondition);
    }

    public void Execute()
    {
        currentState.Execute();
    }

    public bool Transit(T2 condition)
    {
        foreach (var edge in graph[currentState])
        {
            if (edge.Key.MeetCondition(condition))
            {
                currentState.Transit(edge.Value.GetState());
                return true;
            }
        }
        return false;
    }

    public T GetCurrentState<T>() where T : FSMState<T1>
    {
        return (T)currentState;
    }
}

/// <summary>
/// ����״̬�������������Լ�д��
/// </summary>
/// <typeparam name="T1">״̬�ı�ʾenum</typeparam>
/// <typeparam name="T2">����ǰ���������ж�����</typeparam>
public class FSMBuilder<T1, T2>
{
    private FSM<T1, T2> fsm;

    public FSMBuilder()
    {
        fsm = new FSM<T1, T2>();
    }

    public FSMBuilder<T1, T2> AddState(FSMState<T1> state)
    {
        fsm.AddState(state);
        return this;
    }

    public FSMBuilder<T1, T2> AddTransition(FSMState<T1> from, FSMState<T1> to, FSMCondition<T2> condition)
    {
        fsm.AddTransition(from, to, condition);
        return this;
    }

    public FSMBuilder<T1, T2> SetInitialState(FSMState<T1> initialState)
    {
        fsm.SetInitialState(initialState);
        return this;
    }

    public FSMBuilder<T1, T2> Transit(T2 condition)
    {
        fsm.Transit(condition);
        return this;
    }
    public T GetCurrentState<T>() where T : FSMState<T1>
    {
        return fsm.GetCurrentState<T>();
    }
}
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditModeTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    [Test]
    public void Inherit()
    {
        A a = new B();
        a.print1();
        a.print2();
    }

    [Test]
    public void Test1()
    {
        GameObject player1 = GameObject.Find("Player (1)");
        GameObject player2 = GameObject.Find("Player (2)");
        Debug.Log(player1);
    }

    [Test]
    public void Test2()
    {
        int a = 1;
        int b = 2;
        int c = 0;
        C.Add(a, b, c);
        Debug.Log(c);
    }

    [Test]
    public void Test3()
    {
        Queue<int> a = new Queue<int>();
        a.Enqueue(1);
        a.Enqueue(2);
        a.Enqueue(3);
        a.Enqueue(4);
        a.Enqueue(5);
        Debug.Log(string.Join(", ", a));
        Debug.Log(a.Dequeue());
        Debug.Log(string.Join(", ", a));
    }
}

public class A
{
    public virtual void print1()
    {
        Debug.Log("A1");
    }

    public virtual void print2()
    {
        Debug.Log("A2");
    }
}

public class B : A
{
    public override void print1()
    {
        Debug.Log("B1");
    }
}

public class C
{
    public static void Add(int a, int b, int c)
    {
        c = a + b;
    }
}
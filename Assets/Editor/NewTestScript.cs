using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
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
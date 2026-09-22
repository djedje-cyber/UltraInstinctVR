using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LaunchTest
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator LaunchTestWithEnumeratorPasses()
    {


        LogAssert.ignoreFailingMessages = true;

        yield return SceneManager.LoadSceneAsync("SampleScene");


        Debug.Log($"[TEST] Start wait at {Time.realtimeSinceStartup}, timeScale={Time.timeScale}");

        // laisse ton expérience tourner
        yield return new WaitForSeconds(300f);


        Debug.Log($"[TEST] End wait at {Time.realtimeSinceStartup}");

        Assert.Pass();
    }
}

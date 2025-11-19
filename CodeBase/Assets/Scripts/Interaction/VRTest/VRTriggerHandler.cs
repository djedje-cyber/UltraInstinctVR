using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Class <c>VRTriggerHandler</c> handles VR trigger interactions in Unity.
/// </summary>
public class VRTriggerHandler
{
    Transform transform;
    Dictionary<GameObject, VRTest.ControlInfo> controls;


    /// <summary>
    /// Method <c>VRTriggerHandler</c> initializes a new instance of the VRTriggerHandler class.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="c"></param>

    public VRTriggerHandler(Transform t, Dictionary<GameObject, VRTest.ControlInfo> c)
    {
        transform = t;
        controls = c;
    }


    /// <summary>
    /// Method  <c>FetchControls</c> fetches VR controls and sets up event triggers for them.
    /// </summary>
    /// <param name="objectsFound"></param>
    /// <param name="lastInteractionTime"></param>
    public void FetchControls(ref int objectsFound, ref float lastInteractionTime)
    {
        EnsureDirectoryExists("Logs/CoveredObjects");
        EnsureDirectoryExists("Assets/TESTREPLAY");

        string replayFilePath = GetReplayFilePath();
        string coveredObjectsFilePath = "Logs/FoundObject.txt";

        using (StreamWriter writerCoveredObjects = new StreamWriter(coveredObjectsFilePath, false))
        using (StreamWriter writerReplay = new StreamWriter(replayFilePath, false))
        {
            foreach (GameObject go in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                ProcessGameObject(go, writerCoveredObjects, writerReplay, ref objectsFound, ref lastInteractionTime);
            }
        }
    }

    /// <summary>
    /// Method <c>EnsureDirectoryExists</c> ensures that a directory exists at the specified path.
    /// </summary>
    /// <param name="path"></param>
    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }


    /// <summary>
    /// Method <c>GetReplayFilePath</c> generates a unique file path for the replay file.
    /// </summary>
    /// <returns></returns>
    private string GetReplayFilePath()
    {
        string replayFolderPath = "Assets/TESTREPLAY";
        string uuid = Guid.NewGuid().ToString();
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(replayFolderPath, $"TEST_REPLAY_ObjectFound_{uuid}_{date}.txt");
    }

    /// <summary>
    /// Method <c>ProcessGameObject</c> processes a single GameObject to check for VR grab interactables and sets up event triggers.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="writerCoveredObjects"></param>
    /// <param name="writerReplay"></param>
    /// <param name="objectsFound"></param>
    /// <param name="lastInteractionTime"></param>
    private void ProcessGameObject(GameObject go, StreamWriter writerCoveredObjects, StreamWriter writerReplay,
                                   ref int objectsFound, ref float lastInteractionTime)
    {
        var grabInteractable = go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null || controls.ContainsKey(go))
            return;

        controls[go] = new VRTest.ControlInfo(go);
        string objectInfo = $"{go.name}: {go.transform.position}";
        Debug.Log(objectInfo);
        writerCoveredObjects.WriteLine(objectInfo);
        writerReplay.WriteLine(objectInfo);
        objectsFound++;

        EventTrigger r = go.GetComponent<EventTrigger>() ?? go.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { VRTest.UpdateTrigger(); });
        r.triggers.Add(entry);

        lastInteractionTime = Time.time;
    }

}

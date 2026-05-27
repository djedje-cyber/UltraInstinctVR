using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;

/// <summary>
/// Generates a Xareus-compatible scenario XML from a [TestInteractionClass] annotated class.
/// </summary>
public class PetriNetXmlGenerator
{
    private const string XMLNS = "http://www.insa-rennes.fr/Xareus.Scenarios";
    private const string VERSION = "5.12.0.0";

    // -------------------------------------------------------
    // Entry point
    // -------------------------------------------------------

    public static string Generate<T>(string scenarioId = null, string scenarioLabel = null) where T : class
    {
        Type type = typeof(T);
        scenarioId = scenarioId ?? type.Name;
        scenarioLabel = scenarioLabel ?? type.Name;

        // Collect all methods with [Transition] and [Place]
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        List<TransitionData> transitions = new List<TransitionData>();
        HashSet<int> placeIds = new HashSet<int>();

        int? initialPlaceId = null;
        int? finalPlaceId = null;

        foreach (MethodInfo method in methods)
        {
            TransitionAttribute transAttr = method.GetCustomAttribute<TransitionAttribute>();
            if (transAttr == null) continue;

            PlaceAttribute[] placeAttrs = (PlaceAttribute[])method.GetCustomAttributes(typeof(PlaceAttribute), false);
            SensorAttribute sensorAttr = method.GetCustomAttribute<SensorAttribute>();
            EffectorAttribute effectorAttr = method.GetCustomAttribute<EffectorAttribute>();

            TransitionData td = new TransitionData
            {
                TransitionId = transAttr.Id,
                Label = method.Name,
                UpstreamPlace = transAttr.UpstreamPlace,
                DownstreamPlaces = new List<int>(),
                SensorClass = sensorAttr?.ClassName,
                EffectorClass = effectorAttr?.ClassName,
                SensorParams = sensorAttr?.Params ?? new List<ParamData>(),
                EffectorParams = effectorAttr?.Params ?? new List<ParamData>()
            };

            foreach (PlaceAttribute p in placeAttrs)
            {
                td.DownstreamPlaces.Add(p.Id);
                placeIds.Add(p.Id);
            }

            placeIds.Add(transAttr.UpstreamPlace);
            transitions.Add(td);
        }

        // First upstream place = initial, last downstream place = final
        initialPlaceId = transitions.Count > 0 ? transitions[0].UpstreamPlace : 0;
        finalPlaceId = transitions.Count > 0 ? transitions[transitions.Count - 1].DownstreamPlaces[0] : 1;

        return BuildXml(scenarioId, scenarioLabel, placeIds, transitions, initialPlaceId.Value, finalPlaceId.Value);
    }

    // -------------------------------------------------------
    // XML builder
    // -------------------------------------------------------

    private static string BuildXml(
        string scenarioId,
        string scenarioLabel,
        HashSet<int> placeIds,
        List<TransitionData> transitions,
        int initialPlaceId,
        int finalPlaceId)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine($"<scenario xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"");
        sb.AppendLine($"          xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
        sb.AppendLine($"          id=\"{scenarioId}\"");
        sb.AppendLine($"          label=\"{scenarioLabel}\"");
        sb.AppendLine($"          version=\"{VERSION}\"");
        sb.AppendLine($"          xmlns=\"{XMLNS}\">");

        sb.AppendLine("  <sequence xsi:type=\"SafePetriNet\" id=\"Root\" label=\"Root\">");

        // Places
        int posY = 78;
        foreach (int placeId in placeIds)
        {
            sb.AppendLine($"    <sequence xsi:type=\"Place\" id=\"Place_{placeId}\" label=\"Place_{placeId}\">");
            sb.AppendLine($"      <extendedInfo><unityEditor xmlns=\"\"><position x=\"342\" y=\"{posY}\" /></unityEditor></extendedInfo>");
            sb.AppendLine($"    </sequence>");
            posY += 100;
        }

        // Transitions
        int posX = 520;
        int tPosY = 200;
        foreach (TransitionData td in transitions)
        {
            string tId = $"Transition_{td.TransitionId}";

            sb.AppendLine($"    <transition id=\"{tId}\" label=\"{td.Label}\">");
            sb.AppendLine($"      <extendedInfo><unityEditor xmlns=\"\"><position x=\"{posX}\" y=\"{tPosY}\" /></unityEditor></extendedInfo>");
            sb.AppendLine($"      <event id=\"{tId}\" label=\"{td.Label}\">");

            // Sensor
            if (!string.IsNullOrEmpty(td.SensorClass))
            {
                sb.AppendLine($"        <sensorCheck classname=\"{td.SensorClass}\">");
                foreach (ParamData p in td.SensorParams)
                    sb.AppendLine(BuildParam(p));
                sb.AppendLine($"        </sensorCheck>");
            }

            // Effector
            if (!string.IsNullOrEmpty(td.EffectorClass))
            {
                sb.AppendLine($"        <effectorUpdate classname=\"{td.EffectorClass}\">");
                foreach (ParamData p in td.EffectorParams)
                    sb.AppendLine(BuildParam(p));
                sb.AppendLine($"        </effectorUpdate>");
            }

            sb.AppendLine($"      </event>");

            // Upstream
            sb.AppendLine($"      <upstreamSequence idref=\"Place_{td.UpstreamPlace}\" />");

            // Downstream (one per [Place] annotation)
            foreach (int downId in td.DownstreamPlaces)
                sb.AppendLine($"      <downstreamSequence idref=\"Place_{downId}\" />");

            sb.AppendLine($"    </transition>");
            tPosY += 100;
        }

        // Initial and final
        sb.AppendLine($"    <initialSequence idref=\"Place_{initialPlaceId}\">");
        sb.AppendLine($"      <tokenInit classname=\"Xareus.Scenarios.TokenInit.EmptyTokenInit,Xareus.Scenarios\" />");
        sb.AppendLine($"    </initialSequence>");
        sb.AppendLine($"    <finalSequence idref=\"Place_{finalPlaceId}\" />");

        sb.AppendLine("  </sequence>");
        sb.AppendLine("</scenario>");

        return sb.ToString();
    }

    // -------------------------------------------------------
    // Param builder
    // -------------------------------------------------------

    private static string BuildParam(ParamData p)
    {
        if (p.IsGuid)
            return $"          <param value=\"{p.Value}\" type=\"System.Guid,mscorlib\" name=\"{p.Name}\" />";

        return $"          <param value=\"{p.Value}\" type=\"{p.Type}\" name=\"{p.Name}\" />";
    }

    // -------------------------------------------------------
    // Save to file
    // -------------------------------------------------------

    public static void SaveToFile<T>(string path = null) where T : class
    {
        string xml = Generate<T>();
        string file = path ?? $"Assets/Scenarios/{typeof(T).Name}.xml";

        Directory.CreateDirectory(Path.GetDirectoryName(file));
        File.WriteAllText(file, xml, Encoding.UTF8);

        Debug.Log($"PetriNetXmlGenerator: Scenario saved to {file}");
    }
}
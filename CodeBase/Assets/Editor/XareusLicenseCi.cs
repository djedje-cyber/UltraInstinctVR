using UnityEditor;
using UnityEngine;
using System.IO;
using Xareus;
public static class XareusLicenseSetup
{

    [InitializeOnLoadMethod]

    static void ActivateXareus()
    {
        string licenseContent = System.Environment.GetEnvironmentVariable("XAREUS_LICENSE");
        if (!string.IsNullOrEmpty(licenseContent))
        {

            string tempPath = Path.Combine(Application.temporaryCachePath, "xareus-license.xml");
            File.WriteAllText(tempPath, licenseContent);
            string fileContent = File.ReadAllText(tempPath);
            Xareus.Unity.Licensing.LicenseManager.CheckLicense(fileContent);
            Debug.Log(Xareus.Unity.Licensing.LicenseManager.LicenseOk);
            UnityEngine.Debug.Log("Xareus license written from environment variable.");
        }
        else 
        {
            Debug.LogWarning("XAREUS_LICENSE environment variables is not set. Xareus license not configured.");
        }
    }
}


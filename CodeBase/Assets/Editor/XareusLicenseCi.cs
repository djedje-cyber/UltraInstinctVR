

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.IO;
using Xareus;




/// <summary>
/// Class <c>XareusLicenseSetup</c> sets up the Xareus license from an environment variable for Unity Editor.
/// </summary>

public static class XareusLicenseSetup
{

    /// <summary>
    /// Method <c>ActivateXareus</c> reads the XAREUS_LICENSE environment variable, writes its content to a temporary file and activate the license,
    /// </summary>

    [InitializeOnLoadMethod]
    static void XareusLicenseLoader()
    {
    #if UNITY_EDITOR
        // Skip UI dialogs if running in batchmode (CI)
        if (Application.isBatchMode)
        {
            Debug.Log("Batchmode detected — skipping license dialogs");

            ActivateXareus();
            return;
        }


    #endif
    }









    public static void ActivateXareus()
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


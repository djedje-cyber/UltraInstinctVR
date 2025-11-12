using UnityEditor;
using UnityEngine;
using System.IO;
using Xareus.Unity.Licensing;
[InitializeOnLoad]
public class XareusLicenseSetup
{
    static XareusLicenseSetup()
    {
        string licenseContent = System.Environment.GetEnvironmentVariable("XAREUS_LICENSE");
        Debug.Log(licenseContent);
        if (!string.IsNullOrEmpty(licenseContent))
        {
            Xareus.Unity.Licensing.LicenseManager.CheckLicense(licenseContent);
            UnityEngine.Debug.Log("Xareus license written from environment variable.");
        }
        else
        {
            Debug.LogWarning("XAREUS_LICENSE environment variables is not set. Xareus license not configured.");
        }
    }
}


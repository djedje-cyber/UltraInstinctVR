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
        if (!string.IsNullOrEmpty(licenseContent))
        {
            Xareus.Unity.Licensing.LicenseManager.CheckLicense(licenseContent);
            UnityEngine.Debug.Log("Xareus license written from environment variable.");
        }
        
    }
}


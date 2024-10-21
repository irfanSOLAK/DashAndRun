using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProcessor : IPreprocessBuildWithReport
{
    // Build iþleminden önce çalýþtýrýlacak öncelik sýrasýný belirler
    public int callbackOrder => -1;

    // Build iþleminden önce çalýþacak fonksiyon
    public void OnPreprocessBuild(BuildReport report)
    {
        // AdConfiguration instance'ýný yükle
        var adConfig = AdConfiguration.LoadInstance();

        // Update settings based on the user's choice
        AdConfigurationEditor.UpdateSettings(adConfig);

        // Koþullarý kontrol et
        if (!CheckBuildConditions())
        {
            // Koþullar saðlanmadý, kullanýcýdan onay iste
            int userChoice = EditorUtility.DisplayDialogComplex(
                "Test IDs Selected",
                "The build is configured to use test IDs. Do you want to continue with test IDs or switch to real IDs?",
                "Use Test IDs",
                "Use Real IDs",
                "Cancel");

            switch (userChoice)
            {
                case 0: // Use Test IDs
                    // Do nothing, proceed with test IDs
                    break;

                case 1: // Use Real IDs
                    adConfig.useTestIds = false;
                    break;

                case 2: // Cancel
                    throw new BuildFailedException("Build process canceled by the user.");
            }
        }
    }

    // Koþullarý kontrol eden fonksiyon
    private bool CheckBuildConditions()
    {
        // Buraya kendi koþullarýnýzý ekleyin
        // Örneðin:
        return !AdConfiguration.LoadInstance().useTestIds;
    }
}

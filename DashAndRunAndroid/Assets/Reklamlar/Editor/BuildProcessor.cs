using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProcessor : IPreprocessBuildWithReport
{
    // Build i�leminden �nce �al��t�r�lacak �ncelik s�ras�n� belirler
    public int callbackOrder => -1;

    // Build i�leminden �nce �al��acak fonksiyon
    public void OnPreprocessBuild(BuildReport report)
    {
        // AdConfiguration instance'�n� y�kle
        var adConfig = AdConfiguration.LoadInstance();

        // Update settings based on the user's choice
        AdConfigurationEditor.UpdateSettings(adConfig);

        // Ko�ullar� kontrol et
        if (!CheckBuildConditions())
        {
            // Ko�ullar sa�lanmad�, kullan�c�dan onay iste
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

    // Ko�ullar� kontrol eden fonksiyon
    private bool CheckBuildConditions()
    {
        // Buraya kendi ko�ullar�n�z� ekleyin
        // �rne�in:
        return !AdConfiguration.LoadInstance().useTestIds;
    }
}

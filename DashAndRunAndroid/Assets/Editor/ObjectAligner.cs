using UnityEditor;
using UnityEngine;
using System.Linq;

public class ObjectAligner : EditorWindow
{
    // Modlarý temsil eden enum
    private enum AlignmentMode
    {
        Distribute,  // Nesneleri hizalama
        ApplyOffset  // Nesnelere ofset uygulama
    }

    private AlignmentMode alignmentMode = AlignmentMode.Distribute; // Varsayýlan mod hizalama
    private Vector3 firstObjectPosition;  // Ýlk nesnenin konumu
    private Vector3 lastObjectPosition;   // Son nesnenin konumu
    private Vector3 offsetToApply;        // Eklenmek için kullanýlan ofset
    private int numberOfSelectedObjects;  // Seçilen nesne sayýsý
    private Transform[] orderedTransforms; // Sýralanmýþ nesneler

    [MenuItem("GameObject/My Settings/Align Selected Objects")]
    private static void ShowWindow()
    {
        var window = GetWindow<ObjectAligner>("Object Aligner");
        window.InitializeValues();
    }

    private void OnGUI()
    {
        GUILayout.Label("Object Alignment Tool", EditorStyles.boldLabel);

        // Mod seçim menüsü
        alignmentMode = (AlignmentMode)EditorGUILayout.EnumPopup("Alignment Mode", alignmentMode);

        // Seçilen nesne sayýsýný göster
        GUILayout.Label($"Selected Objects: {numberOfSelectedObjects}", EditorStyles.label);

        // Baþlangýç ve bitiþ noktalarýný veya ofseti göster
        if (alignmentMode == AlignmentMode.Distribute)
        {
            firstObjectPosition = EditorGUILayout.Vector3Field("First Object Position", firstObjectPosition);
            lastObjectPosition = EditorGUILayout.Vector3Field("Last Object Position", lastObjectPosition);
        }
        else if (alignmentMode == AlignmentMode.ApplyOffset)
        {
            firstObjectPosition = EditorGUILayout.Vector3Field("Base Position", firstObjectPosition);
            offsetToApply = EditorGUILayout.Vector3Field("Offset Per Object", offsetToApply);
        }

        // Buton: Ýþlevine göre metni ve fonksiyonu ayarla
        if (GUILayout.Button(alignmentMode == AlignmentMode.ApplyOffset ? "Apply Offset" : "Distribute Objects"))
        {
            if (alignmentMode == AlignmentMode.ApplyOffset)
            {
                ApplyOffsetToObjects();
            }
            else
            {
                DistributeObjectsEvenly();
            }
        }
    }

    private void OnSelectionChange()
    {
        UpdateObjectSelection();
        Repaint(); // Pencereyi yenile
    }

    private void InitializeValues()
    {
        firstObjectPosition = Vector3.zero; // Baþlangýç
        lastObjectPosition = Vector3.zero;  // Bitiþ
        offsetToApply = Vector3.zero;       // Ofset sýfýrla
        UpdateObjectSelection(); // Seçili objeleri güncelle
    }

    private void UpdateObjectSelection()
    {
        numberOfSelectedObjects = Selection.transforms.Length;
        SetInitialAndFinalPositions(); // Seçilen objelerin sýralý listesiyle noktalarý güncelle
    }

    private void SetInitialAndFinalPositions()
    {
        if (numberOfSelectedObjects > 0)
        {
            // Seçili objeleri hiyerarþi sýrasýna göre sýralama
            orderedTransforms = Selection.transforms.OrderBy(t => t.GetSiblingIndex()).ToArray();

            // Ýlk ve son objelerin konumlarýný ayarla
            firstObjectPosition = orderedTransforms.First().position;
            lastObjectPosition = orderedTransforms.Last().position;
        }
    }

    private void DistributeObjectsEvenly()
    {
        if (numberOfSelectedObjects < 2)
        {
            Debug.LogWarning("At least two objects must be selected for distribution.");
            return;
        }

        // Undo kaydý oluþtur
        Undo.RecordObjects(orderedTransforms, "Distribute Objects");

        // Aralýðý hesapla
        Vector3 spacing = lastObjectPosition - firstObjectPosition;
        float intervalX = spacing.x / (orderedTransforms.Length - 1);
        float intervalY = spacing.y / (orderedTransforms.Length - 1);
        float intervalZ = spacing.z / (orderedTransforms.Length - 1);

        // Nesneleri daðýt
        for (int i = 0; i < orderedTransforms.Length; i++)
        {
            Transform obj = orderedTransforms[i];
            obj.position = new Vector3(
                firstObjectPosition.x + (intervalX * i),
                firstObjectPosition.y + (intervalY * i),
                firstObjectPosition.z + (intervalZ * i)
            );
        }

        // Sahneyi güncelle
        SceneView.RepaintAll();
    }

    private void ApplyOffsetToObjects()
    {
        if (numberOfSelectedObjects < 1)
        {
            Debug.LogWarning("At least one object must be selected to apply an offset.");
            return;
        }

        // Undo kaydý oluþtur
        Undo.RecordObjects(orderedTransforms, "Apply Offset");

        // Ofset uygula
        for (int i = 0; i < orderedTransforms.Length; i++)
        {
            Transform obj = orderedTransforms[i];
            obj.position = firstObjectPosition + new Vector3(offsetToApply.x * i, offsetToApply.y * i, offsetToApply.z * i);
        }

        // Sahneyi güncelle
        SceneView.RepaintAll();
    }
}

using UnityEditor;
using UnityEngine;
using System.Linq;

public class ObjectAligner : EditorWindow
{
    // Modlar� temsil eden enum
    private enum AlignmentMode
    {
        Distribute,  // Nesneleri hizalama
        ApplyOffset  // Nesnelere ofset uygulama
    }

    private AlignmentMode alignmentMode = AlignmentMode.Distribute; // Varsay�lan mod hizalama
    private Vector3 firstObjectPosition;  // �lk nesnenin konumu
    private Vector3 lastObjectPosition;   // Son nesnenin konumu
    private Vector3 offsetToApply;        // Eklenmek i�in kullan�lan ofset
    private int numberOfSelectedObjects;  // Se�ilen nesne say�s�
    private Transform[] orderedTransforms; // S�ralanm�� nesneler

    [MenuItem("GameObject/My Settings/Align Selected Objects")]
    private static void ShowWindow()
    {
        var window = GetWindow<ObjectAligner>("Object Aligner");
        window.InitializeValues();
    }

    private void OnGUI()
    {
        GUILayout.Label("Object Alignment Tool", EditorStyles.boldLabel);

        // Mod se�im men�s�
        alignmentMode = (AlignmentMode)EditorGUILayout.EnumPopup("Alignment Mode", alignmentMode);

        // Se�ilen nesne say�s�n� g�ster
        GUILayout.Label($"Selected Objects: {numberOfSelectedObjects}", EditorStyles.label);

        // Ba�lang�� ve biti� noktalar�n� veya ofseti g�ster
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

        // Buton: ��levine g�re metni ve fonksiyonu ayarla
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
        firstObjectPosition = Vector3.zero; // Ba�lang��
        lastObjectPosition = Vector3.zero;  // Biti�
        offsetToApply = Vector3.zero;       // Ofset s�f�rla
        UpdateObjectSelection(); // Se�ili objeleri g�ncelle
    }

    private void UpdateObjectSelection()
    {
        numberOfSelectedObjects = Selection.transforms.Length;
        SetInitialAndFinalPositions(); // Se�ilen objelerin s�ral� listesiyle noktalar� g�ncelle
    }

    private void SetInitialAndFinalPositions()
    {
        if (numberOfSelectedObjects > 0)
        {
            // Se�ili objeleri hiyerar�i s�ras�na g�re s�ralama
            orderedTransforms = Selection.transforms.OrderBy(t => t.GetSiblingIndex()).ToArray();

            // �lk ve son objelerin konumlar�n� ayarla
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

        // Undo kayd� olu�tur
        Undo.RecordObjects(orderedTransforms, "Distribute Objects");

        // Aral��� hesapla
        Vector3 spacing = lastObjectPosition - firstObjectPosition;
        float intervalX = spacing.x / (orderedTransforms.Length - 1);
        float intervalY = spacing.y / (orderedTransforms.Length - 1);
        float intervalZ = spacing.z / (orderedTransforms.Length - 1);

        // Nesneleri da��t
        for (int i = 0; i < orderedTransforms.Length; i++)
        {
            Transform obj = orderedTransforms[i];
            obj.position = new Vector3(
                firstObjectPosition.x + (intervalX * i),
                firstObjectPosition.y + (intervalY * i),
                firstObjectPosition.z + (intervalZ * i)
            );
        }

        // Sahneyi g�ncelle
        SceneView.RepaintAll();
    }

    private void ApplyOffsetToObjects()
    {
        if (numberOfSelectedObjects < 1)
        {
            Debug.LogWarning("At least one object must be selected to apply an offset.");
            return;
        }

        // Undo kayd� olu�tur
        Undo.RecordObjects(orderedTransforms, "Apply Offset");

        // Ofset uygula
        for (int i = 0; i < orderedTransforms.Length; i++)
        {
            Transform obj = orderedTransforms[i];
            obj.position = firstObjectPosition + new Vector3(offsetToApply.x * i, offsetToApply.y * i, offsetToApply.z * i);
        }

        // Sahneyi g�ncelle
        SceneView.RepaintAll();
    }
}

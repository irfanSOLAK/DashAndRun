using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RenameMultipleObjectsWithNumbers : ScriptableWizard
{
    // Base name
    public string BaseName;
    // Start Count
    public int StartNumber = 1;
    // Increment
    public int Increment = 1;

    [MenuItem("GameObject/My Settings/Rename Multiple Objects With Numbers &R", priority = 20)]
    static void CreateWizard()
    {
        if (GetWindow<RenameMultipleObjectsWithNumbers>() == null)
        {
            DisplayWizard<RenameMultipleObjectsWithNumbers>("Rename Multiple Objects With Numbers", "Rename");
        }
    }

    void OnEnable()
    {
        UpdateSelectionHelper();
        SetBaseNameFromFirstSelectedObject();
    }

    void OnSelectionChange()
    {
        UpdateSelectionHelper();
        SetBaseNameFromFirstSelectedObject();
    }

    void UpdateSelectionHelper()
    {
        helpString = $"Number of objects selected: {Selection.objects.Length}";
    }

    void OnWizardCreate()
    {
        RenameObjects();
    }

    private void RenameObjects()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected to rename.");
            return;
        }

        Undo.SetCurrentGroupName("Rename Multiple Objects");
        int undoGroup = Undo.GetCurrentGroup();

        int postFix = StartNumber;
        foreach (Object obj in selectedObjects)
        {
            Undo.RecordObject(obj, "Rename Object");
            obj.name = BaseName + postFix;
            postFix += Increment;
        }

        Undo.CollapseUndoOperations(undoGroup);
    }

    private void SetBaseNameFromFirstSelectedObject()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length > 0 && selectedObjects[0] is GameObject firstObject)
        {
            BaseName = firstObject.name;
        }
    }
}

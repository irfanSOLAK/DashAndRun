using UnityEditor;
using UnityEngine;

public class MoveSelectedToClickPosition : Editor
{
    private static bool isActive = false;

    [MenuItem("GameObject/My Settings/Move Selected To Click Position &S")] // %g = Ctrl+G
    private static void MoveSelected()
    {
        isActive = true; // Aktif hale getir
        SceneView.duringSceneGui += OnSceneGUI; // Dinleyiciyi ekle
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (isActive && e.type == EventType.MouseDown && e.button == 0) // Sol fare tu�una t�klama
        {
            Vector3 clickPosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).GetPoint(10f); // T�klanan pozisyon
            MoveObjectsToPosition(clickPosition);
            isActive = false; // Aktif durumu kapat
            SceneView.duringSceneGui -= OnSceneGUI; // Dinleyiciyi kald�r
        }
    }

    private static void MoveObjectsToPosition(Vector3 clickPosition)
    {
        foreach (Transform t in Selection.transforms)
        {
            Undo.RecordObject(t, "Move Object"); // Undo kayd� olu�tur
            t.position = new Vector3(clickPosition.x, clickPosition.y, t.position.z); // X ve Y konumlar�n� de�i�tir, Z sabit kals�n
        }
    }
}

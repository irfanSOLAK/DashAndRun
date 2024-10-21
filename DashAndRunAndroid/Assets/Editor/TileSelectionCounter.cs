using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[InitializeOnLoad]
public class TileSelectionCounter : Editor
{
    private static Vector3Int selectionStart;
    private static Vector3Int selectionEnd;
    private static bool isSelecting = false;

    private static readonly int LabelFontSize = 16;
    private static readonly Color LabelTextColor = Color.white;
    private static readonly Color SelectionBoxColor = Color.red;

    static TileSelectionCounter()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Tilemap activeTilemap = GetActiveTilemap();
        if (activeTilemap == null) return;

        HandleSelectionInput(activeTilemap, Event.current, sceneView);

        if (isSelecting)
        {
            DisplaySelection(activeTilemap);
        }
    }

    private static Tilemap GetActiveTilemap()
    {
        return FindObjectOfType<Tilemap>();
    }

    private static void HandleSelectionInput(Tilemap tilemap, Event currentEvent, SceneView sceneView)
    {
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            StartSelection(tilemap, currentEvent.mousePosition);
        }
        else if (currentEvent.type == EventType.MouseDrag && currentEvent.button == 0 && isSelecting)
        {
            UpdateSelectionEnd(tilemap, currentEvent.mousePosition);
            sceneView.Repaint();
        }
        else if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0 && isSelecting)
        {
            EndSelection();
            SceneView.RepaintAll();
        }
    }

    private static void StartSelection(Tilemap tilemap, Vector2 mousePosition)
    {
        isSelecting = true;
        selectionStart = GetMouseGridPosition(tilemap, mousePosition);
        selectionEnd = selectionStart;
        SceneView.RepaintAll();
    }

    private static void UpdateSelectionEnd(Tilemap tilemap, Vector2 mousePosition)
    {
        selectionEnd = GetMouseGridPosition(tilemap, mousePosition);
    }

    private static void EndSelection()
    {
        isSelecting = false;
    }

    private static Vector3Int GetMouseGridPosition(Tilemap tilemap, Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        return tilemap.WorldToCell(ray.origin);
    }

    private static void DisplaySelection(Tilemap tilemap)
    {
        int horizontalCount = Mathf.Abs(selectionEnd.x - selectionStart.x) + 1;
        int verticalCount = Mathf.Abs(selectionEnd.y - selectionStart.y) + 1;

        DrawSelectionBox(tilemap);
        DisplayGridCount(tilemap, horizontalCount, verticalCount);
    }

    private static void DrawSelectionBox(Tilemap tilemap)
    {
        Vector3 minPosition = tilemap.CellToWorld(new Vector3Int(Mathf.Min(selectionStart.x, selectionEnd.x), Mathf.Min(selectionStart.y, selectionEnd.y), 0));
        Vector3 maxPosition = tilemap.CellToWorld(new Vector3Int(Mathf.Max(selectionStart.x, selectionEnd.x) + 1, Mathf.Max(selectionStart.y, selectionEnd.y) + 1, 0));

        Handles.color = SelectionBoxColor;
        Handles.DrawWireCube((minPosition + maxPosition) / 2, maxPosition - minPosition);
    }

    private static void DisplayGridCount(Tilemap tilemap, int horizontalCount, int verticalCount)
    {
        GUIStyle labelStyle = CreateLabelStyle();
        DisplayLabel(tilemap, horizontalCount, verticalCount, labelStyle);
    }

    private static void DisplayLabel(Tilemap tilemap, int horizontalCount, int verticalCount, GUIStyle labelStyle)
    {
        Vector3 topCenter = CalculateTopLabelPosition(tilemap);
        Vector3 leftCenter = CalculateLeftLabelPosition(tilemap);

        Handles.Label(topCenter + new Vector3(0.5f, 1.6f, 0), horizontalCount.ToString(), labelStyle);
        Handles.Label(leftCenter + new Vector3(-0.4f, 0.7f, 0), verticalCount.ToString(), labelStyle);
    }

    private static GUIStyle CreateLabelStyle()
    {
        return new GUIStyle
        {
            fontSize = LabelFontSize,
            normal = { textColor = LabelTextColor },
            alignment = TextAnchor.MiddleCenter
        };
    }

    private static Vector3 CalculateTopLabelPosition(Tilemap tilemap)
    {
        return (tilemap.CellToWorld(new Vector3Int(Mathf.Min(selectionStart.x, selectionEnd.x), Mathf.Max(selectionStart.y, selectionEnd.y), 0)) +
                tilemap.CellToWorld(new Vector3Int(Mathf.Max(selectionStart.x, selectionEnd.x), Mathf.Max(selectionStart.y, selectionEnd.y), 0))) / 2;
    }

    private static Vector3 CalculateLeftLabelPosition(Tilemap tilemap)
    {
        return (tilemap.CellToWorld(new Vector3Int(Mathf.Min(selectionStart.x, selectionEnd.x), Mathf.Min(selectionStart.y, selectionEnd.y), 0)) +
                tilemap.CellToWorld(new Vector3Int(Mathf.Min(selectionStart.x, selectionEnd.x), Mathf.Max(selectionStart.y, selectionEnd.y), 0))) / 2;
    }
}

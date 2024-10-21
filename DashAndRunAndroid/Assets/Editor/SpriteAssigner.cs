using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SpriteAssigner : EditorWindow
{
    public GameObject[] gameObjects; // SpriteRenderer veya Image bileþenine sahip objeler dizisi
    public Sprite[] sprites; // Atanacak sprite dizisi

    private static SpriteAssigner window;

    [MenuItem("GameObject/My Settings/Assign Sprites to GameObjects", false, 20)]
    static void ShowWindow()
    {
        // Pencereyi oluþtur veya varsa mevcut pencereyi kullan
        window = GetWindow<SpriteAssigner>("Assign Sprites to GameObjects");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Assign Sprites to GameObjects", EditorStyles.boldLabel);

        // GameObject dizisi için UI oluþturma
        EditorGUILayout.LabelField("GameObjects");
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty gameObjectsProperty = serializedObject.FindProperty("gameObjects");
        EditorGUILayout.PropertyField(gameObjectsProperty, true);

        // Sprite dizisi için UI oluþturma
        EditorGUILayout.LabelField("Sprites");
        SerializedProperty spritesProperty = serializedObject.FindProperty("sprites");
        EditorGUILayout.PropertyField(spritesProperty, true);

        // Pencereyi güncelle
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Assign Sprites"))
        {
            AssignSpritesToGameObjects();
            Close(); // Pencereyi kapat
        }
    }

    private void AssignSpritesToGameObjects()
    {
        if (gameObjects == null || sprites == null)
        {
            Debug.LogError("GameObjects or sprites are missing");
            return;
        }

        if (gameObjects.Length != sprites.Length)
        {
            Debug.LogError("The number of game objects is not equal to the number of sprites.");
            return;
        }

        // Undo iþlemi baþlat
        Undo.SetCurrentGroupName("Assign Sprites to GameObjects");
        int undoGroup = Undo.GetCurrentGroup();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject obj = gameObjects[i];
            if (obj == null)
            {
                Debug.LogError($"GameObject at index {i} is null.");
                continue;
            }

            // SpriteRenderer bileþenini kontrol et
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Undo.RecordObject(spriteRenderer, "Change Sprite");
                spriteRenderer.sprite = sprites[i];
                EditorUtility.SetDirty(spriteRenderer);
                continue;
            }

            // Image bileþenini kontrol et
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    Undo.RecordObject(buttonImage, "Change Button Sprite");
                    buttonImage.sprite = sprites[i];
                    EditorUtility.SetDirty(buttonImage);
                }
                else
                {
                    Debug.LogError($"Button at index {i} does not have an Image component.");
                }
            }
            else
            {
                Debug.LogError($"GameObject at index {i} does not have a SpriteRenderer or Button component.");
            }
        }

        // Undo iþlemini sonlandýr
        Undo.CollapseUndoOperations(undoGroup);

        // Canvas güncellemelerini zorla
        Canvas.ForceUpdateCanvases();

        Debug.Log("Sprites assigned to game objects successfully.");
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabData))]
public class PrefabDataDrawer : PropertyDrawer
{
    private static HashSet<int> existingIDs = new HashSet<int>(); // Benzersiz ID'ler i�in koleksiyon
    private static int idLength = 5; // Varsay�lan ID uzunlu�u
    private GUIStyle italicGreyMiniLabel; // Stil de�i�keni

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty idProp = property.FindPropertyRelative("id");
        SerializedProperty prefabProp = property.FindPropertyRelative("prefab");
        SerializedProperty descriptionProp = property.FindPropertyRelative("description");

        // Stil tan�m�n� burada yap
        italicGreyMiniLabel = new GUIStyle(EditorStyles.miniLabel)
        {
            normal = { textColor = Color.grey },
            fontStyle = FontStyle.Italic
        };

        EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        // Geni�lik ayarlar�
        float totalWidth = contentPosition.width;
        float idWidth = totalWidth * 0.2f; 
        float prefabWidth = totalWidth * 0.45f;
        float descriptionWidth = totalWidth * 0.5f; 

        // ID alan�
        Rect idRect = new Rect(contentPosition.x-45, contentPosition.y, idWidth, contentPosition.height);
        EditorGUI.PropertyField(idRect, idProp, GUIContent.none);

        // E�er ID mevcut de�ilse ve hala atanmad�ysa, benzersiz bir ID olu�tur
        if (idProp.intValue <= 0)
        {
            GUI.Label(idRect, "Enter ID here...", italicGreyMiniLabel);
            // Benzersiz ID'yi olu�tur ve ata
            int newID = GenerateUniqueRandomID(idLength);
            idProp.intValue = newID; // ID'yi atama
            existingIDs.Add(newID); // Yeni ID'yi ekle

            // Prefab'a ID'yi atama
            if (prefabProp.objectReferenceValue != null)
            {
                GameObject prefabInstance = prefabProp.objectReferenceValue as GameObject;
                if (prefabInstance != null)
                {
                    PrefabIdentifier prefabIdentifier = prefabInstance.GetComponent<PrefabIdentifier>();
                    if (prefabIdentifier == null)
                    {
                        prefabIdentifier = prefabInstance.AddComponent<PrefabIdentifier>();
                    }
                    prefabIdentifier.id = newID; // ID'yi prefab'a ata
                }
            }
        }
        else
        {
            existingIDs.Add(idProp.intValue); // Mevcut ID'yi ekle
        }

        // Prefab alan�
        Rect prefabRect = new Rect(contentPosition.x + idWidth + 5-45, contentPosition.y, prefabWidth, contentPosition.height);
        EditorGUI.PropertyField(prefabRect, prefabProp, GUIContent.none);

        // A��klama alan�
        Rect descriptionRect = new Rect(contentPosition.x + idWidth + prefabWidth + 10-45, contentPosition.y, descriptionWidth, contentPosition.height);
        EditorGUI.PropertyField(descriptionRect, descriptionProp, GUIContent.none);

        // Varsay�lan a��klama metnini g�stermek i�in
        if (string.IsNullOrEmpty(descriptionProp.stringValue))
        {
            GUI.Label(descriptionRect, "Write Description here...", italicGreyMiniLabel);
        }

        EditorGUI.EndProperty();
    }

    private int GenerateUniqueRandomID(int length)
    {
        int id;
        do
        {
            id = Random.Range(1, (int)Mathf.Pow(10, length)); // Belirtilen uzunlukta rastgele bir ID
        } while (existingIDs.Contains(id)); // E�er mevcut ID'lerle �ak���yorsa tekrar dene

        return id; // Yeni ID'yi d�nd�r
    }
}

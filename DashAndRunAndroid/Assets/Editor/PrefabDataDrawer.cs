using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabData))]
public class PrefabDataDrawer : PropertyDrawer
{
    private static HashSet<int> existingIDs = new HashSet<int>(); // Benzersiz ID'ler için koleksiyon
    private static int idLength = 5; // Varsayýlan ID uzunluðu
    private GUIStyle italicGreyMiniLabel; // Stil deðiþkeni

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty idProp = property.FindPropertyRelative("id");
        SerializedProperty prefabProp = property.FindPropertyRelative("prefab");
        SerializedProperty descriptionProp = property.FindPropertyRelative("description");

        // Stil tanýmýný burada yap
        italicGreyMiniLabel = new GUIStyle(EditorStyles.miniLabel)
        {
            normal = { textColor = Color.grey },
            fontStyle = FontStyle.Italic
        };

        EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        // Geniþlik ayarlarý
        float totalWidth = contentPosition.width;
        float idWidth = totalWidth * 0.2f; 
        float prefabWidth = totalWidth * 0.45f;
        float descriptionWidth = totalWidth * 0.5f; 

        // ID alaný
        Rect idRect = new Rect(contentPosition.x-45, contentPosition.y, idWidth, contentPosition.height);
        EditorGUI.PropertyField(idRect, idProp, GUIContent.none);

        // Eðer ID mevcut deðilse ve hala atanmadýysa, benzersiz bir ID oluþtur
        if (idProp.intValue <= 0)
        {
            GUI.Label(idRect, "Enter ID here...", italicGreyMiniLabel);
            // Benzersiz ID'yi oluþtur ve ata
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

        // Prefab alaný
        Rect prefabRect = new Rect(contentPosition.x + idWidth + 5-45, contentPosition.y, prefabWidth, contentPosition.height);
        EditorGUI.PropertyField(prefabRect, prefabProp, GUIContent.none);

        // Açýklama alaný
        Rect descriptionRect = new Rect(contentPosition.x + idWidth + prefabWidth + 10-45, contentPosition.y, descriptionWidth, contentPosition.height);
        EditorGUI.PropertyField(descriptionRect, descriptionProp, GUIContent.none);

        // Varsayýlan açýklama metnini göstermek için
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
        } while (existingIDs.Contains(id)); // Eðer mevcut ID'lerle çakýþýyorsa tekrar dene

        return id; // Yeni ID'yi döndür
    }
}

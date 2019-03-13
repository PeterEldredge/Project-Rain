using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(Collectable)), CanEditMultipleObjects]
public class UsableTriggersEditor : Editor {
	private SerializedProperty CollectableTypeProp;
	private SerializedProperty DocumentProp, ItemProp, JournalProp;

	private void OnEnable()
	{
		CollectableTypeProp = serializedObject.FindProperty("collectableType");

		DocumentProp = serializedObject.FindProperty("document");
		ItemProp = serializedObject.FindProperty("item");
		JournalProp = serializedObject.FindProperty("journal");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		HandleCollectableGUI();

		GUILayout.Space(10);

		serializedObject.ApplyModifiedProperties();
	}

	private void HandleCollectableGUI()
	{
		EditorGUILayout.PropertyField(CollectableTypeProp, new GUIContent("CollectableType"));

		Collectable.CollectableType collectableType = (Collectable.CollectableType) CollectableTypeProp.intValue;

		switch(collectableType)
		{
			case Collectable.CollectableType.Document:
				EditorGUILayout.PropertyField(DocumentProp, new GUIContent("Document"));
				break;
			case Collectable.CollectableType.Item:
				EditorGUILayout.PropertyField(ItemProp, new GUIContent("Item"));
				break;
			case Collectable.CollectableType.Journal:
				EditorGUILayout.PropertyField(JournalProp, new GUIContent("Journal"));
				break;
		}
	}
}

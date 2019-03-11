using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(UsableTriggers)), CanEditMultipleObjects]
public class UsableTriggersEditor : Editor {
	/* USE AS EXAMPLE
	private SerializedProperty ObjectTypeProp, CollectableTypeProp;
	private SerializedProperty DocumentProp, ItemProp, JournalProp;
	private SerializedProperty OnUsedProp;

	private void OnEnable()
	{
		ObjectTypeProp = serializedObject.FindProperty("objectType");
		CollectableTypeProp = serializedObject.FindProperty("collectableType");

		DocumentProp = serializedObject.FindProperty("document");
		ItemProp = serializedObject.FindProperty("item");
		JournalProp = serializedObject.FindProperty("journal");

		OnUsedProp = serializedObject.FindProperty("onUsed");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(ObjectTypeProp);

		UsableTriggers.ObjectType objectType = (UsableTriggers.ObjectType) ObjectTypeProp.intValue;

		switch(objectType)
		{
			case UsableTriggers.ObjectType.Collectable:
				HandleCollectableGUI();
				break;
			default:
				break;
		}

		GUILayout.Space(10);
		EditorGUILayout.PropertyField(OnUsedProp, new GUIContent("OnUsed"));

		serializedObject.ApplyModifiedProperties();
	}

	private void HandleCollectableGUI()
	{
		EditorGUILayout.PropertyField(CollectableTypeProp, new GUIContent("CollectableType"));

		UsableTriggers.CollectableType collectableType = (UsableTriggers.CollectableType) CollectableTypeProp.intValue;

		switch(collectableType)
		{
			case UsableTriggers.CollectableType.Document:
				EditorGUILayout.PropertyField(DocumentProp, new GUIContent("Document"));
				break;
			case UsableTriggers.CollectableType.Item:
				EditorGUILayout.PropertyField(ItemProp, new GUIContent("Item"));
				break;
			case UsableTriggers.CollectableType.Journal:
				EditorGUILayout.PropertyField(JournalProp, new GUIContent("Journal"));
				break;
		}
	}
	*/
}

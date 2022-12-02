using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Menus
{
    /*
    private const string MANAGER_PATH = "Assets/Resources/Editor Prefabs.asset";

    private static void SafeInstantiate(Func<EditorPrefabManager, GameObject> itemSelector)
    {
        EditorPrefabManager prefabs = AssetDatabase.LoadAssetAtPath<EditorPrefabManager>(MANAGER_PATH);

        if (!prefabs)
        {
            Debug.LogWarning("EditorPrefabManager not found at " + MANAGER_PATH);
            return;
        }

        var item = itemSelector(prefabs);
        var instance = PrefabUtility.InstantiatePrefab(item, Selection.activeTransform);

        Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
        Selection.activeObject = instance;
    }

    [MenuItem(Constants.MENU_ITEM_PATH + "UI/Tab Menu", priority = -50)]
    private static void CreateTabMenu()
    {
        SafeInstantiate(prefabManager => prefabManager.TabMenu);
    }

    [MenuItem(Constants.MENU_ITEM_PATH + "UI/Tab Menu", true)]
    private static bool SelectionHasCanvasValidate() =>
    Selection.activeGameObject && Selection.activeGameObject.GetComponentInParent<Canvas>();
    */
}

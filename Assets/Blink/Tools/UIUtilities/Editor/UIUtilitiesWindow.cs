using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BLINK.UIUtilities
{
    public class UIUtilitiesWindow : EditorWindow
    {
        private ScriptableObject _scriptableObj;
        private SerializedObject _serialObj;
        private GUISkin _skin;
        private UIUtilitiesEditorData _editorData;

        public GameObject[] gameObjectParents;
        public string selectionNameValue;

        private Vector2 _viewScrollPosition;
        
        private enum Categories
        {
            Home = 0,
            Utilities = 1
        }

        private Categories _currentCategory;
        
        private enum ToolCategory
        {
            Select = 0,
            Rename = 1,
        }

        private ToolCategory _currentToolCategory;
        
        private enum SelectionType
        {
            Component = 0,
            Name = 1
        }

        private SelectionType _currentSelectionType;
        
        private enum SelectionNameType
        {
            Contains = 0,
            DoNotContain = 1,
            Equal = 2,
            DoNotEqual = 3,
            StartWith = 4,
            DoNotStartWith = 5,
            EndWith = 6,
            DoNotEndWith = 7,
        }

        private SelectionNameType _currentSelectionNameType;
        
        private enum SelectionComponentType
        {
            Text = 0,
            TextTMP = 1,
            Image = 2,
            RawImage = 3,
            Button = 4,
            Toggle = 5,
            Slider = 6,
            Scrollbar = 7,
            Dropdown = 8,
            DropdownTMP = 9,
            InputField = 10,
            InputFieldTMP = 11,
            Canvas = 12,
            ScrollView = 13,
            GameObject = 14
        }

        private SelectionComponentType _currentSelectionComponentType;
        
        private enum RenamingType
        {
            ReplaceCompletely = 0,
            ReplaceInName = 1,
            AddStart = 2,
            AddEnd = 3,
            RemoveStart = 4,
            RemoveEnd = 5,
            NumberStart = 6,
            NumberEnd = 7,
        }

        private RenamingType _currentRenamingType;
        public string renameTextValue, renameTextValue2;
        public string numberTextPrefix, numberTextSuffix;
        public int removeCharacterCount = 1, numberRenameStartAt = 1;
        
        [MenuItem("BLINK/UI Utilities")]
        private static void OpenWindow()
        {
            var window = (UIUtilitiesWindow) GetWindow(typeof(UIUtilitiesWindow), false,"Blink UI Utilities");
            window.minSize = new Vector2(300, 300);
            GUI.contentColor = Color.white;
            window.Show();
        }

        private void OnGUI()
        {
            if (_skin == null) return;
            DrawMainWindow();
        }

        private void OnEnable()
        {
            _scriptableObj = this;
            _serialObj = new SerializedObject(_scriptableObj);
            _skin = Resources.Load<GUISkin>("EditorData/UIUtilitiesEditorSkin");
            _editorData = Resources.Load<UIUtilitiesEditorData>("EditorData/UIUtilitiesEditorData");
        }

        private void Update()
        {
            Repaint();
        }

        private void DrawMainWindow()
        {
            _viewScrollPosition = EditorGUILayout.BeginScrollView(_viewScrollPosition, false, false);

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            float width = Screen.width;
            width /= 3;
            width *= 0.9f;
            GUILayout.Space(5);
            if (GUILayout.Button("HOME",
                _currentCategory == Categories.Home
                    ? _skin.GetStyle(_editorData.buttonSelectedStyle)
                    : _skin.GetStyle(_editorData.buttonOffStyle),
                GUILayout.ExpandWidth(true), GUILayout.MinWidth(width), GUILayout.MinHeight(25)))
            {
                _currentCategory = Categories.Home;
            }

            GUILayout.Space(10);
            if (GUILayout.Button("UTILITIES",
                _currentCategory == Categories.Utilities
                    ? _skin.GetStyle(_editorData.buttonSelectedStyle)
                    : _skin.GetStyle(_editorData.buttonOffStyle),
                GUILayout.ExpandWidth(true), GUILayout.MinWidth(width), GUILayout.MinHeight(25)))
            {
                _currentCategory = Categories.Utilities;
            }

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();

            switch (_currentCategory)
            {
                case Categories.Home:
                    DrawHome();
                    break;
                case Categories.Utilities:
                    DrawUtilities();
                    break;
            }

            _serialObj.ApplyModifiedProperties();

            GUILayout.Space(20);
            GUILayout.EndScrollView();
        }

        private void DrawHome()
        {
            GUILayout.Space(15);
            
            EditorGUILayout.LabelField("Watch the introduction video:", GetStyle("title"),
                GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
                
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Introduction to UI Toolbox", _skin.GetStyle(_editorData.addButtonStyle),
                GUILayout.Height(30), GUILayout.ExpandWidth(true)))
            {
                Application.OpenURL("https://youtu.be/svZe6cND-Gc");
            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(15);
            EditorGUILayout.LabelField("Other Blink Products:", GetStyle("title"),
                GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
                
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Blink Store", _skin.GetStyle(_editorData.addButtonStyle),
                GUILayout.Height(30), GUILayout.ExpandWidth(true)))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/49855");
            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawUtilities()
        {
            GUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Action:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
            _currentToolCategory = (ToolCategory) EditorGUILayout.EnumPopup(_currentToolCategory, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            switch (_currentToolCategory)
            {
                case ToolCategory.Select:
                    DrawSelectingTools();
                    break;
                case ToolCategory.Rename:
                    DrawRenamingTools();
                    break;
            }
        }

        private void DrawSelectingTools()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select By:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
            _currentSelectionType =
                (SelectionType) EditorGUILayout.EnumPopup(_currentSelectionType);
            EditorGUILayout.EndHorizontal();
            
            switch (_currentSelectionType)
            {
                case SelectionType.Component:
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Component:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(70));
                    GUILayout.Space(5);
                    _currentSelectionComponentType =
                        (SelectionComponentType) EditorGUILayout.EnumPopup(_currentSelectionComponentType);
                    EditorGUILayout.EndHorizontal();
                
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Select In:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    var serialProp = _serialObj.FindProperty("gameObjectParents");
                    EditorGUILayout.PropertyField(serialProp, true);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(20);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    if (GUILayout.Button("SELECT", _skin.GetStyle(_editorData.addButtonStyle),
                        GUILayout.Height(35), GUILayout.ExpandWidth(true)))
                    {
                        if (gameObjectParents.Length == 0) return;
                        Selection.objects = null;
                        List<GameObject> allChildGameObjects = new List<GameObject>();
                        foreach (var go in gameObjectParents)
                        {
                            switch (_currentSelectionComponentType)
                            {
                                case SelectionComponentType.Text:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Text>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.TextTMP:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<TextMeshProUGUI>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Image:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Image>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.RawImage:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<RawImage>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Button:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Button>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Toggle:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Toggle>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Slider:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Slider>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Scrollbar:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Scrollbar>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Dropdown:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Dropdown>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.DropdownTMP:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<TMP_Dropdown>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.InputField:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<InputField>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.InputFieldTMP:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<TMP_InputField>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.Canvas:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Canvas>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.ScrollView:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<ScrollRect>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                                case SelectionComponentType.GameObject:
                                    allChildGameObjects.AddRange(go.GetComponentsInChildren<Transform>()
                                        .Select(matchingComponent => matchingComponent.gameObject));
                                    break;
                            }
                        }

                        Selection.objects = allChildGameObjects.ToArray();
                    }
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                case SelectionType.Name:
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Rule:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    _currentSelectionNameType =
                        (SelectionNameType) EditorGUILayout.EnumPopup(_currentSelectionNameType);
                    EditorGUILayout.EndHorizontal();
                
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Text Value:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    selectionNameValue = EditorGUILayout.TextField(selectionNameValue, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Select In:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    var serialProp = _serialObj.FindProperty("gameObjectParents");
                    EditorGUILayout.PropertyField(serialProp, true);
                    EditorGUILayout.EndHorizontal();
                
                    GUILayout.Space(20);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    if (GUILayout.Button("SELECT", _skin.GetStyle(_editorData.addButtonStyle), GUILayout.Height(35), GUILayout.ExpandWidth(true)))
                    {
                        if (gameObjectParents.Length == 0) return;
                        Selection.objects = null;
                        List<GameObject> allChildGameObjects = new List<GameObject>();
                        foreach (var go in gameObjectParents)
                        {
                            foreach (var child in go.GetComponentsInChildren<Transform>())
                            {
                                if (IsMatchingNameRules(child.gameObject.name)) allChildGameObjects.Add(child.gameObject);
                            }
                        }

                        Selection.objects = allChildGameObjects.ToArray();
                    }
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            }
        }

        private void DrawRenamingTools()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
            _currentRenamingType =
                (RenamingType) EditorGUILayout.EnumPopup(_currentRenamingType);
            EditorGUILayout.EndHorizontal();

            switch (_currentRenamingType)
            {
                case RenamingType.ReplaceCompletely:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Replace by:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    renameTextValue =
                        EditorGUILayout.TextField(renameTextValue, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
                case RenamingType.ReplaceInName:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Replace:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    renameTextValue =
                        EditorGUILayout.TextField(renameTextValue, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("By:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    renameTextValue2 =
                        EditorGUILayout.TextField(renameTextValue2, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
                case RenamingType.AddStart:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Add:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    renameTextValue =
                        EditorGUILayout.TextField(renameTextValue, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
                case RenamingType.AddEnd:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Add:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    renameTextValue =
                        EditorGUILayout.TextField(renameTextValue, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
                case RenamingType.RemoveStart:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Count:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    removeCharacterCount =
                        EditorGUILayout.IntField(removeCharacterCount, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
                case RenamingType.RemoveEnd:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Count:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    removeCharacterCount =
                        EditorGUILayout.IntField(removeCharacterCount, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
                case RenamingType.NumberStart:
                case RenamingType.NumberEnd:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Prefix:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    numberTextPrefix =
                        EditorGUILayout.TextField(numberTextPrefix, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Start At:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    numberRenameStartAt =
                        EditorGUILayout.IntField(numberRenameStartAt, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Suffix:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(75));
                    numberTextSuffix =
                        EditorGUILayout.TextField(numberTextSuffix, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    break;
            }
            
            GUILayout.Space(20);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("RENAME SELECTION", _skin.GetStyle(_editorData.addButtonStyle), GUILayout.Height(35), GUILayout.ExpandWidth(true)))
            {
                if (Selection.objects.Length == 0) return;
                int currentNumber = numberRenameStartAt;
                foreach (var selectedObject in Selection.objects)
                {
                    if (_currentRenamingType != RenamingType.NumberStart && _currentRenamingType != RenamingType.NumberEnd)
                    {
                        selectedObject.name = GetNewName(selectedObject.name);
                    }
                    else
                    {
                        string numberText = numberTextPrefix + currentNumber + numberTextSuffix;
                        currentNumber++;

                        if (_currentRenamingType == RenamingType.NumberStart)
                            selectedObject.name = numberText + selectedObject.name;
                        else
                            selectedObject.name = selectedObject.name + numberText;
                    }
                }
            }
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }

        private string GetNewName(string objectName)
        {
            switch (_currentRenamingType)
            {
                case RenamingType.ReplaceCompletely:
                    return renameTextValue;
                case RenamingType.ReplaceInName:
                    return objectName.Replace(renameTextValue, renameTextValue2);
                case RenamingType.AddStart:
                    return renameTextValue + objectName;
                case RenamingType.AddEnd:
                    return objectName + renameTextValue;
                case RenamingType.RemoveStart:
                    return objectName.Length < removeCharacterCount ? objectName : objectName.Remove(0, removeCharacterCount);
                case RenamingType.RemoveEnd:
                    return objectName.Length < removeCharacterCount ? objectName : objectName.Remove(objectName.Length-removeCharacterCount, removeCharacterCount);
                case RenamingType.NumberEnd:
                    break;
            }

            return objectName;
        }

        private bool IsMatchingNameRules(string goName)
        {
            switch (_currentSelectionNameType)
            {
                case SelectionNameType.Contains:
                    return goName.Contains(selectionNameValue);
                case SelectionNameType.DoNotContain:
                    return !goName.Contains(selectionNameValue);
                case SelectionNameType.Equal:
                    return string.Equals(goName, selectionNameValue);
                case SelectionNameType.DoNotEqual:
                    return !string.Equals(goName, selectionNameValue);
                case SelectionNameType.StartWith:
                    return goName.StartsWith(selectionNameValue);
                case SelectionNameType.DoNotStartWith:
                    return !goName.StartsWith(selectionNameValue);
                case SelectionNameType.EndWith:
                    return goName.EndsWith(selectionNameValue);
                case SelectionNameType.DoNotEndWith:
                    return !goName.EndsWith(selectionNameValue);
            }

            return false;
        }
        
        private GUIStyle GetStyle(string styleName)
        {
            var style = new GUIStyle();
            switch (styleName)
            {
                case "title":
                    style.alignment = TextAnchor.MiddleLeft;
                    style.fontSize = 20;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = Color.white;
                    break;
            }

            return style;
        }
        
    }
}

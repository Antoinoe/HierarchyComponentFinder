using UnityEditor;
using UnityEngine;

class HierarchyComponentFinder : EditorWindow 
{
    private string userInputFieldText = "";
    private string gameObjectsFound = "";
    private GUIStyle redLabelStyle;
    
    [MenuItem ("Tools/GameObjectFinderByComponent")]
    public static void DisplayWindow () 
    {
        var window = GetWindow<HierarchyComponentFinder>();
        window.titleContent = new GUIContent("Hierarchy Component Finder");
        window.Show();
    }
    private Vector2 scrollPosition = Vector2.zero;
    private void OnGUI () {
        GUILayout.Label("Research all GameObjects in the scene by entering the name of a component");
        userInputFieldText = EditorGUILayout.TextField ("Text Field", userInputFieldText);
        
        if (GUILayout.Button("Research"))
        {
            gameObjectsFound = "";
            
            if (string.IsNullOrEmpty(userInputFieldText)) return;
            
            var components = Component.FindObjectsOfType(typeof(Component));
            var hasFoundAtLeastOne = false;
            
            foreach (var c in components)
            {
               if (!c.GetType().Name.ToLower().Contains(userInputFieldText.ToLower())) continue;
               hasFoundAtLeastOne = true;
               gameObjectsFound += $"{c.name}\n";
            }

            //Displays an error message if no match has been found.
            if (!hasFoundAtLeastOne)
            {
                //IMPORTANT : For some reasons the label is not appearing when no match is found.
                GUILayout.Label($"Didn't find any match for {userInputFieldText}.", GetRedLabelStyle());
                Debug.LogError($"Didn't found any GameObjects with a component {userInputFieldText}");
                return;
            }
        }
        
        GUILayoutOption[] scrollOptions = { GUILayout.ExpandHeight(true) };
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, scrollOptions);
        
        var answerLines = gameObjectsFound.Split('\n');
        foreach (var t in answerLines)
        {
            var gameObjectName = t.Trim();
            if (string.IsNullOrEmpty(gameObjectName)) continue;
            GUILayout.BeginHorizontal(); 
            if (GUILayout.Button(gameObjectName, GUILayout.ExpandWidth(false))) // Set the button to expand horizontally
            {
                var selectedGameObject = GameObject.Find(gameObjectName);
                if (selectedGameObject != null)
                {
                    Selection.activeGameObject = selectedGameObject;
                    EditorGUIUtility.PingObject(selectedGameObject);
                }
            }
            GUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
    }

    private GUIStyle GetRedLabelStyle()
    {
        
        if (redLabelStyle != null) return redLabelStyle;
        redLabelStyle = new GUIStyle(EditorStyles.label)
        {
            normal =
            {
                textColor = Color.red
            }
        };

        return redLabelStyle;
    }
}

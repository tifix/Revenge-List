using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DS.Windows
{
    
    using Utilities;
    public class DSEditorWindow : EditorWindow
    {
        private readonly string defaultFilename = "DialogueFileName";
        
        private TextField fileNameTextField;
        private Button saveButton;

        [MenuItem("Window/DS/Dialogue Graph")]
        public static void Open()
        {
            DSEditorWindow wnd = GetWindow<DSEditorWindow>();
            wnd.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();

            AddToolbar();

            AddStyles();
        }

        

        #region Elements Addition
        private void AddGraphView()
        {
            DSGraphView graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DSElementUtility.CreateTextField(defaultFilename, "File Name: ", callback => 
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DSElementUtility.CreateButton("Save");

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);

            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
        #endregion

        #region Utility Methods
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}


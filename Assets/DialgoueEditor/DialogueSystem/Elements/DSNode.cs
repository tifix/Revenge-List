using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DS.Elements
{
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }

        private DSGraphView graphView;

        private Color defaultBackgroundColor;

        public virtual void Init(DSGraphView dsGraphView, Vector2 position)
        {
            DialogueName = "DialogueName";
            Choices = new List<string>();
            Text = "Dialogue text.";

            graphView = dsGraphView;

            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node_main-container");
            extensionContainer.AddToClassList("ds-node_extension-container");
        }

        public virtual void Draw()
        {

            // Title Container
            TextField dialgueNameTextField = DSElementUtility.CreateTextField(DialogueName, callback =>
            {
                graphView.RemoveUngroupedNode(this);

                DialogueName = callback.newValue;

                graphView.AddUngroupedNode(this);
            });


            dialgueNameTextField.AddClasses(
                "ds-node_textfield",
                "ds-node_filename-textfield",
                "ds-node_textfield_hidden"
                );

            titleContainer.Insert(0, dialgueNameTextField);

            // Input Container
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputPort.name = "Dialogue Connection";

            inputContainer.Insert(0, inputPort);

            // Extensions Container
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node_custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

            TextField textTextField = DSElementUtility.CreateTextArea(Text);


            textTextField.AddClasses(
                "ds-node_textfield",
                "ds-node_quote-textfield"
                );

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
    }
}


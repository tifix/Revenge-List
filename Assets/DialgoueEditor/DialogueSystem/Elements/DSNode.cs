using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace DS.Elements
{

    using Enumerations;
    using UnityEditor;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }

        public virtual void Init(Vector2 position)
        {
            DialogueName = "DialogueName";
            Choices = new List<string>();
            Text = "Dialogue text.";

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node_main-container");
            extensionContainer.AddToClassList("ds-node_extension-container");
        }

        public virtual void Draw()
        {

            // Title Container
            TextField dialgueNameTextField = new TextField()
            {
                value = DialogueName
            };

            dialgueNameTextField.AddToClassList("ds-node_textfield");
            dialgueNameTextField.AddToClassList("ds-node_filename-textfield");
            dialgueNameTextField.AddToClassList("ds-node_textfield_hidden");

            titleContainer.Insert(0, dialgueNameTextField);

            // Input Container
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(InputActionType));

            inputPort.name = "Dialogue Connection";

            inputContainer.Add(inputPort);

            // Extensions Container
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node_custom-data-container");

            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };

            TextField textTextField = new TextField()
            {
                value = Text
            };

            textTextField.AddToClassList("ds-node_textfield");
            textTextField.AddToClassList("ds-node_quote-textfield");

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

        }
    }
}


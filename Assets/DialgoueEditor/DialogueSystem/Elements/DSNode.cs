using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace DS.Elements
{

    using Enumerations;
    using UnityEditor;
    using UnityEngine.UIElements;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }

        public void Init()
        {
            DialogueName = "DialogueName";
            Choices = new List<string>();
            Text = "Dialogue text.";
        }

        public void Draw()
        {

            // Title Container
            TextField dialgueNameTextField = new TextField()
            {
                value = DialogueName
            };

            titleContainer.Insert(0, dialgueNameTextField);

            // Input Container
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Input));

            inputPort.name = "Dialogue Connection";

            inputContainer.Add(inputPort);

            // Extensions Container
            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };

            TextField textTextField = new TextField()
            {
                value = Text
            };

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}


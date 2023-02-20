using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }

        public DSGroup Group { get; set; }

        protected DSGraphView graphView;

        private Color defaultBackgroundColor;

        public virtual void Init(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DSChoiceSaveData>();
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
            TextField dialgueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField) callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if(string.IsNullOrEmpty(target.value))
                {
                    if(!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if(string.IsNullOrEmpty(DialogueName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if( Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                DSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                DialogueName = target.value;

                graphView.AddGroupedNode(this, currentGroup);
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

            TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });


            textTextField.AddClasses(
                "ds-node_textfield",
                "ds-node_quote-textfield"
                );

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

        }

        #region Overrided Methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        #endregion

        #region Utility Methods

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach(Port port in container.Children())
            {
                if(!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}


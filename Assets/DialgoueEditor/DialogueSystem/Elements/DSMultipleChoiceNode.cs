using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace DS.Elements
{
    using Data.Save;
    using Windows;
    using Enumerations;
    using Utilities;

    public class DSMultipleChoiceNode : DSNode
    {
        public override void Init(DSGraphView dsGraphView, Vector2 position)
        {
            base.Init(dsGraphView, position);

            DialogueType = DSDialogueType.MultipleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            // Main Container
            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Insert(0, choicePort);
            });

            addChoiceButton.AddToClassList("ds-node_button");

            mainContainer.Insert(0, addChoiceButton);

            // Output Container
            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Insert(0, choicePort); 
            }

            RefreshExpandedState();

        }

        #region Element Creation
        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData)userData;

            Button deleteChoiceButton = DSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node_button");

            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });


            choiceTextField.AddClasses(
                "ds-node_textfield",
                "ds-node_choice-textfield",
                "ds-node_textfield_hidden"
                );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            return choicePort;
        }
        #endregion
    }
}


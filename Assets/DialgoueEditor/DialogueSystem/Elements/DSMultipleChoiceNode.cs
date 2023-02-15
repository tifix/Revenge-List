using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace DS.Elements
{
    using Enumerations;
    using Utilities;

    public class DSMultipleChoiceNode : DSNode
    {
        public override void Init(Vector2 position)
        {
            base.Init(position);

            DialogueType = DSDialogueType.MultipleChoice;

            Choices.Add("X");
        }

        public override void Draw()
        {
            base.Draw();

            // Main Container
            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                Port choicePort = CreateChoicePort("New Choice");

                Choices.Add("New Choice");

                outputContainer.Insert(0, choicePort);
            });

            addChoiceButton.AddToClassList("ds-node_button");

            mainContainer.Insert(0, addChoiceButton);

            // Output Container
            foreach (string choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Insert(0, choicePort); 
            }

            RefreshExpandedState();

        }

        #region Element Creation
        private Port CreateChoicePort(string choice)
        {
            Port choicePort = this.CreatePort();

            Button deleteChoiceButton = DSElementUtility.CreateButton("X");

            deleteChoiceButton.AddToClassList("ds-node_button");

            TextField choiceTextField = DSElementUtility.CreateTextField(choice);


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


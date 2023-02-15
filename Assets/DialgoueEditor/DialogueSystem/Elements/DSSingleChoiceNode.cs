using UnityEngine;
using Unity.VisualScripting.InputSystem;
using UnityEditor.Experimental.GraphView;


namespace DS.Elements
{
    using Enumerations;
    using Utilities;


    public class DSSingleChoiceNode : DSNode
    {
        public override void Init(Vector2 position)
        {
            base.Init(position);

            DialogueType = DSDialogueType.SingleChoice;

            Choices.Add("Next Dialogue");
        }

        public override void Draw()
        {
            base.Draw();

            /* Output Container */

            foreach(string choice in Choices)
            {
                Port choicePort = this.CreatePort(choice);

                choicePort.portName = choice;

                outputContainer.Add(choicePort);

            }

            RefreshExpandedState();
        }
        
    }
}


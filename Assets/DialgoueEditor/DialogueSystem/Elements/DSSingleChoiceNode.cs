using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DS.Elements
{
    using Enumerations;
    using Unity.VisualScripting.InputSystem;
    using UnityEditor.Experimental.GraphView;

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

            // Output Container

            foreach(string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(OutputType));

                choicePort.portName = choice;

                outputContainer.Add(choicePort);

            }

            RefreshExpandedState();
        }
        
    }
}


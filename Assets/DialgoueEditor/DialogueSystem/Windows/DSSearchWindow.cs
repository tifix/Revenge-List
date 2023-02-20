using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Windows
{
    using Elements;
    using Enumerations;
    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        private Texture2D indentationIcon;

        public void Init(DSGraphView dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0,0, Color.clear);
            indentationIcon.Apply();
        }
        // Populates the search Window
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.MultipleChoice
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return searchTreeEntries;
        }


        //What happens when menu item is seleted
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch(SearchTreeEntry.userData)
            {
                case DSDialogueType.SingleChoice:
                    {
                        DSSingleChoiceNode singleChoiceNode = (DSSingleChoiceNode) graphView.CreateNode("DialogueName", DSDialogueType.SingleChoice, localMousePosition);

                        graphView.AddElement(singleChoiceNode);

                        return true;
                    }
                case DSDialogueType.MultipleChoice:
                    {
                        DSMultipleChoiceNode multiChoiceNode = (DSMultipleChoiceNode)graphView.CreateNode("DialogueName", DSDialogueType.MultipleChoice, localMousePosition);

                        graphView.AddElement(multiChoiceNode);

                        return true;
                    }
                case Group _:
                    {
                        graphView.CreateGroup("Dialogue Group", localMousePosition);

                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}


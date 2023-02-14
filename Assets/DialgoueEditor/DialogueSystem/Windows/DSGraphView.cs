using DS.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{

    using Elements;

    public class DSGraphView : GraphView
    {
        public DSGraphView()
        {
            AddManipulators();

            AddGridBackground();

            AddStyles();
        }
        
        private void AddManipulators()
        {
            // sets up zoom to allow mouse wheel scrolling zoom
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(CreateNodeContextualMenu());

            // adds manipulator to allow movement by dragging
            this.AddManipulator(new ContentDragger());
        }

        private IManipulator CreateNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode()))
                );

            return contextualMenuManipulator;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private DSNode CreateNode()
        {
            DSNode node = new DSNode();

            node.Init();
            node.Draw();

            return node;
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("DialogueSystem/DSGraphViewStyles.uss");

            styleSheets.Add(styleSheet);
        }
    }
}



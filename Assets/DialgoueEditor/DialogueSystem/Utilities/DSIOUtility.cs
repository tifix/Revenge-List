#if (UNITY_EDITOR) 
using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Utilities
{

    using Data.Save;
    using Data;
    using Elements;
    using ScriptableObjects;
    using Windows;
    

    public static class DSIOUtility
    {
        private static DSGraphView graphView;

        private static string graphFileName;
        private static string containerFolderPath;

        private static List<DSGroup> groups;
        private static List<DSNode> nodes;

        private static Dictionary<string, DSDialogueGroupSO> createdDialogueGroups;
        private static Dictionary<string, DSDialogueSO> createdDialogues;

        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;

        public static void Init(DSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFileName}";

            groups = new List<DSGroup>();
            nodes = new List<DSNode>();

            createdDialogueGroups = new Dictionary<string, DSDialogueGroupSO>();
            createdDialogues = new Dictionary<string, DSDialogueSO>();

            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();

    }

    #region Save Methods
    public static void Save()
        {
            CreateStaticFolders();                      //Create folder structure from groups
            GetElementsFromGraphView();                 
                                                        //Export the Dialogue container to Scriptable Object
            DSGraphSaveDataSO graphData = CreateAsset<DSGraphSaveDataSO>("Assets/DialogueSystem/Graphs", $"{graphFileName}Graph");
            graphData.Init(graphFileName);

            DSDialogueContainerSO dialogueContainer = new DSDialogueContainerSO();
            dialogueContainer.Init(graphFileName);

            SaveGroups(graphData, dialogueContainer);   //Export groups - to graph, .Json file and Scriptable Object
            SaveNodes(graphData, dialogueContainer);    //Export nodes - to graph, .Json file and Scriptable Object

            SaveAsset(graphData);                       //Force refresh asset database
            SaveToJsonMasterData(dialogueContainer);    //Export the Dialogue container to .Json
        }

        #region Groups
        private static void SaveGroups(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (DSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToJson(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveDataSO graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }
        private static void SaveGroupToJson(DSGroup group, DSDialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            StreamWriter writer = new StreamWriter($"{containerFolderPath}/Groups/" + groupName + "/" + groupName + ".json");


            DSDialogueGroupSO dialogueGroup = new DSDialogueGroupSO();
            dialogueGroup.Init(groupName);

                        //Debug.Log("name "+groupName+ " ID " + group.ID);
            Debug.Log("name "+ dialogueGroup.GroupName);
            createdDialogueGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.DialogueGroups.Add(groupName, new List<DSDialogueSO>());

            string s = JsonUtility.ToJson(dialogueGroup, true);
            writer.Write(s);
            writer.Close();
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach(string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }


        #endregion

        #region Nodes
        private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();
            foreach(DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveToJson(node, dialogueContainer);

                if(node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);

            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }


        private static void SaveNodeToGraph(DSNode node, DSGraphSaveDataSO graphData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            List<string> IDs = new List<string>();

            //Value.outputContainer
            foreach (var p in node.outputContainer.Children())
            {
               // Debug.Log("Child");
                if (p is Port) 
                {
                    
                    DSChoiceSaveData temp = (DSChoiceSaveData)p.userData;
                    IDs.Add(temp.NodeID);

                    Port temp2 = (Port)p;

                    if (temp2.connected == true) Debug.Log("port found " + temp.NodeID);
                    else 
                    {
                        Debug.Log("final port empty. ");
                        //IDs.
                    }
                    
                }

            }

            DSNodeSaveData nodeData = new DSNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogueName,
                Choices = choices,
                Text = node.Text,
                SpeakerName = node.SpeakerName,
                SpritePath = node.SpritePath,
                GroupID = node.Group?.ID,
                //DialogueType = node.DialogueType,
                Position = node.GetPosition().position,
                isStartNode = node.IsStartingNode(),
                ChildIDs = IDs


            };


            graphData.Nodes.Add(nodeData);

        }
        private static void SaveToJson(DSNode node, DSDialogueContainerSO dialogueContainer)//, DSDialogueContainerSO dialogueContainer 
        {
            StreamWriter writer;
            DSDialogueSO dialogue = new DSDialogueSO();


            if (node.Group != null)
            {
                writer = new StreamWriter($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues/" + node.DialogueName + ".json");   //save file structure

                dialogueContainer.DialogueGroups.AddItem(node.Group.ID, dialogue);
            }
            else
            {
                writer = new StreamWriter($"{containerFolderPath}/Global/Dialogues/" + node.DialogueName + ".json");   //save file structure
                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Init(
                 node.ID,
                 node.DialogueName,
                 node.Text,
                 node.SpeakerName,
                 node.SpritePath,
                 ConvertNodeChoicesToDialogueChoices(node.Choices),
                 node.DialogueType,
                 node.IsStartingNode()
                 );

            createdDialogues.Add(node.ID, dialogue);

            string s = JsonUtility.ToJson(dialogue,true);
            writer.Write(s);
            writer.Close();
        }
        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

            foreach(DSChoiceSaveData nodeChoice in nodeChoices)
            {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };

                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
        }
        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (DSNode node in nodes)
            {
                DSDialogueSO dialogue = createdDialogues[node.ID];

                for(int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].nextID = createdDialogues[nodeChoice.NodeID].nodeID;

                    //SaveAsset(dialogue);
                }
            }
        }
        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupNodeNames != null && graphData.OldGroupNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();
                    if(currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach(string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }
        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if(graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach ( string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }


        #endregion
        #endregion

        #region Load Methods
        public static void Load()
        {
            DSGraphSaveDataSO graphData = LoadAsset<DSGraphSaveDataSO>("Assets/DialogueSystem/Graphs", graphFileName);
           // graphData = LoadFromJson();


            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Couldn't load file!", 
                    "The File at the following path could not be found:\n\n" +
                    $"Assets/DialogueSystem/Graphs/{graphFileName}\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path above.", 
                    "Thanks!"
                    );

                return;
            }

            DSEditorWindow.UpdateFilename(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }


        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach (DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }


        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach (DSNodeSaveData nodeData in nodes)
            {
               
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
                DSNode node = graphView.CreateNode(nodeData.Name, Enumerations.DSDialogueType.SingleChoice, nodeData.Position, false);      //disabled multiple choice for the time being

                node.ID = nodeData.ID;
                node.Choices = choices;
                node.SpeakerName = nodeData.SpeakerName;
                node.Text = nodeData.Text;
                node.SpritePath = nodeData.SpritePath;

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if(string.IsNullOrEmpty(nodeData.GroupID))
                    continue;
                

                DSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;
                group.AddElement(node);
            }
        }

        public static DSDialogueContainerSO LoadFromJson()
        {
            string path = "Assets/DialogueSystem/Graphs/" + graphFileName + ".json";
            StreamReader r = new StreamReader(path);

            DSDialogueContainerSO masterData = new DSDialogueContainerSO();
            try
            {
                Debug.Log("Attempting to load from "+ path);
                string rawJson = r.ReadToEnd();
                Debug.Log("json length"+rawJson.Length);
                Debug.Log(rawJson);
                masterData = JsonUtility.FromJson<DSDialogueContainerSO>(rawJson);
                Debug.LogWarning(masterData.FileName+" loaded!");
            }
            
            catch{Debug.LogWarning("Unknown error loading master data");}

            r.Close();

            return masterData;
        }
        
        private static void LoadSaveDataFromContainer(DSDialogueContainerSO container) 
        {
        
        }

        private static void LoadGroupFromJson(DSGroup group)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            StreamWriter writer = new StreamWriter($"{containerFolderPath}/Groups" + groupName + ".json");
            //($"{containerFolderPath}/Groups/{group.title}/Dialogues" + node.DialogueName + ".json");
            string s = JsonUtility.ToJson(group);
            writer.Write(s);
            writer.Close();
        }


        private static void LoadNodesConnections()
        {
            foreach(KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData)choicePort.userData;

                    if(string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    DSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }
        #endregion

        #region Creation Methods
        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/DialogueEditor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");
            CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }
        #endregion

        #region Fetch Methods
        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DSGroup);
            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DSNode node)
                {
                    nodes.Add(node);

                    return;
                }

                if(graphElement.GetType() == groupType)
                {
                    DSGroup group = (DSGroup)graphElement;

                    groups.Add(group);

                    return;
                }
            });
        }

        public static DSNode FindObjectID(string ID)
        {
            if(ID==null || ID=="") return null;

            foreach (DSNode node in nodes)
            {
                if(ID == node.ID)
                {
                    return node;
                }
            }
            return null;
        }
        
        public static DSNodeSaveData FindSaveDataID(string ID, DSGraphSaveDataSO graph)
        {
            if (ID == null || ID == "") return null;

            foreach (DSNodeSaveData node in graph.Nodes)
            {
                if (ID == node.ID)
                {
                    return node;
                }
            }
            return null;
        }

        
        #endregion

        #region Utility Methods

        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }
        private static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private static void SaveToJsonMasterData(DSDialogueContainerSO dialogueContainer) 
        {
            //StreamWriter w = new StreamWriter(containerFolderPath +"/"+ graphFileName + "Graph.json");
            StreamWriter w = new StreamWriter("Assets/DialogueSystem/Graphs/" + graphFileName + "Graph.json");
            string s = JsonUtility.ToJson(dialogueContainer,true);
            Debug.LogWarning("Saving data to json at Assets/DialogueSystem/Graphs/" + graphFileName + "Graph.json");
            Debug.Log(s);
            w.Write(s);
            w.Close();
            AssetDatabase.Refresh();
        }


        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();

            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choiceData);
            }

            return choices;
        }
        #endregion
    }

}
#endif
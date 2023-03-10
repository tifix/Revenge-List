using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    public class DSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName;// { get; set; }
//#if (UNITY_EDITOR)
        [field: SerializeField] public List<DSGroupSaveData> Groups;// { get; set; }
        [field: SerializeField] public List<DSNodeSaveData> Nodes;// { get; set; }
//#endif
        [field: SerializeField] public List<string> OldGroupNames;// { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames;// { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupNodeNames;// { get; set; }

        public void Init(string fileName)
        {
            FileName = fileName;

//#if (UNITY_EDITOR)
            Groups = new List<DSGroupSaveData>();
            Nodes = new List<DSNodeSaveData>();
//#endif
        }
    }
}
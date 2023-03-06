using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    [System.Serializable]
    public class DSDialogueContainerSO 
    {
        [field: SerializeField] public string FileName;// { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<DSDialogueSO>> DialogueGroups;//{ get; set; }
        [field: SerializeField] public List<DSDialogueSO> UngroupedDialogues;// { get; set; }

        public void Init(string filename)
        {
            FileName = filename;

            DialogueGroups = new SerializableDictionary<string, List<DSDialogueSO>>();
            UngroupedDialogues = new List<DSDialogueSO>();
        }
    }
}


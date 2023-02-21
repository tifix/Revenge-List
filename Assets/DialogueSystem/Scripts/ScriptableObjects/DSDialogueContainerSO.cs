using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    public class DSDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>> DialogueGroups{ get; set; }
        [field: SerializeField] public List<DSDialogueSO> UngroupedDialogues { get; set; }

        public void Init(string filename)
        {
            FileName = filename;

            DialogueGroups = new SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>>();
            UngroupedDialogues = new List<DSDialogueSO>();
        }
    }
}


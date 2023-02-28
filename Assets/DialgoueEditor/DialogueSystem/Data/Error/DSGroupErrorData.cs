#if (UNITY_EDITOR) 
using System.Collections.Generic;

namespace DS.Data.Error
{
    using DS.Elements;

    public class DSGroupErrorData
    {
        public DSErrorData ErrorData { get; set; }
        public List<DSGroup> Groups { get; set; }

        public DSGroupErrorData()
        {
            ErrorData = new DSErrorData();
            Groups = new List<DSGroup>();
        }
    }
}

#endif
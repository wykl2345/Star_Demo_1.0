using UnityEngine;

namespace Script.Tool
{
    public class ToolsHelp
    {
        public static void RemoveChildren(GameObject parent)
        {
            var paTrans = parent.transform;
            for (int i = paTrans.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(paTrans.GetChild(i).gameObject);
            }
        }

    }
}
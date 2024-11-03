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

        public static void SetActive(GameObject go, bool active)
        {
            if (go.activeSelf != active)
            {
                go.SetActive(active);
            }
        }
        
        public static void SetActive(Transform trans, bool active)
        {
            if (trans.gameObject.activeSelf != active)
            {
                trans.gameObject.SetActive(active);
            }
        }
        
        /// <summary>
        /// 平滑插值，带最大值限制
        /// </summary>
        /// <param name="from">起始值</param>
        /// <param name="to">目标值</param>
        /// <param name="t">插值因子，范围从 0 到 1</param>
        /// <param name="maxValue">最大值限制</param>
        /// <returns>插值结果</returns>
        public static float SmoothStepWithMax(float from, float to, float t, float maxValue)
        {
            float result = Mathf.SmoothStep(from, to, t);
            return Mathf.Min(result, maxValue);
        }

        /// <summary>
        /// 平滑插值，带最大值限制
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <param name="t">插值因子，范围从 0 到 1</param>
        /// <param name="maxValue">最大值限制</param>
        /// <returns>插值结果</returns>
        public static Vector3 SmoothStepWithMax(Vector3 from, Vector3 to, float t, float maxValue)
        {
            Vector3 result = Vector3.Lerp(from, to, t * t * (3f - 2f * t)); // 使用 SmoothStep 公式
            return new Vector3(
                Mathf.Min(result.x, maxValue),
                Mathf.Min(result.y, maxValue),
                Mathf.Min(result.z, maxValue)
            );
        }

        
    }
}
// using UnityEngine;
// using System.Collections;

// [DefaultExecutionOrder(50)]
// public class RaisingSingleSceneBootstrapper : MonoBehaviour
// {
//     [SerializeField] private int fallbackOverrideMaxTurn = 0; // 의도에 maxTurn 없을 때만 사용(0이면 무시)

//     private IEnumerator Start()
//     {
//         yield return null; // GM/SM 싱글톤 준비 대기

//         var sm = SaveManager.Instance;
//         if (sm == null) yield break;

//         sm.ConsumeRaisingEnterRequest(out int idx, out bool reset, out int? maxTurnOpt);

//         int? finalMax = maxTurnOpt.HasValue ? maxTurnOpt
//                                             : (fallbackOverrideMaxTurn > 0 ? fallbackOverrideMaxTurn : (int?)null);

//         sm.SetRaisingContext(idx <= 0 ? 1 : idx, resetTurnAndStamina: reset, overrideMaxTurn: finalMax);
//     }
// }

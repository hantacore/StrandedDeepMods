using HarmonyLib;
using LE_LevelEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedWideMod_Harmony
{
    public static partial class Main
    {
        static MethodInfo mi_GetEditDelta = AccessTools.Method(typeof(LE_ObjectEditHandle), "GetEditDelta");
        static FieldInfo fi_m_dragSkipCounter = AccessTools.Field(typeof(LE_ObjectEditHandle), "m_dragSkipCounter");
        static FieldInfo fi_m_activeEditAxis = AccessTools.Field(typeof(LE_ObjectEditHandle), "m_activeEditAxis");

        [HarmonyPatch(typeof(LE_ObjectEditHandle), "Move")]
        class LE_ObjectEditHandle_Move_Patch
        {
            static bool Prefix(LE_ObjectEditHandle __instance)
            {
                try
                {
                    float editDelta = (float)mi_GetEditDelta.Invoke(__instance, new object[] { });//__instance.GetEditDelta();
                    //if (__instance.m_dragSkipCounter == 0)
                    int dragSkipCounter = (int)fi_m_dragSkipCounter.GetValue(__instance);
                    if (dragSkipCounter == 0)
                    {
                        int num = 350;//100
                        //Vector3 a = __instance.transform.TransformDirection(__instance.m_activeEditAxis);
                        Vector3 a = __instance.transform.TransformDirection((Vector3)fi_m_activeEditAxis.GetValue(__instance));
                        if (__instance.transform.position.x > (float)num)
                        {
                            if (editDelta < 0f)
                            {
                                __instance.transform.parent.position += a * editDelta;
                            }
                        }
                        else if (__instance.transform.position.x < -(float)num)
                        {
                            if (editDelta > 0f)
                            {
                                __instance.transform.parent.position += a * editDelta;
                            }
                        }
                        else if (__instance.transform.position.y > (float)num)
                        {
                            if (editDelta < 0f)
                            {
                                __instance.transform.parent.position += a * editDelta;
                            }
                        }
                        else if (__instance.transform.position.y < -(float)num)
                        {
                            if (editDelta > 0f)
                            {
                                __instance.transform.parent.position += a * editDelta;
                            }
                        }
                        else if (__instance.transform.position.z > (float)num)
                        {
                            if (editDelta < 0f)
                            {
                                __instance.transform.parent.position += a * editDelta;
                            }
                        }
                        else if (__instance.transform.position.z < -(float)num)
                        {
                            if (editDelta > 0f)
                            {
                                __instance.transform.parent.position += a * editDelta;
                            }
                        }
                        else
                        {
                            __instance.transform.parent.position += a * editDelta;
                        }
                        Vector3 position = __instance.transform.parent.position;
                        position.x = Mathf.Clamp(__instance.transform.parent.position.x, -(float)num, (float)num);
                        position.y = Mathf.Clamp(__instance.transform.parent.position.y, -(float)num, (float)num);
                        position.z = Mathf.Clamp(__instance.transform.parent.position.z, -(float)num, (float)num);
                        __instance.transform.parent.position = position;
                        if (__instance.m_onTransformed != null)
                        {
                            __instance.m_onTransformed(__instance, EventArgs.Empty);
                            return false;
                        }
                    }
                    else if (Mathf.Abs(editDelta) > 0.0005f)
                    {
                        //__instance.m_dragSkipCounter++;
                        dragSkipCounter++;
                        fi_m_dragSkipCounter.SetValue(__instance, dragSkipCounter);
                    }
                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Wide (Harmony edition) : error while patching LE_GUI3dObject.IsObjectPlaceable : " + e);
                }
                return true;
            }
        }
    }
}

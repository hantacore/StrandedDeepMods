using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepModsUtils
{
    public static class KeyHandler
    {
        public static KeyCode PreviousKeyCode { get; set; }

        public static event EventHandler<KeyCode> KeyPressed;

        public static void HandleKey()
        {
            Event currentevent = Event.current;
            if (currentevent.isKey)
            {
                //Debug.Log("Stranded Deep Tweaks Mod : current Key : " + currentevent.keyCode);

                if (currentevent.type == EventType.KeyDown)
                {
                    if (PreviousKeyCode == currentevent.keyCode)
                    {
                        //Debug.Log("Stranded Deep Tweaks Mod : Key still pressed : " + currentevent.keyCode);
                        return;
                    }

                    //Debug.Log("Stranded Deep Tweaks Mod : Key pressed changed : " + currentevent.keyCode + " / " + PreviousKeyCode + " firing event");
                    PreviousKeyCode = currentevent.keyCode;
                    KeyPressed(null, PreviousKeyCode);
                }
                else if (currentevent.type == EventType.KeyUp)
                {
                    //Debug.Log("Stranded Deep Tweaks Mod : Key released ? : " + currentevent.keyCode);
                    PreviousKeyCode = KeyCode.None;
                }
            }
        }
    }
}

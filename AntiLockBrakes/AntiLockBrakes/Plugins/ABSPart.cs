﻿using System;
using UnityEngine;

namespace AntiLockBrakes
{
    public class ABSPart : PartModule
    {
        float timesince = 0;
        Boolean running = false;
        String currentText = "0.1";
        double currentRate = 0.1;
        protected Rect windowPos;

        [KSPField(isPersistant = false)]
        public float PowerConsumption;

        public override string GetInfo()
        {
            string i = base.GetInfo();
            i += "Power Consumption: " + PowerConsumption + "/s";
            return i;
        }

        public override void OnStart(StartState state)
        {
            print("ABS: Hello Kerbin!");
            if ((windowPos.x == 0) && (windowPos.y == 0))
            {
                windowPos = new Rect(Screen.width / 2, Screen.height / 2, 10, 10);
            }
        }

        public override void OnUpdate()
        {
            if (running && !(vessel.GetSrfVelocity().magnitude <= 1))
            {
                if (GetPower(part, PowerConsumption))
                {
                    timesince += TimeWarp.deltaTime;
                    print("ABS: " + timesince.ToString());
                    if (timesince >= currentRate)
                    {
                        vessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
                        print("ABS: Toggle Brakes");
                        timesince = 0;
                    }
                }
            }

            if (running && vessel.GetSrfVelocity().magnitude <= 1)
            {
                Deactivate();
                vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
            }
        }

        private bool GetPower(Part part, float PowerConsumption)
        {
            if (TimeWarp.deltaTime != 0)
            {
                float amount = part.RequestResource("ElectricCharge", PowerConsumption * TimeWarp.deltaTime);
                return amount != 0;
            }
            else
            {
                return true;
            }
        }

        [KSPEvent(guiActive=true, guiName="Activate ABS")]
        public void Activate()
        {
            print("ABS: ABS Started");
            running = true;
            Events["Activate"].active = false;
            Events["Deactivate"].active = true;
        }

        [KSPEvent(guiActive=true, guiName="Deactivate ABS", active=false)]
        public void Deactivate()
        {
            print("ABS: ABS Stopped");
            running = false;
            Events["Activate"].active = true;
            Events["Deactivate"].active = false;
        }
        [KSPEvent(guiActive=true, guiName="Edit ABS Settings")]
        private void openGUI()
        {
            RenderingManager.AddToPostDrawQueue(3, new Callback(drawGUI));
        }

        private void WindowGUI(int windowID)
        {
            GUIStyle sty = new GUIStyle(GUI.skin.button);
            GUIStyle sty2 = new GUIStyle(GUI.skin.textField);

            sty.normal.textColor = sty.focused.textColor = Color.white;
            sty.hover.textColor = sty.active.textColor = Color.yellow;
            sty.onNormal.textColor = sty.onFocused.textColor = sty.onHover.textColor = sty.onActive.textColor = Color.green;
            sty.padding = new RectOffset(8, 8, 8, 8);

            if (!Double.TryParse(currentText, out currentRate))
            {
                currentRate = 0.1;
            }

            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    GUILayout.Label("ABS Cycle Rate");
                    currentText = GUILayout.TextField(currentText, GUILayout.MinWidth(30.0F));
                    GUILayout.Label("s");
                GUILayout.EndHorizontal();
                if (GUILayout.Button("CLOSE", sty, GUILayout.ExpandWidth(true)))
                {
                    closeGUI();
                }
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void drawGUI()
        {
            GUI.skin = HighLogic.Skin;
            windowPos = GUILayout.Window(1, windowPos, WindowGUI, "ABS Options", GUILayout.MinWidth(100));
        }

        private void closeGUI()
        {
            RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGUI));
        }
    }
}

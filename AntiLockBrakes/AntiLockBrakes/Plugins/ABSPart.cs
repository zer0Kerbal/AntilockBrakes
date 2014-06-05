﻿using System;
using System.Timers;

namespace AntiLockBrakes
{
    public class ABSPart : PartModule
    {
        Timer timer;
        Boolean running = false;


        public override void OnStart(StartState state)
        {
            print("ABS: Hello Kerbin!");

            if (state != StartState.Editor)
            {
                timer = new Timer(100);
                print("ABS: Timer Set");
                timer.Elapsed += timer_Elapsed;
                print("ABS: Timer Elapsed Set");
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (running && vessel.GetSrfVelocity().magnitude <= 1)
            {
                Deactivate();
                vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            vessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
            print("ABS: Toggle Brakes");
        }

        [KSPEvent(guiActive=true, guiName="Activate ABS")]
        public void Activate()
        {
            timer.Start();
            print("ABS: Timer Started");
            running = true;
            Events["Activate"].active = false;
            Events["Deactivate"].active = true;
        }

        [KSPEvent(guiActive=true, guiName="Deactivate ABS", active=false)]
        public void Deactivate()
        {
            timer.Stop();
            print("ABS: Timer Stopped");
            running = false;
            Events["Activate"].active = true;
            Events["Deactivate"].active = false;
        }
    }
}
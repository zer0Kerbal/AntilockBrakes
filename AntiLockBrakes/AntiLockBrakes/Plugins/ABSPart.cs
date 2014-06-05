using System;

namespace AntiLockBrakes
{
    public class ABSPart : PartModule
    {
        float timesince = 0;
        Boolean running = false;

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
        }

        public override void OnUpdate()
        {
            if (running && !(vessel.GetSrfVelocity().magnitude <= 1))
            {
                if (GetPower(part, PowerConsumption))
                {
                    timesince += TimeWarp.deltaTime;
                    print("ABS: " + timesince.ToString());
                    if (timesince >= .1)
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
    }
}

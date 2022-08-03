using HugsLib;
using HugsLib.Settings;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorldTV {
    public class RimWorldTV : ModBase {
        public override string ModIdentifier => "RimWorldTV";

        private ModService ModService;

        public RimWorldTV() {
            ModService = ModService.Instance;
            ModService.Logger = this.Logger;
        }

        public override void DefsLoaded() {
            RegisterModSettings();
        }

        public void RegisterModSettings() {
            ModService.RegisterSettings(Settings);

            // This is a little hacky, basically we make a copy of all setting handles prior to adding effect settings. 
            // Then use that list to determine what effects should be hidden until "Show Advanced Settings" is checked. 
            List<SettingHandle> systemSettings = new List<SettingHandle>(Settings.Handles);
            foreach (KeyValuePair<string, Effect> entry in ModService.EffectList) {
                Effect effect = entry.Value;
                effect.RegisterSettings(Settings);
            }
            Settings.Handles.ToList().ForEach(handle => { 
                if (systemSettings.Contains(handle) == false) {
                    handle.VisibilityPredicate = () => { return ModService.ShowAdvanced; };
                }
            });
        }
    }
}

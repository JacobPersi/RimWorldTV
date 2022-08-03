using HugsLib.Settings;

namespace RimWorldTV {
    public interface ISettingProvider {
        void RegisterSettings(ModSettingsPack Settings);
    }
}
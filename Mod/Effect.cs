using HugsLib.Settings;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimWorldTV {
    public abstract class Effect : ISettingProvider {
        protected SettingHandle<bool> Enabled;

        public abstract EffectStatus Execute(EffectCommand command);
        public abstract string Code { get; }

        public virtual void RegisterSettings(ModSettingsPack Settings) { }
        public void RegisterBaseSetting(ModSettingsPack Settings) {
            Enabled = Settings.GetHandle<bool>(
                settingName: $"Settings.{Code}", title: $"{Code}.Title".Translate(), description: $"{Code}.Description".Translate(),
                defaultValue: true);
            Enabled.CustomDrawer = rect => false;
            Enabled.CanBeReset = false;
        }

        private void SendCardNotificationInternal(string label = null, string description = null, LetterDef notificationType = null, string triggeredBy = null, LookTargets lookTarget = null) {
            if (string.IsNullOrEmpty(label))
                label = $"{Code}.Notification".Translate();

            if (string.IsNullOrEmpty(description))
                description = $"{Code}.Description".Translate();
            
            if (notificationType == null)
                notificationType = LetterDefOf.NeutralEvent;

            if (ModService.Instance.ShouldDisplayViewerName && string.IsNullOrEmpty(triggeredBy) == false)
                description = description + string.Format("Notification.TriggeredBy".Translate(), triggeredBy);

            if (lookTarget == null)
                Find.LetterStack.ReceiveLetter(label, description, notificationType);
            else
                Find.LetterStack.ReceiveLetter(label, description, notificationType, lookTarget);
        }

        protected void SendCardNotification(LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(notificationType: notificationType, triggeredBy: triggeredBy);
        }

        protected void SendCardNotification(string label, string description = null, LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(label: label, description: description, notificationType: notificationType, triggeredBy: triggeredBy);
        }

        protected void SendCardNotification(Thing lookAtThing, LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(notificationType: notificationType, triggeredBy: triggeredBy, lookTarget: new LookTargets(lookAtThing));
        }

        protected void SendCardNotification(List<Thing> lookAtThings, LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(notificationType: notificationType, triggeredBy: triggeredBy, lookTarget: new LookTargets(lookAtThings));
        }

        protected void SendCardNotification(List<TargetInfo> lookAtThings, LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(notificationType: notificationType, triggeredBy: triggeredBy, lookTarget: new LookTargets(lookAtThings));
        }

        protected void SendCardNotification(Map map, IntVec3 location, LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(notificationType: notificationType, triggeredBy: triggeredBy, lookTarget: new LookTargets(location, map));
        }

        protected void SendCardNotification(Map map, IntVec3 location, string label = null, string description = null, LetterDef notificationType = null, string triggeredBy = null) {
            SendCardNotificationInternal(label: label, description: description, notificationType: notificationType, triggeredBy: triggeredBy, lookTarget: new LookTargets(location, map));
        }
    }
}

using FallenStrap.Models.SettingTasks.Base;

namespace FallenStrap.Models.SettingTasks
{
    // lets the user pick their own .rbxl file (built in Roblox Studio) to replace
    // the background scene shown behind the avatar/character editor, instead of
    // only being able to toggle the built-in "old avatar background" preset.
    public class AvatarBackgroundModPresetTask : StringBaseTask
    {
        public AvatarBackgroundModPresetTask() : base("ModPreset", "CustomAvatarBackground")
        {
            if (File.Exists(Paths.CustomAvatarBackground))
                OriginalState = Paths.CustomAvatarBackground;
        }

        public override void Execute()
        {
            if (!String.IsNullOrEmpty(NewState))
            {
                if (String.Compare(NewState, Paths.CustomAvatarBackground, StringComparison.InvariantCultureIgnoreCase) != 0 && File.Exists(NewState))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Paths.CustomAvatarBackground)!);
                    Filesystem.AssertReadOnly(Paths.CustomAvatarBackground);
                    File.Copy(NewState, Paths.CustomAvatarBackground, true);
                }
            }
            else
            {
                if (File.Exists(Paths.CustomAvatarBackground))
                {
                    Filesystem.AssertReadOnly(Paths.CustomAvatarBackground);
                    File.Delete(Paths.CustomAvatarBackground);
                }
            }

            OriginalState = NewState;
        }
    }
}

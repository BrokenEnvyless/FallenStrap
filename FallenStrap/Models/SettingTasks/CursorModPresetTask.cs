using FallenStrap.Models.SettingTasks.Base;

namespace FallenStrap.Models.SettingTasks
{
    public class CursorModPresetTask : StringBaseTask
    {
        public string? GetFileHash()
        {
            if (!File.Exists(Paths.CustomCursor))
                return null;

            using var fileStream = File.OpenRead(Paths.CustomCursor);
            return MD5Hash.Stringify(App.MD5Provider.ComputeHash(fileStream));
        }

        public CursorModPresetTask() : base("ModPreset", "CustomCursor")
        {
            if (File.Exists(Paths.CustomCursor))
                OriginalState = Paths.CustomCursor;
        }

        public override void Execute()
        {
            if (!String.IsNullOrEmpty(NewState))
            {
                if (String.Compare(NewState, Paths.CustomCursor, StringComparison.InvariantCultureIgnoreCase) != 0 && File.Exists(NewState))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Paths.CustomCursor)!);

                    // The same image is used for both the regular cursor and the
                    // "far" cursor (shown when zoomed out / at a distance in-game).
                    Filesystem.AssertReadOnly(Paths.CustomCursor);
                    File.Copy(NewState, Paths.CustomCursor, true);

                    Filesystem.AssertReadOnly(Paths.CustomCursorFar);
                    File.Copy(NewState, Paths.CustomCursorFar, true);
                }
            }
            else
            {
                if (File.Exists(Paths.CustomCursor))
                {
                    Filesystem.AssertReadOnly(Paths.CustomCursor);
                    File.Delete(Paths.CustomCursor);
                }

                if (File.Exists(Paths.CustomCursorFar))
                {
                    Filesystem.AssertReadOnly(Paths.CustomCursorFar);
                    File.Delete(Paths.CustomCursorFar);
                }
            }

            OriginalState = NewState;
        }
    }
}

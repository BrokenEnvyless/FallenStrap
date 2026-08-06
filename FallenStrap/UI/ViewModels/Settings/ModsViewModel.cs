using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using Windows.Win32;
using Windows.Win32.UI.Shell;
using Windows.Win32.Foundation;

using CommunityToolkit.Mvvm.Input;

using FallenStrap.Models.SettingTasks;
using FallenStrap.AppData;

namespace FallenStrap.UI.ViewModels.Settings
{
    public class ModsViewModel : NotifyPropertyChangedViewModel
    {
        private void OpenModsFolder() => Process.Start("explorer.exe", Paths.Modifications);

        private void ImportSkyboxTextures()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Roblox texture files (*.tex)|*.tex",
                Multiselect = true,
                Title = "Selecciona los 6 archivos .tex del skybox (sky###_lf/rt/up/dn/bk/ft.tex)"
            };

            if (dialog.ShowDialog() != true)
                return;

            if (dialog.FileNames.Length == 0)
                return;

            string destFolder = Path.Combine(Paths.Modifications, "PlatformContent\\pc\\textures\\sky");
            Directory.CreateDirectory(destFolder);

            int copied = 0;
            foreach (var file in dialog.FileNames)
            {
                if (!file.EndsWith(".tex", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                string dest = Path.Combine(destFolder, Path.GetFileName(file));
                Filesystem.AssertReadOnly(dest);
                File.Copy(file, dest, true);
                copied++;
            }

            Frontend.ShowMessageBox(
                $"Se copiaron {copied} archivo(s) .tex a la carpeta de skybox. Asegurate de haber seleccionado los 6 archivos (lf, rt, up, dn, bk, ft) para que el skybox se vea completo.",
                MessageBoxImage.Information
            );
        }

        private readonly Dictionary<string, byte[]> FontHeaders = new()
        {
            { "ttf", new byte[4] { 0x00, 0x01, 0x00, 0x00 } },
            { "otf", new byte[4] { 0x4F, 0x54, 0x54, 0x4F } },
            { "ttc", new byte[4] { 0x74, 0x74, 0x63, 0x66 } } 
        };

        private void ManageCustomFont()
        {
            if (!String.IsNullOrEmpty(TextFontTask.NewState))
            {
                TextFontTask.NewState = "";
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    Filter = $"{Strings.Menu_FontFiles}|*.ttf;*.otf;*.ttc"
                };

                if (dialog.ShowDialog() != true)
                    return;

                string type = dialog.FileName.Substring(dialog.FileName.Length-3, 3).ToLowerInvariant();

                if (!FontHeaders.ContainsKey(type) 
                    || !FontHeaders.Any(x => File.ReadAllBytes(dialog.FileName).Take(4).SequenceEqual(x.Value)))
                {
                    Frontend.ShowMessageBox(Strings.Menu_Mods_Misc_CustomFont_Invalid, MessageBoxImage.Error);
                    return;
                }

                TextFontTask.NewState = dialog.FileName;
            }

            OnPropertyChanged(nameof(ChooseCustomFontVisibility));
            OnPropertyChanged(nameof(DeleteCustomFontVisibility));
        }

        private void ManageCustomCursor()
        {
            if (!String.IsNullOrEmpty(CustomCursorTask.NewState))
            {
                CustomCursorTask.NewState = "";
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Imagen PNG|*.png"
                };

                if (dialog.ShowDialog() != true)
                    return;

                // PNG file signature check (first 8 bytes)
                byte[] pngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
                byte[] fileBytes = File.ReadAllBytes(dialog.FileName).Take(8).ToArray();

                if (!fileBytes.SequenceEqual(pngHeader))
                {
                    Frontend.ShowMessageBox("El archivo seleccionado no es un PNG valido.", MessageBoxImage.Error);
                    return;
                }

                // Selecting a custom cursor overrides any preset cursor selection
                CursorTypeTask.NewState = Enums.CursorType.Default;
                CustomCursorTask.NewState = dialog.FileName;
            }

            OnPropertyChanged(nameof(ChooseCustomCursorVisibility));
            OnPropertyChanged(nameof(DeleteCustomCursorVisibility));
        }

        private void ManageCustomAvatarBackground()
        {
            if (!String.IsNullOrEmpty(AvatarBackgroundTask.NewState))
            {
                AvatarBackgroundTask.NewState = "";
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Roblox Place file|*.rbxl"
                };

                if (dialog.ShowDialog() != true)
                    return;

                // Roblox place files (.rbxl, binary format) start with this exact 8-byte signature
                byte[] rbxlHeader = { 0x3C, 0x72, 0x6F, 0x62, 0x6C, 0x6F, 0x78, 0x21 }; // "<roblox!"
                byte[] fileBytes = File.ReadAllBytes(dialog.FileName).Take(8).ToArray();

                if (!fileBytes.SequenceEqual(rbxlHeader))
                {
                    Frontend.ShowMessageBox("El archivo seleccionado no parece ser un .rbxl binario valido (debe exportarse desde Roblox Studio como 'Roblox Place', no como .rbxlx XML).", MessageBoxImage.Error);
                    return;
                }

                // Selecting a custom background overrides the built-in old avatar background preset
                OldAvatarBackgroundTask.NewState = false;
                AvatarBackgroundTask.NewState = dialog.FileName;
            }

            OnPropertyChanged(nameof(ChooseCustomAvatarBackgroundVisibility));
            OnPropertyChanged(nameof(DeleteCustomAvatarBackgroundVisibility));
        }

        public ICommand OpenModsFolderCommand => new RelayCommand(OpenModsFolder);

        public ICommand ImportSkyboxTexturesCommand => new RelayCommand(ImportSkyboxTextures);

        public Visibility ChooseCustomFontVisibility => !String.IsNullOrEmpty(TextFontTask.NewState) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility DeleteCustomFontVisibility => !String.IsNullOrEmpty(TextFontTask.NewState) ? Visibility.Visible : Visibility.Collapsed;

        public ICommand ManageCustomFontCommand => new RelayCommand(ManageCustomFont);

        public ICommand ManageCustomCursorCommand => new RelayCommand(ManageCustomCursor);

        public Visibility ChooseCustomCursorVisibility => !String.IsNullOrEmpty(CustomCursorTask.NewState) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility DeleteCustomCursorVisibility => !String.IsNullOrEmpty(CustomCursorTask.NewState) ? Visibility.Visible : Visibility.Collapsed;

        public ICommand ManageCustomAvatarBackgroundCommand => new RelayCommand(ManageCustomAvatarBackground);

        public Visibility ChooseCustomAvatarBackgroundVisibility => !String.IsNullOrEmpty(AvatarBackgroundTask.NewState) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility DeleteCustomAvatarBackgroundVisibility => !String.IsNullOrEmpty(AvatarBackgroundTask.NewState) ? Visibility.Visible : Visibility.Collapsed;

        public ICommand OpenCompatSettingsCommand => new RelayCommand(OpenCompatSettings);

        public ModPresetTask OldAvatarBackgroundTask { get; } = new("OldAvatarBackground", @"ExtraContent\places\Mobile.rbxl", "OldAvatarBackground.rbxl");

        public ModPresetTask OldCharacterSoundsTask { get; } = new("OldCharacterSounds", new()
        {
            { @"content\sounds\action_footsteps_plastic.mp3", "Sounds.OldWalk.mp3"  },
            { @"content\sounds\action_jump.mp3",              "Sounds.OldJump.mp3"  },
            { @"content\sounds\action_get_up.mp3",            "Sounds.OldGetUp.mp3" },
            { @"content\sounds\action_falling.mp3",           "Sounds.Empty.mp3"    },
            { @"content\sounds\action_jump_land.mp3",         "Sounds.Empty.mp3"    },
            { @"content\sounds\action_swim.mp3",              "Sounds.Empty.mp3"    },
            { @"content\sounds\impact_water.mp3",             "Sounds.Empty.mp3"    }
        });

        public EmojiModPresetTask EmojiFontTask { get; } = new();

        public EnumModPresetTask<Enums.CursorType> CursorTypeTask { get; } = new("CursorType", new()
        {
            {
                Enums.CursorType.From2006, new()
                {
                    { @"content\textures\Cursors\KeyboardMouse\ArrowCursor.png",    "Cursor.From2006.ArrowCursor.png"    },
                    { @"content\textures\Cursors\KeyboardMouse\ArrowFarCursor.png", "Cursor.From2006.ArrowFarCursor.png" }
                }
            },
            {
                Enums.CursorType.From2013, new()
                {
                    { @"content\textures\Cursors\KeyboardMouse\ArrowCursor.png",    "Cursor.From2013.ArrowCursor.png"    },
                    { @"content\textures\Cursors\KeyboardMouse\ArrowFarCursor.png", "Cursor.From2013.ArrowFarCursor.png" }
                }
            }
        });

        public FontModPresetTask TextFontTask { get; } = new();

        public CursorModPresetTask CustomCursorTask { get; } = new();

        public AvatarBackgroundModPresetTask AvatarBackgroundTask { get; } = new();

        private void OpenCompatSettings()
        {
            string path = new RobloxPlayerData().ExecutablePath;

            if (File.Exists(path))
                PInvoke.SHObjectProperties(HWND.Null, SHOP_TYPE.SHOP_FILEPATH, path, "Compatibility");
            else
                Frontend.ShowMessageBox(Strings.Common_RobloxNotInstalled, MessageBoxImage.Error);

        }
    }
}

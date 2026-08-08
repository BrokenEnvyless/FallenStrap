using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using FallenStrap.Enums.FlagPresets;

namespace FallenStrap.UI.ViewModels.Settings
{
    public class FastFlagsViewModel : NotifyPropertyChangedViewModel
    {
        private Dictionary<string, object>? _preResetFlags;

        public event EventHandler? RequestPageReloadEvent;
        
        public event EventHandler? OpenFlagEditorEvent;

        private void OpenFastFlagEditor() => OpenFlagEditorEvent?.Invoke(this, EventArgs.Empty);

        public ICommand OpenFastFlagEditorCommand => new RelayCommand(OpenFastFlagEditor);

        public Visibility CanShowFastFlagEditor => App.IsStudioInstalled ? Visibility.Visible : Visibility.Collapsed;

        public bool UseFastFlagManager
        {
            get => App.Settings.Prop.UseFastFlagManager;
            set => App.Settings.Prop.UseFastFlagManager = value;
        }

        public IReadOnlyDictionary<MSAAMode, string?> MSAALevels => FastFlagManager.MSAAModes;

        public MSAAMode SelectedMSAALevel
        {
            get => MSAALevels.FirstOrDefault(x => x.Value == App.FastFlags.GetPreset("Rendering.MSAA")).Key;
            set => App.FastFlags.SetPreset("Rendering.MSAA", MSAALevels[value]);
        }

        public bool FixDisplayScaling
        {
            get => App.FastFlags.GetPreset("Rendering.DisableScaling") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisableScaling", value ? "True" : null);
        }

        public RenderingBackend SelectedRenderingBackend
        {
            get
            {
                if (App.FastFlags.GetPreset("Rendering.Backend.Vulkan") == "True")
                    return RenderingBackend.Vulkan;
                if (App.FastFlags.GetPreset("Rendering.Backend.OpenGL.Enable") == "True")
                    return RenderingBackend.OpenGL;
                if (App.FastFlags.GetPreset("Rendering.Backend.D3D11") == "True")
                    return RenderingBackend.D3D11;
                return RenderingBackend.Default;
            }
            set
            {
                // Clear all backend-related flags first
                App.FastFlags.SetPreset("Rendering.Backend.Vulkan", null);
                App.FastFlags.SetPreset("Rendering.Backend.D3D11", null);
                App.FastFlags.SetPreset("Rendering.Backend.OpenGL", null);

                switch (value)
                {
                    case RenderingBackend.Vulkan:
                        App.FastFlags.SetPreset("Rendering.Backend.Vulkan", "True");
                        break;
                    case RenderingBackend.OpenGL:
                        // OpenGL requires Direct3D 11 to be explicitly disabled to take effect
                        App.FastFlags.SetPreset("Rendering.Backend.OpenGL", "True");
                        break;
                    case RenderingBackend.D3D11:
                        App.FastFlags.SetPreset("Rendering.Backend.D3D11", "True");
                        break;
                }
            }
        }

        public bool GraySky
        {
            get => App.FastFlags.GetPreset("Rendering.GraySky") == "True";
            set => App.FastFlags.SetPreset("Rendering.GraySky", value ? "True" : null);
        }

        public bool DisableGrass
        {
            get => App.FastFlags.GetPreset("Rendering.DisableGrass.MinDistance") == "0";
            set => App.FastFlags.SetPreset("Rendering.DisableGrass", value ? "0" : null);
        }

        public bool DisableShadows
        {
            get => App.FastFlags.GetPreset("Rendering.DisableShadows") == "0";
            set => App.FastFlags.SetPreset("Rendering.DisableShadows", value ? "0" : null);
        }

        public bool PauseVoxelizer
        {
            get => App.FastFlags.GetPreset("Rendering.PauseVoxelizer") == "True";
            set => App.FastFlags.SetPreset("Rendering.PauseVoxelizer", value ? "True" : null);
        }

        public bool DisablePostFx
        {
            get => App.FastFlags.GetPreset("Rendering.DisablePostFx") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisablePostFx", value ? "True" : null);
        }

        public bool DisableTextureCompositor
        {
            get => App.FastFlags.GetPreset("Rendering.TextureCompositor") == "0";
            set => App.FastFlags.SetPreset("Rendering.TextureCompositor", value ? "0" : null);
        }

        public bool SkipTextureMipLevels
        {
            get => App.FastFlags.GetPreset("Rendering.TextureQuality.SkipMips") == "-1";
            set => App.FastFlags.SetPreset("Rendering.TextureQuality.SkipMips", value ? "-1" : null);
        }

        public IReadOnlyDictionary<int, string?> GraphicsQualityLevels => FastFlagManager.GraphicsQualityLevels;

        public int SelectedGraphicsQualityLevel
        {
            get => GraphicsQualityLevels.FirstOrDefault(x => x.Value == App.FastFlags.GetPreset("Rendering.GraphicsQualityOverride")).Key;
            set => App.FastFlags.SetPreset("Rendering.GraphicsQualityOverride", GraphicsQualityLevels[value]);
        }

        public bool MeshDetails
        {
            get => App.FastFlags.GetPreset("Rendering.MeshDetails.MainViewHigh") == "10000";
            set => App.FastFlags.SetPreset("Rendering.MeshDetails", value ? "10000" : null);
        }

        public string FpsLimit
        {
            get => App.FastFlags.GetPreset("Performance.FpsUnlocker.Enabled") ?? "";
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    App.FastFlags.SetPreset("Performance.FpsUnlocker.Enabled", null);
                    return;
                }

                // only accept whole numbers, ignore invalid input rather than crash
                if (int.TryParse(value, out int fps) && fps > 0)
                    App.FastFlags.SetPreset("Performance.FpsUnlocker.Enabled", fps.ToString());
            }
        }

        public IReadOnlyDictionary<TextureQuality, string?> TextureQualities => FastFlagManager.TextureQualityLevels;

        public TextureQuality SelectedTextureQuality
        {
            get => TextureQualities.Where(x => x.Value == App.FastFlags.GetPreset("Rendering.TextureQuality.Level")).FirstOrDefault().Key;
            set
            {
                if (value == TextureQuality.Default)
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality", null);
                }
                else
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality.OverrideEnabled", "True");
                    App.FastFlags.SetPreset("Rendering.TextureQuality.Level", TextureQualities[value]);
                }
            }
        }
        public bool ResetConfiguration
        {
            get => _preResetFlags is not null;

            set
            {
                if (value)
                {
                    _preResetFlags = new(App.FastFlags.Prop);
                    App.FastFlags.Prop.Clear();
                }
                else
                {
                    App.FastFlags.Prop = _preResetFlags!;
                    _preResetFlags = null;
                }

                RequestPageReloadEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

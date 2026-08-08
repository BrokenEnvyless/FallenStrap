using FallenStrap.Enums.FlagPresets;

namespace FallenStrap
{
    public class FastFlagManager : JsonManager<Dictionary<string, object>>
    {
        private Dictionary<string, object> OriginalProp = new();

        public override string ClassName => nameof(FastFlagManager);

        public override string LOG_IDENT_CLASS => ClassName;

        public override string FileName => "ClientAppSettings.json";

        public override string FileLocation => Path.Combine(Paths.Modifications, "ClientSettings", FileName);

        public bool Changed => !OriginalProp.SequenceEqual(Prop);

        public static IReadOnlyDictionary<string, string> PresetFlags = new Dictionary<string, string>
        {
            { "Rendering.ManualFullscreen", "FFlagHandleAltEnterFullscreenManually" },
            { "Rendering.DisableScaling", "DFFlagDisableDPIScale" },
            { "Rendering.MSAA", "FIntDebugForceMSAASamples" },

            { "Rendering.TextureQuality.OverrideEnabled", "DFFlagTextureQualityOverrideEnabled" },
            { "Rendering.TextureQuality.Level", "DFIntTextureQualityOverride" },

            { "Rendering.Backend.Vulkan", "FFlagDebugGraphicsPreferVulkan" },
            { "Rendering.Backend.D3D11", "FFlagDebugGraphicsPreferD3D11" },
            { "Rendering.Backend.OpenGL.Enable", "FFlagDebugGraphicsPreferOpenGL" },
            { "Rendering.Backend.OpenGL.DisableD3D11", "FFlagDebugGraphicsDisableDirect3D11" },

            { "Rendering.GraySky", "FFlagDebugSkyGray" },

            { "Rendering.DisableGrass.MinDistance", "FIntFRMMinGrassDistance" },
            { "Rendering.DisableGrass.MaxDistance", "FIntFRMMaxGrassDistance" },
            { "Rendering.DisableGrass.DetailStrands", "FIntRenderGrassDetailStrands" },
            { "Rendering.DisableGrass.HeightScaler", "FIntRenderGrassHeightScaler" },

            { "Rendering.GraphicsQualityOverride", "DFIntDebugFRMQualityLevelOverride" },

            { "Performance.FpsUnlocker.Enabled", "DFIntTaskSchedulerTargetFps" },

            { "Rendering.MeshDetails.MainViewHigh", "DFIntCullFactorPixelThresholdMainViewHighQuality" },
            { "Rendering.MeshDetails.MainViewLow", "DFIntCullFactorPixelThresholdMainViewLowQuality" },
            { "Rendering.MeshDetails.ShadowMapHigh", "DFIntCullFactorPixelThresholdShadowMapHighQuality" },
            { "Rendering.MeshDetails.ShadowMapLow", "DFIntCullFactorPixelThresholdShadowMapLowQuality" },

            // Nuevos presets agregados - basados en flags documentados por la comunidad (ver flags.json de Fallenware)
            { "Rendering.DisableShadows", "FIntRenderShadowIntensity" },
            { "Rendering.PauseVoxelizer", "DFFlagDebugPauseVoxelizer" },
            { "Rendering.DisablePostFx", "FFlagDisablePostFx" },
            { "Rendering.TextureCompositor", "DFIntTextureCompositorActiveJobs" },
            { "Rendering.TextureQuality.SkipMips", "DFIntPerformanceControlTextureQualityBestUtility" },
        };

        // Roblox's graphics quality slider (1-10) doesn't map 1:1 to the internal FRM value.
        // Mapping sourced from community-documented Bloxstrap FastFlag references.
        public static IReadOnlyDictionary<int, string?> GraphicsQualityLevels => new Dictionary<int, string?>
        {
            { 0, null },  // Automatic / not overridden
            { 1, "1" },
            { 2, "2" },
            { 3, "6" },
            { 4, "7" },
            { 5, "11" },
            { 6, "14" },
            { 7, "15" },
            { 8, "17" },
            { 9, "18" },
            { 10, "21" },
        };

        public static IReadOnlyDictionary<MSAAMode, string?> MSAAModes => new Dictionary<MSAAMode, string?>
        {
            { MSAAMode.Default, null },
            { MSAAMode.x1, "1" },
            { MSAAMode.x2, "2" },
            { MSAAMode.x4, "4" }
        };

        public static IReadOnlyDictionary<TextureQuality, string?> TextureQualityLevels => new Dictionary<TextureQuality, string?>
        {
            { TextureQuality.Default, null },
            { TextureQuality.Level0, "0" },
            { TextureQuality.Level1, "1" },
            { TextureQuality.Level2, "2" },
            { TextureQuality.Level3, "3" },
        };

        // all fflags are stored as strings
        // to delete a flag, set the value as null
        public void SetValue(string key, object? value)
        {
            const string LOG_IDENT = "FastFlagManager::SetValue";

            if (value is null)
            {
                if (Prop.ContainsKey(key))
                    App.Logger.WriteLine(LOG_IDENT, $"Deletion of '{key}' is pending");

                Prop.Remove(key);
            }
            else
            {
                if (Prop.ContainsKey(key))
                {
                    if (key == Prop[key].ToString())
                        return;

                    App.Logger.WriteLine(LOG_IDENT, $"Changing of '{key}' from '{Prop[key]}' to '{value}' is pending");
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Setting of '{key}' to '{value}' is pending");
                }

                Prop[key] = value.ToString()!;
            }
        }

        // this returns null if the fflag doesn't exist
        public string? GetValue(string key)
        {
            // check if we have an updated change for it pushed first
            if (Prop.TryGetValue(key, out object? value) && value is not null)
                return value.ToString();

            return null;
        }

        public void SetPreset(string prefix, object? value)
        {
            foreach (var pair in PresetFlags.Where(x => x.Key.StartsWith(prefix)))
                SetValue(pair.Value, value);
        }

        public void SetPresetEnum(string prefix, string target, object? value)
        {
            foreach (var pair in PresetFlags.Where(x => x.Key.StartsWith(prefix)))
            {
                if (pair.Key.StartsWith($"{prefix}.{target}"))
                    SetValue(pair.Value, value);
                else
                    SetValue(pair.Value, null);
            }
        }

        public string? GetPreset(string name)
        {
            if (!PresetFlags.ContainsKey(name))
            {
                App.Logger.WriteLine("FastFlagManager::GetPreset", $"Could not find preset {name}");
                Debug.Assert(false, $"Could not find preset {name}");
                return null;
            }

            return GetValue(PresetFlags[name]);
        }

        public T GetPresetEnum<T>(IReadOnlyDictionary<T, string> mapping, string prefix, string value) where T : Enum
        {
            foreach (var pair in mapping)
            {
                if (pair.Value == "None")
                    continue;

                if (GetPreset($"{prefix}.{pair.Value}") == value)
                    return pair.Key;
            }

            return mapping.First().Key;
        }

        public override void Save()
        {
            // convert all flag values to strings before saving

            foreach (var pair in Prop)
                Prop[pair.Key] = pair.Value.ToString()!;

            base.Save();

            // clone the dictionary
            OriginalProp = new(Prop);
        }

        public override bool Load(bool alertFailure = true)
        {
            bool result = base.Load(alertFailure);

            // clone the dictionary
            OriginalProp = new(Prop);

            if (GetPreset("Rendering.ManualFullscreen") != "False")
                SetPreset("Rendering.ManualFullscreen", "False");

            return result;
        }
    }
}

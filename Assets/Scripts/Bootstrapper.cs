using Core.Data;
using Core.Systems;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public static class Bootstrapper {
    
    
    [RuntimeInitializeOnLoadMethod]
    public static async void Initialize() {
        // Load player options
        Core.Systems.GamePlayerOptions.Load();

        var masterMixer = await Addressables.LoadAssetAsync<AudioMixer>("Master").Task;
        var soundCategory = await Addressables.LoadAssetAsync<AudioCategoryData>("Sounds").Task;
        var musicCategory = await Addressables.LoadAssetAsync<AudioCategoryData>("Music").Task;
        
        var categories = new AudioCategoryData[] { soundCategory, musicCategory };
        
        AudioManager.InitializeAudioSystem(true, true, true, categories, masterMixer);
    }
}
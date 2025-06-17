using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace Core.Editors {
    public static class AddressableExtensions {
        public static void SetAddressableGroup(this Object obj, string groupName) {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
    
            if (settings) {
                var group = settings.FindGroup(groupName);
                if (!group)
                    group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
    
                var assetpath = AssetDatabase.GetAssetPath(obj);
                var guid = AssetDatabase.AssetPathToGUID(assetpath);
    
                var e = settings.CreateOrMoveEntry(guid, group, false, false);
                var entriesAdded = new List<AddressableAssetEntry> {e};
    
                group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            }
        }

        public static void SetAddressableLabel(this Object obj, string label) {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

    
            if (settings) {

                if (settings.GetLabels().Contains(label) == false) {
                    Debug.Log("No label named " + label);
                    return;
                }

                var assetpath = AssetDatabase.GetAssetPath(obj);
                var guid = AssetDatabase.AssetPathToGUID(assetpath);

                var assetEntry = settings.FindAssetEntry(guid);

                assetEntry.SetLabel(label, true, true);

                if (assetEntry != null) {

                    var entries = new List<AddressableAssetEntry> { assetEntry };
        
                    assetEntry.parentGroup.SetDirty(AddressableAssetSettings.ModificationEvent.LabelAdded, entries, false, true);
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.LabelAdded, entries, true, false);
                }
            }
        }
    }
}

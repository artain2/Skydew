// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEditor.AddressableAssets;
// using UnityEditor.AddressableAssets.Settings;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
//
// namespace DrawerTools
// {
//
//     public static class DTAddressables
//     {
//         // Get Address
//         //_____________________________________________________
//         public static bool TryGetAddressableAddress(UnityEngine.Object asset, out string address)
//         {
//             if (!TryFindAddressableAssetEntry(asset, out var entry))
//             {
//                 address = "";
//                 return false;
//             }
//             address = entry.address;
//             return true;
//         }
//         public static string GetAddressableAddress(UnityEngine.Object asset)
//         {
//             TryGetAddressableAddress(asset, out var result);
//             return result;
//         }
//
//         // Set Address
//         //_____________________________________________________
//         public static bool SetAddressableAddress(UnityEngine.Object asset, string address)
//         {
//             if (string.IsNullOrEmpty(address))
//             {
//                 Debug.LogError($"Can not set an empty adressables ID.");
//                 return false;
//             }
//             if (TryFindAddressableAssetEntry(asset, out var entry))
//             {
//                 entry.address = address;
//                 return true;
//             }
//             return false;
//         }
//
//         // Set as Addressable
//         //_____________________________________________________
//         public static void SetObjectAddressable(UnityEngine.Object asset, string address, string groupName = null, string label = null)
//         {
//             var aaSettings = AddressableAssetSettingsDefaultObject.Settings;
//             var settings = AddressableAssetSettingsDefaultObject.Settings;
//             var group = settings.DefaultGroup;
//             if (groupName != null)
//             {
//                 group = settings.FindGroup(groupName);
//                 if (group == null)
//                 {
//                     Debug.LogError($"Не найдена группа {groupName}, объект не будет помечен как Addressable");
//                     return;
//                 }
//             }
//             var assetFound = TryGetAssetGUID(asset, out var guid);
//             if (!assetFound)
//             {
//                 return;
//             }
//
//             var entry = settings.CreateOrMoveEntry(guid, group);
//             entry.address = address;
//             if (label != null)
//             {
//                 entry.labels.Add(label);
//             }
//             settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
//         }
//
//         public static void SetObjectNotAddressable(UnityEngine.Object asset)
//         {
//             var aaSettings = AddressableAssetSettingsDefaultObject.Settings;
//             var settings = AddressableAssetSettingsDefaultObject.Settings;
//             var group = settings.DefaultGroup;
//             var assetFound = TryGetAssetGUID(asset, out var guid);
//             if (!assetFound)
//             {
//                 return;
//             }
//
//             var entry = settings.RemoveAssetEntry(guid, group);
//             settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
//         }
//
//         private static bool TryFindAddressableAssetEntry(UnityEngine.Object asset, out AddressableAssetEntry entry)
//         {
//             entry = null;
//             var assetFound = TryGetAssetGUID(asset, out var guid);
//             if (!assetFound)
//             {
//                 return false;
//             }
//
//             var found = TryFindAddressableAssetEntry(guid, out entry);
//             return found;
//         }
//
//         private static bool TryFindAddressableAssetEntry(string guid, out AddressableAssetEntry entry)
//         {
//             entry = null;
//             if (!IsAssetInAssets(guid))
//             {
//                 return false;
//             }
//             AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.Settings;
//             if (aaSettings == null)
//             {
//                 return false;
//             }
//             entry = aaSettings.FindAssetEntry(guid);
//             return entry != null;
//         }
//
//         private static bool TryGetAssetGUID(UnityEngine.Object asset, out string guid)
//         {
//             var assetFound = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out long _);
//             return assetFound;
//         }
//
//         private static bool IsAssetInAssets(string guid)
//         {
//             var path = AssetDatabase.GUIDToAssetPath(guid);
//             return path.ToLower().Contains("assets");
//         }
//
//         public static T Load<T>(AssetReference reference) where T : UnityEngine.Object
//         {
//             var path = AssetDatabase.GetAssetPath(reference.editorAsset);
//             var asset = DTAssets.LoadAsset<T>(path);
//             return asset;
//         }
//         
//         public static T Load<T>(string address) where T : UnityEngine.Object
//         {
//             var path = GetAssetPath(address);
//             var asset = DTAssets.LoadAsset<T>(path);
//             return asset;
//         }
//         
//         public static string GetAssetPath(string address)
//         {
//             var aaSettings = AddressableAssetSettingsDefaultObject.Settings;
//             if (aaSettings == null)
//                 throw new ArgumentException(nameof(aaSettings));
//  
//             var entries = from addressableAssetGroup in aaSettings.groups
//                 from entrie in addressableAssetGroup.entries
//                 select entrie;
//  
//             foreach (var entry in entries)
//             {
//                 var allEntries = new List<AddressableAssetEntry>();
//                 entry.GatherAllAssets(allEntries, true, true, false);
//                 foreach (var assetEntry in allEntries)
//                 {
//                     if (assetEntry.address == address)
//                     {
//                         return assetEntry.AssetPath;
//                     }
//                 }
//             }
//  
//             throw new ArgumentException("error key:" + address);
//         }
//     }
// }
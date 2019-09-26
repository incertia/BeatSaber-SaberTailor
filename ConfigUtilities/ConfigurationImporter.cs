﻿using IPA.Config;
using System;
using System.Globalization;
using UnityEngine;

namespace SaberTailor.ConfigUtilities
{
    /// <summary>
    /// Imports the old configuration from ModPrefs. Just YEET this once ModPrefs support is dropped!
    /// </summary>
    internal class ConfigurationImporter
    {
        private static float Length;
        private static bool IsTrailEnabled;
        private static int TrailLength;

        private static Vector3 GripLeftPosition;
        private static Vector3 GripRightPosition;

        private static Vector3 GripLeftRotation;
        private static Vector3 GripRightRotation;

        private static bool ModifyMenuHiltGrip;

        /// <summary>
        /// ONLY USED FOR A SPECIFIC PURPOSE IN "Configuration.cs". DO NOT USE ELSEWHERE!
        /// </summary>
        internal static void ImportSettingsFromModPrefs()
        {
            try
            {
#pragma warning disable CS0618 // ModPrefs is obsolete
                // Import trail configuration
                IsTrailEnabled = ModPrefs.GetBool(Plugin.PluginName, nameof(IsTrailEnabled), true, true);
                Configuration.IsTrailEnabled = IsTrailEnabled;

                TrailLength = ModPrefs.GetInt(Plugin.PluginName, nameof(TrailLength), 20, true);
                Configuration.TrailLength = Math.Max(5, Math.Min(100, TrailLength));

                // Check trail modification vars - if these are unchecked from default values, then just disable these
                if (IsTrailEnabled == true && TrailLength == 20)
                {
                    Configuration.IsTrailModEnabled = false;
                }
                else
                {
                    Configuration.IsTrailModEnabled = true;
                }

                // Import grip position settings, convert old centimeter values to millimeter
                GripLeftPosition = ParseVector3(ModPrefs.GetString(Plugin.PluginName, nameof(GripLeftPosition), "0,0,0", true));
                Configuration.GripLeftPositionCfg = new StoreableIntVector3()
                {
                    x = (int)Math.Round(Mathf.Clamp(GripLeftPosition.x, -50f, 50f) * 10),
                    y = (int)Math.Round(Mathf.Clamp(GripLeftPosition.y, -50f, 50f) * 10),
                    z = (int)Math.Round(Mathf.Clamp(GripLeftPosition.z, -50f, 50f) * 10)
                };
                GripLeftRotation = ParseVector3(ModPrefs.GetString(Plugin.PluginName, nameof(GripLeftRotation), "0,0,0", true));
                Configuration.GripLeftRotationCfg = new StoreableIntVector3()
                {
                    x = (int)Math.Round(GripLeftRotation.x),
                    y = (int)Math.Round(GripLeftRotation.y),
                    z = (int)Math.Round(GripLeftRotation.z)
                };

                GripRightPosition = ParseVector3(ModPrefs.GetString(Plugin.PluginName, nameof(GripRightPosition), "0,0,0", true));
                Configuration.GripRightPositionCfg = new StoreableIntVector3()
                {
                    x = (int)Math.Round(Mathf.Clamp(GripRightPosition.x, -50f, 50f) * 10),
                    y = (int)Math.Round(Mathf.Clamp(GripRightPosition.y, -50f, 50f) * 10),
                    z = (int)Math.Round(Mathf.Clamp(GripRightPosition.z, -50f, 50f) * 10)
                };
                GripRightRotation = ParseVector3(ModPrefs.GetString(Plugin.PluginName, nameof(GripRightRotation), "0,0,0", true));
                Configuration.GripRightRotationCfg = new StoreableIntVector3()
                {
                    x = (int)Math.Round(GripRightRotation.x),
                    y = (int)Math.Round(GripRightRotation.y),
                    z = (int)Math.Round(GripRightRotation.z)
                };

                Configuration.ModifyMenuHiltGrip = ModPrefs.GetBool(Plugin.PluginName, nameof(ModifyMenuHiltGrip), false, true);
#pragma warning restore CS0618 // ModPrefs is obsolete

                // Set default values for new config variables not present in old config files
                Configuration.ConfigVersion = 2;
                Configuration.IsSaberScaleModEnabled = false;
                Configuration.SaberScaleHitbox = false;
                Configuration.SaberLengthCfg = 100;
                Configuration.SaberGirthCfg = 100;

                // Save configuration in the new format
                Configuration.Save();
                MarkAsExported();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void MarkAsExported()
        {
#pragma warning disable CS0618 // ModPrefs is obsolete
            ModPrefs.SetBool(Plugin.PluginName, "IsExportedToNewConfig", true);
#pragma warning restore CS0618 // ModPrefs is obsolete
        }

        private static Vector3 ParseVector3(string originalString)
        {
            string[] components = originalString.Trim().Split(',');
            Vector3 parsedVector = Vector3.zero;

            if (components.Length != 3) return parsedVector;

            TryParseInvariantFloat(components[0], out parsedVector.x);
            TryParseInvariantFloat(components[1], out parsedVector.y);
            TryParseInvariantFloat(components[2], out parsedVector.z);

            return parsedVector;
        }

        /// <summary>
        /// Tries to parse a float using invariant culture.
        /// </summary>
        /// <param name="number">The string containing the float to parse.</param>
        /// <param name="result">The parsed float, if successful.</param>
        /// <returns>True on success, false on failure.</returns>
        private static bool TryParseInvariantFloat(string number, out float result)
        {
            return float.TryParse(
                number,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out result
            );
        }
    }
}

﻿using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Lights;
using Windows.System;
using static Dynamic_Lighting_Key_Indicator.KeyStatesHandler;

namespace Dynamic_Lighting_Key_Indicator
{
    using RGBTuple = (int R, int G, int B);

    internal static class ColorSetter
    {
        private static Windows.UI.Color _keyboardMainColor;
        private static LampArray _currentDevice;
        private static List<int> _monitoredIndices;

        public static Windows.UI.Color KeyboardMainColor => _keyboardMainColor;
        public static LampArray CurrentDevice => _currentDevice;
        public static List<int> MonitoredIndices => _monitoredIndices;

        public static void DefineKeyboardMainColor_FromRGB(RGBTuple color)
        {

            _keyboardMainColor = Windows.UI.Color.FromArgb(255, (byte)color.R, (byte)color.G, (byte)color.B);
        }

        public static void DefineKeyboardMainColor_FromName(Windows.UI.Color color)
        {
            _keyboardMainColor = color;
        }

        public static void SetCurrentDevice(LampArray device)
        {
            _currentDevice = device;
        }

        public static void SetInitialDefaultKeyboardColor(LampArray lampArray)
        {
            lampArray.SetColor(KeyboardMainColor);
        }


        public static void SetMonitoredKeysColor(List<KeyStatesHandler.MonitoredKey> monitoredKeys, LampArray lampArray = null)
        {
            if (lampArray == null)
            {
                if (CurrentDevice == null)
                {
                    throw new ArgumentNullException("LampArray must be defined.");
                }
                else
                {
                    lampArray = CurrentDevice;
                }
            }

            Windows.UI.Color[] colors = new Windows.UI.Color[monitoredKeys.Count];
            VirtualKey[] keys = new VirtualKey[monitoredKeys.Count];

            // Build arrays of colors and keys
            foreach (var key in monitoredKeys)
            {
                VirtualKey vkCode = (VirtualKey)key.key;
                Windows.UI.Color color;

                if (key.IsOn())
                {
                    if (key.IsOn())
                    {
                        if (key.onColor != default)
                            color = Windows.UI.Color.FromArgb(255, (byte)key.onColor.R, (byte)key.onColor.G, (byte)key.onColor.B);
                        else
                            color = KeyboardMainColor;
                    }
                    else
                    {
                        if (key.offColor != default)
                            color = Windows.UI.Color.FromArgb(255, (byte)key.offColor.R, (byte)key.offColor.G, (byte)key.offColor.B);
                        else
                            color = KeyboardMainColor;
                    }

                }
                else
                {
                    if (key.offColor != default)
                        color = Windows.UI.Color.FromArgb(255, (byte)key.offColor.R, (byte)key.offColor.G, (byte)key.offColor.B);
                    else
                        color = KeyboardMainColor;
                }

                colors[monitoredKeys.IndexOf(key)] = color;
                keys[monitoredKeys.IndexOf(key)] = vkCode;
            }

            lampArray.SetColorsForKeys(colors, keys);
        }


        // ------------------------------------------- Unused But Maybe Useful Later ------------------------------------------------------------

        #region Unused
        public static void SetSpecificKeysToColor(LampArray lampArray, VirtualKey[] keys, Windows.UI.Color color)
        {
            Windows.UI.Color[] colors = Enumerable.Repeat(color, keys.Length).ToArray();
            lampArray.SetColorsForKeys(colors, keys);
            lampArray.BrightnessLevel = 1.0f;
        }

        public static void SetKeyboardColorExceptMonitoredKeys(List<KeyStatesHandler.MonitoredKey> monitoredKeys, LampArray lampArray = null)
        {

            if (lampArray == null)
            {
                if (CurrentDevice == null)
                {
                    throw new ArgumentNullException("LampArray must be defined.");
                }
                else
                {
                    lampArray = CurrentDevice;
                }
            }

            List<int> monitoredKeyIndices = MonitoredIndices;

            if (monitoredKeyIndices == null)
            {
                monitoredKeyIndices = UpdateMonitoredLampArrayIndices(lampArray);
            }


            Windows.UI.Color[] colors = new Windows.UI.Color[lampArray.LampCount - monitoredKeys.Count];
            int[] keys = new int[lampArray.LampCount - monitoredKeys.Count];

            // Build the arrays of colors and keys
            for (int i = 0; i < lampArray.LampCount; i++)
            {
                if (!monitoredKeyIndices.Contains(i))
                {
                    colors[i] = KeyboardMainColor;
                    keys[i] = i;
                }
            }

            lampArray.SetColorsForIndices(colors, keys);
        }

        // Use this info to set the colors of all the other keys not being monitored, so we don't cause the monitored keys to flicker
        public static List<int> UpdateMonitoredLampArrayIndices(LampArray lampArray)
        {
            List<int> monitoredIndices = [];
            foreach (var key in KeyStatesHandler.monitoredKeys)
            {
                VirtualKey virtualKey = (VirtualKey)key.key;
                monitoredIndices.AddRange(lampArray.GetIndicesForKey(virtualKey));
            }
            _monitoredIndices = monitoredIndices;

            return monitoredIndices;

        }
        #endregion
    }
}

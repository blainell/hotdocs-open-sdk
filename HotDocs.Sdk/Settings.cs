/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     This <c>Settings</c> class provides a means to associate a setting with a string value.
    ///     Also converting settings from <c>Tristate</c> boolean to string and vice versa
    ///     are provided here.
    /// </summary>
    public class Settings
    {
        /// <summary>
        ///     <c>_settings</c> uses the generic <c>Dictionary&lt;string, string&gt;</c> class to match settings with string
        ///     values.
        /// </summary>
        protected Dictionary<string, string> _settings = new Dictionary<string, string>();

        /// <summary>
        ///     Convert a Tristate value to a string value.
        /// </summary>
        /// <param name="triValue">A value that can be either true, false, or unanswered.</param>
        /// <returns>A string if the value is true or false, or null if it is neither.</returns>
        protected string TristateToString(Tristate triValue)
        {
            switch (triValue)
            {
                case Tristate.True:
                    return "true";
                case Tristate.False:
                    return "false";
            }
            return null;
        }

        /// <summary>
        ///     Convert a string value to a Tristate value.
        /// </summary>
        /// <param name="str">A string representation of a Tristate value ("true", "false", or null).</param>
        /// <returns>The <c>Tristate</c> value represented by the given string.</returns>
        protected Tristate StringToTristate(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                switch (str.ToLower())
                {
                    case "true":
                        return Tristate.True;
                    case "false":
                        return Tristate.False;
                }
            }
            return Tristate.Default;
        }

        /// <summary>
        ///     Reads a string value from the settings dictionary and returns it as a Tristate value.
        /// </summary>
        /// <param name="settingName">The name of the setting to be set.</param>
        /// <returns></returns>
        protected Tristate GetSettingTristate(string settingName)
        {
            return StringToTristate(GetSettingString(settingName));
        }

        /// <summary>
        ///     Reads a string from the settings dictionary and returns it as a string (or null if it is not found).
        /// </summary>
        /// <param name="settingName">The name of the setting to be set.</param>
        /// <returns></returns>
        protected string GetSettingString(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
                throw new ArgumentNullException("The settingName cannot be null!");

            string val = null;
            _settings.TryGetValue(settingName, out val);
            return val;
        }

        /// <summary>
        ///     Sets a value in the settings dictionary, or if the value is null, removes it from the dictionary.
        /// </summary>
        /// <param name="settingName">The name of the setting to be set.</param>
        /// <param name="settingValue">The value of the setting.</param>
        protected void SetSettingString(string settingName, string settingValue)
        {
            if (string.IsNullOrEmpty(settingName))
                throw new ArgumentNullException("The settingName cannot be null!");

            if (settingValue != null)
                _settings[settingName] = settingValue;
            else
                _settings.Remove(settingName);
        }
    }
}
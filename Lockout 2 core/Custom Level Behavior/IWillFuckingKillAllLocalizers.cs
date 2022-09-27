using LevelGeneration;
using Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lockout_2_core.Custom_Level_Behavior
{
    public class IWillFuckingKillAllLocalizers
    {
        public TerminalLineType lineType { get; set; } = TerminalLineType.Normal;
        public string Output { get; set; } = "";
        public float Time { get; set; } = 0;
    }

    public class LocalizerGenocideReal
    {
        public static LocalizedText GenerateLocalizedText(string text)
        {
            var localizedText = new LocalizedText();
            localizedText.UntranslatedText = text;
            localizedText.Id = 0;
            return localizedText;
        }
    }
}

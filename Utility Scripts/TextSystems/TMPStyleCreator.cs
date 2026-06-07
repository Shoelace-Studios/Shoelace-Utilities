using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

namespace ShoelaceStudios.Utilities.TextSystems
{
    public class TMPStyleCreator : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;

        public string openingTags;
        public string closingTags;

        public enum TextWeight
        {
            Regular,
            Thin,
            Black
        }

        public TextWeight weight;

        //[Button("Generate Style Tags")]
        public void ReadValuesFromTMP()
        {
            StringBuilder openSB = new();
            StringBuilder closeSB = new();

            if ((targetText.fontStyle & FontStyles.Bold) != 0)
            {
                openSB.Append("<b>");
                closeSB.Append("</b>");
            }

            if ((targetText.fontStyle & FontStyles.Italic) != 0)
            {
                openSB.Append("<i>");
                closeSB.Append("</i>");
            }

            if ((targetText.fontStyle & FontStyles.UpperCase) != 0)
            {
                openSB.Append("<uppercase>");
                closeSB.Append("</uppercase>");
            }

            if ((targetText.fontStyle & FontStyles.LowerCase) != 0)
            {
                openSB.Append("<lowercase>");
                closeSB.Append("</lowercase>");
            }

            string fontAsset = targetText.font.ToString();

            fontAsset = fontAsset.Replace(" (TMPro.TMP_FontAsset)", string.Empty);

            openSB.Append($"<font=\"{fontAsset}\">");
            closeSB.Append("</font>");

            float textSize = targetText.fontSize;
            openSB.Append($"<size={textSize}pt>");
            closeSB.Append("</size>");

            Color textColor = targetText.color;
            string textColorRGB = ColorUtility.ToHtmlStringRGB(textColor);
            openSB.Append($"<color=#{textColorRGB}>");
            closeSB.Append($"</color>");

            float characterSpacing = targetText.characterSpacing;
            characterSpacing /= 100;
            string cSpacing = characterSpacing.ToString("N3", CultureInfo.InvariantCulture);

            if (characterSpacing != 0)
            {
                openSB.Append($"<cspace={cSpacing}em>");
                closeSB.Append($"</cspace>");
            }

            float lineSpacing = targetText.lineSpacing;
            lineSpacing /= 100;
            string lSpacing = lineSpacing.ToString("N3", CultureInfo.InvariantCulture);

            if (lineSpacing != 0)
            {
                openSB.Append($"<line-height={cSpacing}em>");
                closeSB.Append($"</line-height>");
            }

            if (weight == TextWeight.Black)
            {
                openSB.Append($"<font-weight={"900"}>");
                closeSB.Append("</font-weight>");
            }

            openingTags = openSB.ToString();
            closingTags = closeSB.ToString();
        }
    }
}

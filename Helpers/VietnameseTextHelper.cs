using System.Globalization;
using System.Text;

namespace Final_VS1.Helpers
{
    public static class VietnameseTextHelper
    {
        private static readonly string[] VietnameseChars = new[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Handle Vietnamese specific characters
            for (int i = 1; i < VietnameseChars.Length; i++)
            {
                for (int j = 0; j < VietnameseChars[i].Length; j++)
                {
                    result = result.Replace(VietnameseChars[i][j], VietnameseChars[0][i - 1]);
                }
            }

            return result;
        }

        public static bool ContainsIgnoreDiacritics(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return false;

            var normalizedSource = RemoveDiacritics(source.ToLower());
            var normalizedTarget = RemoveDiacritics(target.ToLower());

            return normalizedSource.Contains(normalizedTarget);
        }

        public static bool StartsWithIgnoreDiacritics(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return false;

            var normalizedSource = RemoveDiacritics(source.ToLower());
            var normalizedTarget = RemoveDiacritics(target.ToLower());

            return normalizedSource.StartsWith(normalizedTarget);
        }
    }
}

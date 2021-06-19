using System.Reflection;

namespace CryptoTranslation
{
    public record Dict
    {
        public string Index_KeyDetails { get; set; }
        public string Index_KeyDate { get; set; }
        public string Index_KeyLocation { get; set; }
        public string Index_KeyFound { get; set; }
        public string Index_KeyNotFound { get; set; }
        public string Index_KeyNoExist { get; set; }
        public string Index_NoValidation { get; set; }
        public string Index_ValidationSuccess { get; set; }
        public string Index_ValidationFailed { get; set; }
        public string Index_LoadKeyButton { get; set; }
        public string Index_ValidateKeyButton { get; set; }
        public string Index_NewKeyButton { get; set; }
        public string Index_UpdateButton { get; set; }
        public string Index_UpdateAvailable { get; set; }
        public string Index_UpdateFailed { get; set; }
        public string Index_NotSet { get; set; }
        public string Index_NewKeyPasswordLabel { get; set; }
        public string Index_LoadKeyPasswordLabel { get; set; }
        public string Index_ValidateKeyPasswordLabel { get; set; }
        public string Index_EnterToAccept { get; set; }
        public string Index_Title { get; set; }
        public string GetPropertyValue(string input)
        {
            return GetType().GetProperty(input)?.GetValue(this, null)?.ToString();
        }
    }
}
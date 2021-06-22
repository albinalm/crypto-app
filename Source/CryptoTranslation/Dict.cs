namespace CryptoTranslation
{
    public record Dict
    {
        #region Index

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
        public string Index_DownloadingUpdate { get; set; }

        #endregion Index

        #region PasswordDialogue

        public string PasswordDialogue_IncorrectPassword { get; set; }
        public string PasswordDialogue_PasswordLabel { get; set; }
        public string PasswordDialogue_Title { get; set; }
        public string PasswordDialogue_Key { get; set; }
        public string PasswordDialogue_NoKeyDetected { get; set; }
        public string PasswordDialogue_ValidationSuccess { get; set; }
        public string PasswordDialogue_ValidationFailed { get; set; }

        #endregion PasswordDialogue

        #region Encryption

        public string Encryption_Encrypting { get; set; }
        public string Encryption_Destination { get; set; }
        public string Encryption_Speed { get; set; }
        public string Encryption_Title { get; set; }
        public string Encryption_EncryptingFile { get; set; }
        public string Encryption_EncryptingFileOf { get; set; }
        public string Encryption_Finalizing { get; set; }
        public string Encryption_FinishingUp { get; set; }
        public string Encryption_Source { get; set; }

        #endregion Encryption

        #region Decryption

        public string Decryption_Decrypting { get; set; }
        public string Decryption_Destination { get; set; }
        public string Decryption_Speed { get; set; }
        public string Decryption_Title { get; set; }
        public string Decryption_DecryptingFile { get; set; }
        public string Decryption_DecryptingFileOf { get; set; }
        public string Decryption_Finalizing { get; set; }
        public string Decryption_FinishingUp { get; set; }
        public string Decryption_Source { get; set; }

        #endregion Decryption

        #region General
        public string General_FileDialogFilter { get; set; }
        
        public string General_OpenFileDialogTitle { get; set; }
        public string General_SaveFileDialogTitle { get; set; }
        #endregion General

        #region InstallerLinux
        public string InstallerLinux_WelcomeTo { get; set; }
        public string InstallerLinux_Next { get; set; }
        public string InstallerLinux_Back { get; set; }
        public string InstallerLinux_WhereToInstall { get; set; }
        public string InstallerLinux_Install { get; set; }
        public string InstallerLinux_WhatFMQuestion { get; set; }
        public string InstallerLinux_Title { get; set; }
        public string InstallerLinux_TitleInstalling { get; set; }
        #endregion InstallerLinux
        public string GetPropertyValue(string input)
        {
            return GetType().GetProperty(input)?.GetValue(this, null)?.ToString();
        }
    }
}
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
        public string PasswordDialogue_ChangeKeyText { get; set; }

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
        public string InstallerLinux_TitleFinish { get; set; }
        public string InstallerLinux_Installing { get; set; }
        public string InstallerLinux_OpenPrivateer { get; set; }
        public string InstallerLinux_Finish { get; set; }
        public string InstallerLinux_UseDescription { get; set; }
        public string InstallerLinux_FinishedInstalling { get; set; }
        public string InstallerLinux_ContextMenuEncrypt { get; set; }
        public string InstallerLinux_ContextMenuDecrypt { get; set; }
        public string InstallerLinux_ContextMenuManageKeys { get; set; }

        #endregion InstallerLinux

        #region InstallerWindows

        public string InstallerWindows_WelcomeTo { get; set; }
        public string InstallerWindows_Next { get; set; }
        public string InstallerWindows_Back { get; set; }
        public string InstallerWindows_WhereToInstall { get; set; }
        public string InstallerWindows_Install { get; set; }
        public string InstallerWindows_Title { get; set; }
        public string InstallerWindows_TitleInstalling { get; set; }
        public string InstallerWindows_TitleFinish { get; set; }
        public string InstallerWindows_Installing { get; set; }
        public string InstallerWindows_OpenPrivateer { get; set; }
        public string InstallerWindows_Finish { get; set; }
        public string InstallerWindows_UseDescription { get; set; }
        public string InstallerWindows_FinishedInstalling { get; set; }
        public string InstallerWindows_ContextMenuEncrypt { get; set; }
        public string InstallerWindows_ContextMenuDecrypt { get; set; }

        #endregion InstallerWindows

        public string GetPropertyValue(string input)
        {
            return GetType().GetProperty(input)?.GetValue(this, null)?.ToString();
        }
    }
}
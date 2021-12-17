![varbyte_logo_banner_cropped_small](https://user-images.githubusercontent.com/70197523/146606521-332f60ff-9365-4a9d-b018-76641ce0b782.png)
<br>
# Introduction
This application will let you keep your files safe locally using 256-bit Rjindael encryption.<br>
The goal of the application is to make it easier as a user to encrypt your files without giving up security. Obviously there are already several solutions for this, but most of them are online-based while varbyte will be completely offline.<br><br>
## How it works
Usually encryption keys are a set of bytes secured behind a password. Now when working offline the major issue is most often where to store either the password or this set of bytes. Since you cannot store them behind a secure server this will often be a "dog-chasing-his-own-tail-situation". <br>
The idea behind varbyte is that we save these bytes into a ".varbyte"-file. Which the user then can store wherever they want. This file will later be used to encrypt your files. You can have an infinite set of varbyte-keys.<br><br>

When you choose to encrypt a file you will have to select which varbyte-key you wish to use. Then confirm the key by entering the password. After the encryption is done, the user themselves is responsible for the security levels by choosing where to store the key. <br>
For a more simple setup they could store the file in their documents folder, copy it like a regular file and have a backup on their cloud service. Or they might put the key on a USB drive. This way they could put the USB in, encrypt the file and take out the drive. <br>
The files are completely inaccessible without the key, and even if the key is obtained you would require a password to use it.<br><br>

When decrypting you will have to choose which varbyte-key to use for the decryption. If you pick the wrong key, the decryption will fail.<br>
The the procedure like several locks and keys. You probably cannot use your garage door key to unlock your house.<br><br><br>

**Disclaimer:<br>
Varbyte is completely offline based and can therefore not restore any files lost because of lost passwords or corrupt encryption key.
I strongly recommend to keep a working copy on a cloud service.**

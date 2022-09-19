# Backuper
An app to backup folders and files automatically. 

Allows creating an object named "Backuper", which, provided with info such as the source folder(or file!)'s path, an identifier name for the specific backuper, and a max number of backups, will backup the source with a single click of a button. On computer boot(provided it's not turned off in the settings menu), all the backupers will be backed up after a certain delay(so as to not interfere with regular booting operations).

Currently, it works only on Windows.

Specifics:

 - Custom main backup folder: it's possible to change the folder where all backups are saved in. You can do so in the settings menu.

 - Bin: when a backuper is deleted, all existing backups are moved to a bin folder where they will remain until you erase them manually.

 - Versioning: each backuper will allow a custom amount of maximum backups; older, extra backups get deleted on new backups. It's suggested to keep it at least at three versions.
 
 - Automatic Backups: on windows' boot, the application will be started, and all backupers will be backed up after a small delay. It's possible to deactivate this behavior in the settings menu.
 
 - Dark Theme: the GUI is entirely made in dark theme to ease the eyes.
 
 - Folder/File search: a custom-made window will allow to search for the path of needed files and folders without using the explorer and copy pasting said path.
 
 - Log File: standard behavior and errors will get written in the log file to access in case of issues.

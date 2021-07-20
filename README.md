# Backuper
An app to backup folders more easily. 

Allows creating an object named "Backuper" that, provided with info such as the source folder's path, name of the specific backuper(which is used to name the backup folder), and a max number of versions, will backup a specific folder and version it through the single click of a button. It's furthermore possible to select whether the backupers should execute a backup on the computer's startup(more specifically, when a specific Windows user starts).

Each backuper's source path can either lead to a folder or a file; the back-end code will take care of identifying which it is.

Currently, it works only on Windows.

Specifics:

 - Custom main backup folder: it's possible to change the folder where all backups are saved in. To do so, click on "Change Backup Main Path" button in the main window.

 - Bin: when a backuper is deleted, it's possible to move its completed backups to a bin folder located within the main backup folder. Those backups will be preserved until you decide to erase them manually.

 - Versioning: based on the given "max number of versions", each backuper will check how many versions are currently done, and will delete the oldests if there are too many versions.
 
 - Automatic Backups: if this functionality is turned on(can be activated through a button on the main window), the backupers with autoupdate set to true will execute a backup on a specific windows user's startup.
 
 - Dark Theme: the GUI is entirely made in dark theme to ease the eyes.
 
 - Folder/File search: a custom-made window will allow to search for the path of needed files and folders without using the explorer and copy pasting said path.
 
 - Error log: unsolvable and unexpected errors will get written to the error log; you can find it in the exe's directory under the name of "ErrorLog.txt".

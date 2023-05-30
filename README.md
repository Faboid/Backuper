# Backuper

Backuper is a C# WPF desktop application built to provide an easy way to automatically backup folders and files. 

Currently works only on Windows.

## Features

- **Backup Creation:** Create backup objects named "Backuper" with specific source folder or file paths, an identifier name, and a maximum number of backup versions. Simply click a button to initiate the backup process.
- **Custom Main Backup Folder:** Easily change the folder where all backups are saved using the settings menu.
- **Bin:** Deleted backup objects are moved to a dedicated bin folder, allowing you to manually erase them at your convenience.
- **Versioning:** Each backuper can specify a maximum number of backups. Older backups beyond the specified limit are automatically deleted during new backups, ensuring efficient use of storage. It is recommended to keep a minimum of three versions.
- **Automatic Backups:** On Windows boot, the application starts automatically (unless disabled in the settings menu), and all backupers are backed up after a small delay to avoid interfering with regular booting operations.
- **Dark Theme:** The application's graphical user interface (GUI) is designed with a sleek dark theme for improved readability and visual appeal.
- **Folder/File Search:** A custom-made window allows you to search for the paths of required files and folders without relying on the Windows Explorer and manual copy-pasting.
- **Log File:** Standard behavior and errors are recorded in a log file, providing easy access for troubleshooting in case of issues.

## Installation and Usage

1. Clone or download the repository.
2. Build the solution in Visual Studio.
3. Launch the Backuper application.
4. Configure the main backup folder and any other desired settings through the settings menu.
5. Create backup objects ("Backuper") by providing source folder or file paths, an identifier name, and the maximum number of backup versions.
6. Click the backup button or the Backup All button to initiate the backup process.

AutoDeleteOldFiles
==================

C# Console app to delete old files. 

Uses log4net. 

Use config file to assign folder names and the number of days to keep the files in each folder.

Config file also includes a default value. If you have a directory with too many folders to easily list in config, you can choose to not list them and the default number of days will be used.

Empty folders get deleted.

Pattern layout (how the log file is formatted)
See: http://logging.apache.org/log4net/release/sdk/log4net.Layout.PatternLayout.html


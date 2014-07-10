using System;
using log4net;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

//test master branch
namespace AutoFileRemover
{
    class Program
    {
        //private static volatile Config _instance;
        //private static readonly object SyncRoot = new Object();
        //private static readonly ILog Log = LogManager.GetLogger("AutoFileRemoveLog");
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public static Config Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            lock (SyncRoot)
        //            {
        //                if (_instance == null)
        //                    _instance = new Config();
        //            }
        //        }
        //        return _instance;
        //    }
        //} //end Instance



//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
             
            
            LoadConfig();
            processDirectories();

            //Record end time in log
            System.DateTime endDate = (DateTime.Now);
            log.Info("End: " + endDate);
        }





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
        //Load the config file
        static void LoadConfig()
        {
            //Configure the log4net object.  File will go to C:/Logs/auto_file_remove_log
            try
            {
                
                System.DateTime startDate = (DateTime.Now);
                log.Info("Start: " + startDate);
                log.Info("AutoFileRemove => OnStart(): AutoFileRemoveStarting");
                string WorkingDirectory = System.Configuration.ConfigurationManager.AppSettings["path"];
                log.Info("Config => WorkingDirectory = " + WorkingDirectory);
            }

            catch (Exception ex)
            {
                log.Error("Config => Error loading configuration - " + ex);
            }





            //Make sure app settings have been entered into app.config
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    log.Error("There are no folders or dates configured.");
                }
                else
                {
                    log.Info("There are " + appSettings.Count + " appSettings.");
                }

            }
            catch (ConfigurationErrorsException)
            {
                //write error to log file
                log.Error("Error reading app settings");
                //Environment.Exit(0);   //Disable during debugging
            }

        } //end ReadAllSettings()





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        static void processDirectories()
        {

            //get head directory from app.config (e.g., C:/...)
            string mainPath = System.Configuration.ConfigurationManager.AppSettings["path"];

            //get files that are in the main directory (aka not in a sub-folder)
            processFiles(mainPath);

            //get sub-folders from mainPath
            string[] subdirectoryEntries = Directory.GetDirectories(mainPath);

            //send each subdirectory path to processFiles()
            foreach (string subdirectory in subdirectoryEntries)
            {
                processFiles(subdirectory);
            }

        } //end processDirectories()


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static void processFiles(string path)
        {
            //declare the current date/time
            System.DateTime date = (DateTime.Now);

            //Declare int for how old the file needs to be in order to be deleted
            int daysOldNeeded;

            //Declare string for name of file's parent folder
            string folderName;

            //Get files from folder
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);

                //get the file's name
                string fileName = fi.Name;

                //get its head folder name
                string filePath = fi.DirectoryName;
                /*GetDirectoryName returns the full path. 
                GetFileName returns the last path component (last folder) */
                folderName = Path.GetFileName(Path.GetDirectoryName(fi.FullName));

                //Get how old it has to be to delete (value), based on the folder type (key) from app.config
                try
                {
                    daysOldNeeded = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings[folderName]);

                    //Checks to make sure the required days is in the app.config file and is > 0
                    if (daysOldNeeded > 0)
                    {
                        //If it's older than the necessary days, delete and log
                        if (fi.CreationTime < date.AddDays(-daysOldNeeded))
                        {
                            fi.Delete();
                            log.Info("The file " + folderName + "\\" + fileName + ", created on " + fi.CreationTime + " has been deleted.");
                        }

                        //Else, keep the file and log. Optional. 
                        else
                        {
                            log.Info("The file " + folderName + "\\" + fileName + ", created on " + fi.CreationTime + " has not been deleted.");
                        }
                    } //end if daysOldNeeded > 0

                    else
                    {
                        log.Warn("The folder " + folderName + " is not established in app.config.");
                    }

                }
                catch
                {
                    //if there is nothing in the app.config file with that folder name 
                    log.Warn("The folder " + folderName + " is not set up in app.config.");
                }


            } //end foreach file in files		

        } //end processFiles()



    } //end public sealed class Config
} //end namespace
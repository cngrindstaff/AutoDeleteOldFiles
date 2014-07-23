using System;
using log4net;
using System.Configuration;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AutoFileRemover
{
    public sealed class Config
    {
        private static volatile Config _instance;
        private static readonly object SyncRoot = new Object();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Config));

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Config();
                    }
                }
                return _instance;
            }
        } //end Instance
 
 
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
                    //log.Info("There are " + appSettings.Count + " appSettings.");
                }

            }
            catch (ConfigurationErrorsException)
            {
                //write error to log file
                log.Error("Error reading app settings");
                Environment.Exit(0); 
            }

        } //end ReadAllSettings()



//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static void processDirectories()
        {
            string mainDirectory = System.Configuration.ConfigurationManager.AppSettings["path"];

            //get sub-folders from mainDirectory
            string[] subdirectoryEntries = Directory.GetDirectories(mainDirectory);

            //send each subdirectory path to getDaysOld()
            foreach (string subdirectory in subdirectoryEntries)
            {
                getDaysOld(subdirectory);
            }

        } //end processDirectories()


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static void getDaysOld(string subdirectory)
        {
            //Declare string for name of file's parent folder
            string parentFolderName = Path.GetFileName(subdirectory);

            //Get int for how old the file needs to be in order to be deleted
            int daysOldNeeded = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[parentFolderName]);

            //If the age is defined in app.config
            if (daysOldNeeded != 0)
            {
                processFiles(daysOldNeeded, parentFolderName, subdirectory);
            }
            
            //If the folder is not established in app.config, use the default set up in app.config
            else 
            {
                int defaultDaysOldNeeded = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["default"]);
                processFiles(defaultDaysOldNeeded, parentFolderName, subdirectory);
            }


        } //end getDaysOld()


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static void processFiles(int daysOldNeeded, string parentFolderName, string subdirectory)
        {

            //declare the current date/time
            System.DateTime date = (DateTime.Now);
            
            //Get files from folder
            string[] files = Directory.GetFiles(subdirectory);

            //If there are no files in the folder, delete the folder
            if (files.Length == 0)
            {
                Directory.Delete(subdirectory);
                log.Info("The folder " + parentFolderName + " was empty and has been deleted.");
            }

            else
            {
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);

                    //get the file's name
                    string fileName = fi.Name;

                    //get its head folder name
                    string filePath = fi.DirectoryName;

                    //Checks to make sure the required days is in the app.config file and is > 0
                    if (daysOldNeeded > 0)
                    {
                        //If it's older than the necessary days, delete and log
                        if (fi.CreationTime < date.AddDays(-daysOldNeeded))
                        {
                            fi.Delete();
                            log.Info("The file " + parentFolderName + "\\" + fileName + ", created on " + fi.CreationTime + ", has been deleted.");
                        }

                        //Else, keep the file and log. Optional. 
                        else
                        {
                            //log.Info("The file " + parentFolderName + "\\" + fileName + ", created on " + fi.CreationTime + ", has not been deleted.");
                        }
                    } //end if daysOldNeeded > 0



                } //end foreach file in files		
            } //end else
            
            


        }//end processFiles


    } //end public sealed class Config
} //end namespace
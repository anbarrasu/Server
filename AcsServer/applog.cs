using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Security.Principal;

namespace AcsServer
{
    static class applog
    {
        private static StreamWriter GeneralLogFile;
        private static StreamWriter ExceptionLogFile;
        private static StreamWriter SerialLogFile;
        private static StreamWriter templogfile;

        private static string logfilepath;  //should be taken from setting

        private static string date;
        static public void initCheck()
        {
            logfilepath = string.Format(@"{0}\Log", Environment.CurrentDirectory);
            CreateWithFullAccess(logfilepath);
            return;
        }
        public static void CreateWithFullAccess(string targetDirectory)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                DirectoryInfo info = new DirectoryInfo(targetDirectory);
                SecurityIdentifier allUsersSid =
                new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid,
                null);
                DirectorySecurity security = info.GetAccessControl();
                security.AddAccessRule(
                new FileSystemAccessRule(allUsersSid,
                FileSystemRights.FullControl,
                AccessControlType.Allow));
                info.SetAccessControl(security);
            }
            catch (Exception ex)
            {
                thisexception(ex.ToString());
            }
        }
        static applog()
        {
            date = DateTime.Now.Date.ToString("dd_MM_yyyy");
            logfilepath = string.Format(@"{0}\Log",Environment.CurrentDirectory);
            //MessageBox.Show(logfilepath);
            DirectoryInfo di = new DirectoryInfo(logfilepath);

            if (!di.Exists)
                di.Create();
            //Check if log file exists 

            //Create if not 



            try
            {
                GeneralLogFile = new StreamWriter(string.Format(@"{0}\General_log_{1}.log", logfilepath, date), true);
                ExceptionLogFile = new StreamWriter(string.Format(@"{0}\Excep_log_{1}.log", logfilepath, date), true);
                SerialLogFile = new StreamWriter(string.Format(@"{0}\Serial_log_{1}.log", logfilepath, date), true);
                templogfile =  new StreamWriter(string.Format(@"{0}\Temp_log_{1}.log", logfilepath, date), true);
                GeneralLogFile.AutoFlush = true;
                ExceptionLogFile.AutoFlush = true;
                SerialLogFile.AutoFlush = true;
                templogfile.AutoFlush = true;
                //throw new System.IO.DirectoryNotFoundException();

            }
            catch (Exception e)
            {
                thisexception(e.Message);
                //MessageBox.Show("Hai");

            }

        }

        static public void loggen(string message)
        {
            try
            {
                GeneralLogFile.WriteLine(string.Format("{0} :{1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"), message));

            }
            catch (Exception e)
            {
                thisexception(e.Message);
            }

            return;
        }

        static public void logexcep(string module, string message)
        {
            try
            {

                ExceptionLogFile.WriteLine(string.Format("{0} {1}:{2}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"), module, message));
            }
            catch (Exception e)
            {
                thisexception(e.Message);
            }

            return;
        }

        static public void logserial(string message)
        {
            try
            {
                SerialLogFile.WriteLine(string.Format("{0} :{1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"), message));

            }
            catch (Exception e)
            {
                thisexception(e.Message);
            }

            return;
        }

        static public void logtemp(string message)
        {
            try
            {
                templogfile.WriteLine(string.Format("{0} :{1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"), message));

            }
            catch (Exception e)
            {
                thisexception(e.Message);
            }

            return;
        }


        static public void refreshdatefile()
        {
            date = DateTime.Now.Date.ToString("dd_MM_yyyy_HH_mm");

            if (GeneralLogFile != null)
            {
                GeneralLogFile.Flush();
                GeneralLogFile.Close();
            }

            if (ExceptionLogFile != null)
            {
                ExceptionLogFile.Flush();
                ExceptionLogFile.Close();
            }

            if (SerialLogFile != null)
            {
                SerialLogFile.Flush();
                SerialLogFile.Close();
            }

            if (templogfile != null)
            {
                templogfile.Flush();
                templogfile.Close();
            }

            try
            {
                GeneralLogFile = new StreamWriter(string.Format(@"{0}\General_log_{1}.log", logfilepath, date), true);
                ExceptionLogFile = new StreamWriter(string.Format(@"{0}\Excep_log_{1}.log", logfilepath, date), true);
                SerialLogFile = new StreamWriter(string.Format(@"{0}\Serial_log_{1}.log", logfilepath, date), true);
                templogfile = new StreamWriter(string.Format(@"{0}\Temp_log_{1}.log", logfilepath, date), true);
                //throw 
            }
            catch (Exception e)
            {
                thisexception(e.Message);

            }

        }

        static void thisexception(string message)
        {
            string temppath;
            
            try
            {
                //get environment variable
                //check and create a directory
                //check and open a file 
                //write into file and flush
                temppath=Environment.GetEnvironmentVariable("TEMP");
                temppath+=@"\Access Control System";

                DirectoryInfo di = new DirectoryInfo(temppath);
                if (!di.Exists)
                di.Create();
                temppath = string.Format(@"{0}\ACS_logfile_err_{1}.log",temppath, date);
                StreamWriter LogFileexception = new StreamWriter(temppath, true);
                MessageBox.Show(temppath);
                LogFileexception.WriteLine(string.Format("{0} Exception:{1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"), message));
                LogFileexception.Flush();
                LogFileexception.Close();
            }
            catch 
            {
               
            }
        }

    }
}
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Data;
using AcsServer;


namespace Access_Control_System
{
    class dataBaseM
    {
        private string database_name;
        private string database_path;
        private string connString;
        SqlCeConnection conn;

        public dataBaseM()
        {
            database_name = "ACSystem.sdf";
#if DEBUG_HOME
            database_path = @"C:\Users\Arasu\My Projects\acs\Access Control System\Access Control System\";
#else
#if DEBUG
            database_path = "F:\\Project\\ACS\\Access Control System\\Access Control System\\";
            
#else
            database_path = "C:\\Access Control System\\Data\\";
#endif
#endif
            string fileSource = database_path + database_name;
            connString = "Data Source='" + fileSource + "'; LCID=1033;   Password=acsadmin123; Encrypt = TRUE;";
            conn = null;

        }

        public bool isExists()
        {

            string fileSource = database_path + database_name;

            if (File.Exists(fileSource))

                return true;
            else
                return false;
        }

        public bool deletedatabase()
        {
            string fileSource = database_path + database_name;
            if (this.isExists())
            {
                File.Delete(fileSource);
                MessageBox.Show("DataBase Deleted");
                return true;
            }
            return false;
        }

        public int createdatabase()
        {
            if (this.isExists())
            {
                MessageBox.Show("DataBase exists already");
                return 0;
            }


            //File.Delete("ACSystem.sdf");
            SqlCeEngine engine = new SqlCeEngine(connString);
            engine.CreateDatabase();
            MessageBox.Show("DataBase has been created sucessfully");
            engine.Dispose();

            return 1;
        }

        public bool createHolidayTable()
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "CREATE TABLE HolidayList (Date datetime NOT NULL,Description nvarchar(100) NOT NULL)";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                applog.logexcep("createHolidayTable", e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool addcolumnnTable()
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "ALTER TABLE Attendance ADD DoorNo tinyint default 0 NOT NULL";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER TABLE Attendance ADD inORout tinyint default 0 NOT NULL";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                applog.logexcep("Modify  Table to include two columns Doorno InOrOut", e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool createAttendanceTable()
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlCeCommand cmd = conn.CreateCommand();

                cmd.CommandText = "CREATE TABLE Attendance (RecNo bigint IDENTITY (1,1) PRIMARY KEY, EmployeeId nvarchar(8) NOT NULL, EmployeeName nvarchar(100) NOT NULL,AccessCardNo int ,SwipeTime datetime NOT NULL)";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                applog.logexcep("createAttendanceTable", e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool createAccessControlListTable()
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();

               //IDENTITY (100,1) PRIMARY KEY
                cmd.CommandText = "CREATE TABLE AccessControlList (EmployeeId nvarchar(8) NOT NULL UNIQUE , EmployeeName nvarchar(100) NOT NULL,AccessCardNo int UNIQUE,TemporaryCardNo int,AllowAccess bit NOT NULL,IsEntryValid bit NOT NULL)";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();


                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                applog.logexcep("createAccessControlListTable", e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }


        public int createTables()
        {
            int ret = 0;


                if (createAccessControlListTable())
                {
                    MessageBox.Show("AccessControlList Table created");
                    ret = 1;
                }
                if (createAttendanceTable())
                {
                    MessageBox.Show("AccessControlList Table created");
                    ret = 1;
                }
                
                    
                if (createHolidayTable())
                {
                    MessageBox.Show("Holiday Table created");
                    ret = 1;
                }

                if (ret == 0)
                {
                    //MessageBox.Show("All Tables Exists");
                    return 0;
                }
                else

                return 1;
          

        }

        public bool checkiftablesExists()
        {
            bool acstable, atttable,holtable;
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if(conn.State!=ConnectionState.Open)
                    conn.Open();
                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + "AccessControlList" + "'";

                acstable=Convert.ToBoolean(cmd.ExecuteScalar());

                cmd.CommandText = @"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + "Attendance" + "'";

                atttable = Convert.ToBoolean(cmd.ExecuteScalar());

                cmd.CommandText = @"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + "HolidayList" + "'";
                holtable = Convert.ToBoolean(cmd.ExecuteScalar());
                return (atttable & atttable & holtable);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                applog.logexcep("checkiftablesExists", e.Message);
                return false;
            }
            finally
            {
               conn.Close();

            }

            
        }


        public bool addrecord_acl(string empid, string empname, int cardno, bool AllowAccess, bool IsEntryValid)
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText =string.Format( "INSERT INTO AccessControlList (EmployeeId, EmployeeName, AccessCardNo, AllowAccess, IsEntryValid) VALUES ('{0}','{1}',{2},{3},{4})" , empid, empname, cardno.ToString(),Convert.ToInt32( AllowAccess),Convert.ToInt32( IsEntryValid));
                //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                //MessageBox.Show("Record Added");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                applog.logexcep("addrecord_acl", e.Message);
                return false;
            }
            finally
            {
                conn.Close();

            }


        }



        public bool updaterecord_acl(string empid, string empname, int cardno, bool AllowAccess, bool IsEntryValid)
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("UPDATE  AccessControlList SET EmployeeName='{0}',AccessCardNo={1},AllowAccess={2},IsEntryValid={3} where EmployeeId='{4}'", empname, cardno.ToString(), Convert.ToInt32(AllowAccess), Convert.ToInt32(IsEntryValid), empid);
                 //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //if (cmd.ExecuteNonQuery() != 0)
                //    MessageBox.Show("Record Updated");
                //else
                //    MessageBox.Show("Record not Available to update");
               

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                applog.logexcep("updaterecord_acl", e.Message);
                return false;
            }
            finally
            {
                conn.Close();

            }


        }

        public bool deleterecord_acl(string empid)
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText =string.Format( "DELETE FROM AccessControlList where EmployeeId='{0}'", empid);
                //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //if (cmd.ExecuteNonQuery() != 0) ;
                //MessageBox.Show("Record Deledted");
                //else
                //    MessageBox.Show("Record not Available");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                applog.logexcep("deleterecord_acl",e.Message);
                return false;
            }
            finally
            {
                conn.Close();

            }
        }

        public int getrecord_acl(string empid, ref string empname, ref int cardno,ref bool AllowAccess,ref bool IsEntryValid)
        {
            int tempcardno;
            SqlCeDataReader rdr = null;
   
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();//where employeeid=" + empid.ToString()
                cmd.CommandText =string.Format( "SELECT * FROM AccessControlList where EmployeeId='{0}'",empid);
                //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                rdr = cmd.ExecuteReader();
                //if (rdr.HasRows)
                //{

                bool hasRow = rdr.Read();
                if (hasRow)
                {
                    empid = rdr.GetString(0);
                    empname = rdr.GetString(1);
                    if (!rdr.IsDBNull(2))
                        cardno = rdr.GetInt32(2);
                    else
                        cardno = 0;

                    if (!rdr.IsDBNull(3))
                        tempcardno = rdr.GetInt32(3);
                    else
                        tempcardno = 0;

                    AllowAccess = rdr.GetBoolean(4);
                    IsEntryValid = rdr.GetBoolean(5);
                    return acsConstant.recordAvailable;
                }

                    

                //}
                return acsConstant.recordUnavailable;
            }
            catch (Exception e)
            {
                applog.logexcep("getrecord_acl",e.Message);
                return acsConstant.exceptionoccured;
            }
            finally
            {
                rdr.Close();
                conn.Close();

            }
        }

        public int getrecord_acl_forcardid(ref string empid, ref string empname, int cardno, ref bool AllowAccess, ref bool IsEntryValid)
        {  
            SqlCeDataReader rdr = null;
            int tempcardno;
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();//where employeeid=" + empid.ToString()
                cmd.CommandText = "SELECT * FROM AccessControlList where AccessCardNo=" + cardno.ToString();
                //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                rdr = cmd.ExecuteReader();
                //if (rdr.HasRows)
                //{

                bool hasRow = rdr.Read();
                if (hasRow)
                {
                    empid = rdr.GetString(0);
                    empname = rdr.GetString(1);
                    if (!rdr.IsDBNull(2))
                        cardno = rdr.GetInt32(2);
                    else
                        cardno = 0;

                    if (!rdr.IsDBNull(3))
                        tempcardno = rdr.GetInt32(3);
                    else
                        cardno = 0;

                    AllowAccess = rdr.GetBoolean(4);
                    IsEntryValid = rdr.GetBoolean(5);
                    return acsConstant.recordAvailable;
                }



                //}
                return acsConstant.recordUnavailable;
            }
            catch (Exception e)
            {
                applog.logexcep("getrecord_acl_forcardid",e.Message);
                return acsConstant.exceptionoccured;
            }
            finally
            {
                rdr.Close();
                conn.Close();

            }
        }

        //***************************************************************//
        //public bool addattendance(int cardid)
        public bool addattendance(int cardid,int doorno,int inORout)
        {
            string empid="";
            string empname="";
             bool AllowAccess=false;
             bool IsEntryValid=false;

             if (getrecord_acl_forcardid(ref empid, ref empname, cardid, ref AllowAccess,ref IsEntryValid) == acsConstant.recordAvailable)
            {
                 if(AllowAccess)
                    addrecord_att(empid, empname, DateTime.Now,cardid,(byte)doorno,(byte)inORout);
                return true;
            }
            else
                return false;


        }

        public bool addrecord_att(string empid, string empname, DateTime swipetime,Int32 CardId,byte Doorno,byte inORout)
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();

                //SNo bigint IDENTITY (1,1) PRIMARY KEY, employeeid int NOT NULL, employeename nvarchar(100) NOT NULL,swipetime datatime NOT NULL
                cmd.CommandText = "INSERT INTO Attendance (EmployeeId, EmployeeName,AccessCardNo,SwipeTime,DoorNo,inORout) VALUES ('" + empid + "','" + empname + "','" + CardId + "','" + swipetime.ToString("s") +  "','"+ Doorno + "','"+ inORout + "')";//ToString("yyyy/MM/dd HH:mm:ss")
                //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                //MessageBox.Show("Record Added");
                return true;
            }
            catch (Exception e)
            {
                applog.logexcep("addrecord_att", e.Message);
                return false;
            }
            finally
            {
                conn.Close();

            }


        }


        public bool filltable_att(string querystring,ref DataTable table)
        {
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
               
                // 2
                // Create new DataAdapter
                using (SqlCeDataAdapter a = new SqlCeDataAdapter(
                    querystring, conn))
                {
                    // 3
                    // Use DataAdapter to fill DataTable
                    //DataTable t = new DataTable();
                    a.Fill(table);
                    // 4
                    
                }
                return true;
            }
            catch (Exception e)
            {
                applog.logexcep("filltable_att", e.Message);
                return false;
            }
            finally
            {
                conn.Close();

            }

        }

        public int filllist(string querystring,ref List<attListItem> rpattlist)
        {
            SqlCeDataReader rdr = null;
            int count=0;
            string empid;
            string empname;
            DateTime datemin,datemax;
            try
            {
                if (conn == null)
                    conn = new SqlCeConnection(connString);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();//where employeeid=" + empid.ToString()
                cmd.CommandText = querystring;
                //MessageBox.Show(cmd.CommandText);
                cmd.CommandType = CommandType.Text;
                rdr = cmd.ExecuteReader();
                //if (rdr.HasRows)
                //{

                
                while (rdr.Read())
                {
                    count++;
                    empid = rdr.GetString(0);
                    empname = rdr.GetString(1);
                    if (!rdr.IsDBNull(2))
                        datemin = rdr.GetDateTime(2);
                    else
                        datemin=DateTime.MinValue;
                    if (!rdr.IsDBNull(3))
                    datemax = rdr.GetDateTime(3);
                    else
                        datemax = DateTime.MinValue;
                    rpattlist.Add(new attListItem(empid, empname, datemin, datemax));
                }

                    if(Convert.ToBoolean(count))
                        return acsConstant.recordAvailable;
                    else
                        return acsConstant.recordUnavailable;
            }
            catch (Exception e)
            {
                applog.logexcep("filllist", e.Message);
                return acsConstant.exceptionoccured;
            }
            finally
            {
                rdr.Close();
                conn.Close();

            }

        }
        //public bool updaterecord_att(int empid, string empname, int cardno)
        //{
        //    try
        //    {
        //        if (conn == null)
        //            conn = new SqlCeConnection(connString);
        //        if (conn.State != ConnectionState.Open)
        //            conn.Open();

        //        SqlCeCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "UPDATE  AccessControlList SET employeename='" + empname + "',accesscardno=" + cardno.ToString() + " where employeeid=" + empid.ToString();
        //        //MessageBox.Show(cmd.CommandText);
        //        cmd.CommandType = CommandType.Text;
        //        if (cmd.ExecuteNonQuery() != 0)
        //            MessageBox.Show("Record Updated");
        //        else
        //            MessageBox.Show("Record not Available to update");


        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        applog.logexcep(e.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        conn.Close();

        //    }


        //}

        //public bool deleterecord_att(int empid)
        //{
        //    try
        //    {
        //        if (conn == null)
        //            conn = new SqlCeConnection(connString);
        //        if (conn.State != ConnectionState.Open)
        //            conn.Open();

        //        SqlCeCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "DELETE FROM AccessControlList where employeeid=" + empid.ToString();
        //        //MessageBox.Show(cmd.CommandText);
        //        cmd.CommandType = CommandType.Text;
        //        if (cmd.ExecuteNonQuery() != 0)
        //            MessageBox.Show("Record Deledted");
        //        else
        //            MessageBox.Show("Record not Available");
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        applog.logexcep(e.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        conn.Close();

        //    }
        //}

        //public bool getrecord_att(int empid, ref string empname, ref int cardno)
        //{
        //    SqlCeDataReader rdr = null;

        //    try
        //    {
        //        if (conn == null)
        //            conn = new SqlCeConnection(connString);
        //        if (conn.State != ConnectionState.Open)
        //            conn.Open();

        //        SqlCeCommand cmd = conn.CreateCommand();//where employeeid=" + empid.ToString()
        //        cmd.CommandText = "SELECT * FROM AccessControlList where employeeid=" + empid.ToString();
        //        //MessageBox.Show(cmd.CommandText);
        //        cmd.CommandType = CommandType.Text;
        //        rdr = cmd.ExecuteReader();
        //        //if (rdr.HasRows)
        //        //{

        //        bool hasRow = rdr.Read();
        //        if (hasRow)
        //        {
        //            empid = rdr.GetInt32(0);
        //            empname = rdr.GetString(1);
        //            cardno = rdr.GetInt32(2);
        //            return true;
        //        }



        //        //}
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        applog.logexcep(e.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        rdr.Close();
        //        conn.Close();

        //    }
        //}

    }



}

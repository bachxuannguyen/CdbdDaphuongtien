using System;
using System.Data;
using System.Data.SqlClient;

namespace cdbd_daphuongtien.App_Code
{
    public class DtLib
    {
        CsLib csLib = new CsLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public int getRowCount(string dataTable, bool isAllowDisabled)
        {
            int outputInt = -1;

            string strIsAllowDisabled = "";
            if (!isAllowDisabled)
                strIsAllowDisabled = " WHERE " + dataTable + ".state = 1";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.CommandText = "SELECT COUNT(*) FROM " + dataTable + strIsAllowDisabled;
                outputInt = (Int32)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, dataTable);
                return -1;
            }

            return outputInt;
        }

        public int getNewIntId(string dataTable, string primaryKey)
        {
            int outputInt = -1;

            int rowCount = getRowCount(dataTable, true);
            if (rowCount > 0)
            {
                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(false);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();
                    sqlcomm.CommandText = "SELECT TOP 1 " + primaryKey + " FROM " + dataTable + " ORDER BY " + primaryKey + " DESC";
                    outputInt = (Int32)sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, dataTable);
                    return -1;
                }
            }

            if (outputInt < 0)
                outputInt = 1;
            else
                outputInt = outputInt + 1;

            return outputInt;
        }

        public string getData_StrToStr(string outputValue, string dataTable, string inputCondition, string Id)
        {
            string outputString = "";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@Id", Id);
                sqlcomm.CommandText = "SELECT " + outputValue + " FROM " + dataTable + " WHERE " + inputCondition + " = @Id";
                outputString = sqlcomm.ExecuteScalar() as string ?? "";
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, Id);
                return "";
            }
            try
            {
                sqlconn.Close();
            }
            catch { };

            return outputString;
        }

        public string getData_IntToStr(string outputValue, string dataTable, string inputCondition, int Id)
        {
            string outputString = "";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@Id", Id);
                sqlcomm.CommandText = "SELECT " + outputValue + " FROM " + dataTable + " WHERE " + inputCondition + " = @Id";
                outputString = sqlcomm.ExecuteScalar() as string ?? "";
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, Id.ToString());
                return "";
            }

            return outputString;
        }

        public int getData_StrToInt(string outputValue, string dataTable, string inputCondition, string Id)
        {
            int outputInt = -1;

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@Id", Id);
                sqlcomm.CommandText = "SELECT " + outputValue + " FROM " + dataTable + " WHERE " + inputCondition + " = @Id";
                outputInt = (int)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, Id.ToString());
                return -1;
            }

            return outputInt;
        }

        public int getData_IntToInt(string outputValue, string dataTable, string inputCondition, int Id)
        {
            int outputInt = -1;

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@Id", Id);
                sqlcomm.CommandText = "SELECT " + outputValue + " FROM " + dataTable + " WHERE " + inputCondition + " = @Id";
                outputInt = (int)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, Id.ToString());
                return -1;
            }

            return outputInt;
        }

        public DateTime getData_StrToDt(string outputValue, string dataTable, string inputCondition, string Id)
        {
            DateTime outputDateTime = DateTime.MinValue;

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@Id", Id);
                sqlcomm.CommandText = "SELECT " + outputValue + " FROM " + dataTable + " WHERE " + inputCondition + " = @Id";
                outputDateTime = (DateTime)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, Id.ToString());
                return DateTime.MinValue;
            }

            return outputDateTime;
        }

        public DateTime getData_IntToDt(string inputValue, string dataTable, string inputCondition, int Id)
        {
            DateTime outputDateTime = DateTime.MinValue; ;

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@Id", Id);
                sqlcomm.CommandText = "SELECT " + inputValue + " FROM " + dataTable + " WHERE " + inputCondition + " = @Id";
                outputDateTime = (DateTime)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, Id.ToString());
                return DateTime.MinValue;
            }

            return outputDateTime;
        }

        public int checkIntRecordExist(string primaryKey, string dataTable, int inputInt)
        {
            int output = -1;

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@input", inputInt);
                sqlcomm.CommandText = "SELECT CASE WHEN EXISTS (SELECT 1 FROM " + dataTable + " WHERE " + primaryKey + "=@input) THEN 1 ELSE 0 END";
                output = (int)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputInt.ToString());
                return -1;
            }

            return output;
        }

        public int checkStrRecordExist(string primaryKey, string dataTable, string inputStr)
        {
            int output = -1;

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                sqlcomm.Parameters.AddWithValue("@input", inputStr);
                sqlcomm.CommandText = "SELECT CASE WHEN EXISTS (SELECT 1 FROM " + dataTable + " WHERE " + primaryKey + "=@input) THEN 1 ELSE 0 END";
                output = (int)sqlcomm.ExecuteScalar();
                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputStr);
                return -1;
            }

            return output;
        }
    }
}
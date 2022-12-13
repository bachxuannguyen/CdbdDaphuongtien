using System;
using System.Data;
using System.Data.SqlClient;

namespace cdbd_daphuongtien.App_Code
{
    public class Account
    {
        CsLib csLib = new CsLib();
        DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public string id;
        public string password;
        public string name;
        public int state;

        public Account accountGet(string inputId)
        {
            Account currentAccount = new Account();

            string s = csLib.isValidString("Mã tài khoản", inputId, false, false, true);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, inputId);
                return new Account();
            }
            else
            {
                int x = dtLib.checkStrRecordExist("id", "TableAccount", inputId);
                if (x < 0)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                    return new Account();
                }
                else if (x != 1)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tài khoản này trong cơ sở dữ liệu.", inputId);
                    return new Account();
                }

                try
                {
                    currentAccount.id = inputId;
                    currentAccount.password = dtLib.getData_StrToStr("password", "TableAccount", "id", inputId);
                    currentAccount.name = dtLib.getData_StrToStr("username", "TableAccount", "id", inputId);
                    currentAccount.state = dtLib.getData_StrToInt("state", "TableAccount", "id", inputId);
                    string s1 = isValidAccount(currentAccount, false);
                    if (!string.IsNullOrEmpty(s1))
                    {
                        csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s1, inputId);
                        return new Account();
                    }
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputId);
                    return new Account();
                }
            }
            return currentAccount;
        }

        public bool accountUpdate(Account currentAccount)
        {
            string s = isValidAccount(currentAccount, false);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentAccount.id);
                return false;
            }
            else
            {
                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(true);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();

                    sqlcomm.Parameters.AddWithValue("@Id", currentAccount.id);
                    sqlcomm.Parameters.AddWithValue("@Password", currentAccount.password);
                    sqlcomm.Parameters.AddWithValue("@Name", currentAccount.name);
                    sqlcomm.Parameters.AddWithValue("@State", currentAccount.state);
                    sqlcomm.CommandText = "UPDATE TableAccount SET "
                                        + "password=@Password, username=@Name, state=@State "
                                        + "WHERE id=@Id";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentAccount.id);
                    return false;
                }
            }
        }

        public bool accountInsert(Account currentAccount)
        {
            string s = isValidAccount(currentAccount, true);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentAccount.id);
                return false;
            }
            else
            {
                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(true);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();

                    sqlcomm.Parameters.AddWithValue("@Id", currentAccount.id);
                    sqlcomm.Parameters.AddWithValue("@Password", currentAccount.password);
                    sqlcomm.Parameters.AddWithValue("@Name", currentAccount.name);
                    sqlcomm.Parameters.AddWithValue("@State", currentAccount.state);
                    sqlcomm.CommandText = "INSERT INTO TableAccount (id, password, username, state) "
                                        + "VALUES "
                                        + "(@Id, @Password, @Name, @State) ";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentAccount.id);
                    return false;
                }
            }
        }

        public bool accountDelete(string inputId)
        {
            int x = dtLib.checkStrRecordExist("id", "TableAccount", inputId);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return false;
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tài khoản này trong cơ sở dữ liệu.", inputId);
                return false;
            }

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(true);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.Parameters.AddWithValue("@Id", inputId);
                sqlcomm.CommandText = "DELETE FROM TableAccount WHERE id=@Id;";
                sqlcomm.ExecuteScalar();

                sqlconn.Close();

                return true;
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputId);
                return false;
            }
        }

        public string isValidAccount(Account inputAccount, bool isAlredyUseCheck)
        {
            string outputMessage = "";

            if (isAlredyUseCheck)
            {
                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(false);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();
                    sqlcomm.Parameters.Add(new SqlParameter("@Id", SqlDbType.NVarChar));
                    sqlcomm.Parameters["@Id"].Value = inputAccount.id;
                    sqlcomm.CommandText = "SELECT id FROM TableAccount WHERE id=@Id";
                    string s = sqlcomm.ExecuteScalar() as string ?? "";
                    sqlconn.Close();
                    if (!string.IsNullOrEmpty(s) && s == inputAccount.id)
                        return "Account - Mã tài khoản này đã được sử dụng.<br />";
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputAccount.id);
                    return e.Message;
                }
            }

            outputMessage += csLib.isValidString("Account - Mã", inputAccount.id, false, false, true);
            outputMessage += csLib.isValidString("Account - Mật khẩu", inputAccount.password, true, false, true);
            outputMessage += csLib.isValidString("Account - Tên", inputAccount.name, true, false, true);
            if (inputAccount.state != 0 && inputAccount.state != 1)
                outputMessage += "Account - Trạng thái: Giá trị không hợp lệ.";

            return outputMessage;
        }

        public string[] getAllAccountId(bool isAllowDisabled)
        {
            string[] outputAccountId = new string[0];

            string strIsAllowDisabled = "";
            if (!isAllowDisabled)
                strIsAllowDisabled = "AND TableAccount.state = 1";
            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                int tempInt = -1;
                sqlcomm.CommandText = "SELECT COUNT(*) FROM TableAccount WHERE (TableAccount.id IS NOT NULL " + strIsAllowDisabled + ")";
                tempInt = (int)sqlcomm.ExecuteScalar();

                if (tempInt > 0)
                {
                    for (int i = 0; i < tempInt; i++)
                    {
                        sqlcomm.CommandText = "SELECT TAccount.id";
                        sqlcomm.CommandText += " FROM (SELECT TableAccount.id, ROW_NUMBER() OVER (ORDER BY TableAccount.id ASC) AS rn FROM TableAccount WHERE (TableAccount.id IS NOT NULL " + strIsAllowDisabled + "))";
                        sqlcomm.CommandText += " AS TAccount";
                        sqlcomm.CommandText += " WHERE rn BETWEEN " + Convert.ToString(i + 1) + " AND " + Convert.ToString(i + 1);
                        Array.Resize(ref outputAccountId, outputAccountId.Length + 1);
                        outputAccountId[i] = sqlcomm.ExecuteScalar() as string ?? "";
                    }
                }
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return new string[0];
            }
            return outputAccountId;
        }
    }
}
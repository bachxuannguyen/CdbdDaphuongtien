using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace cdbd_daphuongtien.App_Code
{
    public class Tag
    {
        CsLib csLib = new CsLib();
        DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public string id;
        public string name;
        public string ptag;
        public string ctag;
        public int state;

        /*
        public List<string> tagGetAutoComplete(string inputKeyword)
        {
            List<string> outputStr = new List<string>();

            string s = csLib.isValidString("Tag AutoComplete", inputKeyword, true, false, true);
            if (string.IsNullOrEmpty(s))
            {
                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(true);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();
                    sqlcomm.CommandText = "SELECT TOP 10 id from TableTag WHERE id LIKE ''+@inputKeyword+'%'";
                    sqlcomm.Parameters.AddWithValue("@inputKeyword", inputKeyword);
                    SqlDataReader dr = sqlcomm.ExecuteReader();
                    while (dr.Read())
                    {
                        outputStr.Add(dr["id"].ToString());
                    }
                    sqlconn.Close();
                }
                catch
                { }
            }

            return outputStr;
        }
        */

        public Tag tagGet(string inputTagId)
        {
            Tag currentTag = new Tag();

            string s = csLib.isValidString("Mã tag", inputTagId, false, false, true);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, inputTagId);
                return new Tag();
            }
            else
            {
                int x = dtLib.checkStrRecordExist("id", "TableTag", inputTagId);
                if (x < 0)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                    return new Tag();
                }
                else if (x != 1)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tag này trong cơ sở dữ liệu.", inputTagId);
                    return new Tag();
                }

                try
                {
                    currentTag.id = inputTagId;
                    currentTag.name = dtLib.getData_StrToStr("name", "TableTag", "id", inputTagId);
                    currentTag.ptag = dtLib.getData_StrToStr("ptag", "TableTag", "id", inputTagId);
                    currentTag.ctag = dtLib.getData_StrToStr("ctag", "TableTag", "id", inputTagId);
                    currentTag.state = dtLib.getData_StrToInt("state", "TableTag", "id", inputTagId);
                    string s1 = isValidTag(currentTag, false);
                    if (!string.IsNullOrEmpty(s1))
                    {
                        csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s1, inputTagId);
                        return new Tag();
                    }
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputTagId);
                    return new Tag();
                }
            }
            return currentTag;
        }

        public bool tagUpdate(Tag currentTag)
        {
            string s = isValidTag(currentTag, false);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentTag.id);
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentTag.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentTag.name);
                    sqlcomm.Parameters.AddWithValue("@Ptag", currentTag.ptag);
                    sqlcomm.Parameters.AddWithValue("@Ctag", currentTag.ctag);
                    sqlcomm.Parameters.AddWithValue("@State", currentTag.state);
                    sqlcomm.CommandText = "UPDATE TableTag SET "
                                        + "name=@Name, ptag=@Ptag, ctag=@Ctag, state=@State "
                                        + "WHERE id=@Id";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentTag.id);
                    return false;
                }
            }
        }

        public bool tagInsert(Tag currentTag)
        {
            string s = isValidTag(currentTag, true);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentTag.id);
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentTag.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentTag.name);
                    sqlcomm.Parameters.AddWithValue("@Ptag", currentTag.ptag);
                    sqlcomm.Parameters.AddWithValue("@Ctag", currentTag.ctag);
                    sqlcomm.Parameters.AddWithValue("@State", currentTag.state);
                    sqlcomm.CommandText = "INSERT INTO TableTag (id, name, ptag, ctag, state) "
                                        + "VALUES "
                                        + "(@Id, @Name, @Ptag, @Ctag, @State) ";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentTag.id);
                    return false;
                }
            }
        }

        public bool tagDelete(string inputId)
        {
            int x = dtLib.checkStrRecordExist("id", "TableTag", inputId);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return false;
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tag này trong cơ sở dữ liệu.", inputId);
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
                sqlcomm.CommandText = "DELETE FROM TableTag WHERE id=@Id;";
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

        public string isValidTag(Tag inputTag, bool isAlredyUseCheck)
        {
            string outputMessage = "";

            if (isAlredyUseCheck)
            {
                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(true);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();
                    sqlcomm.Parameters.Add(new SqlParameter("@Id", SqlDbType.NVarChar));
                    sqlcomm.Parameters["@Id"].Value = inputTag.id;
                    sqlcomm.CommandText = "SELECT id FROM TableTag WHERE id=@Id";
                    string s = sqlcomm.ExecuteScalar() as string ?? "";
                    sqlconn.Close();
                    if (!string.IsNullOrEmpty(s) && s == inputTag.id)
                        return "Tag - Mã tag này đã được sử dụng.<br />";
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputTag.id);
                    return e.Message;
                }
            }

            if (!string.IsNullOrEmpty(inputTag.ptag) && !string.IsNullOrEmpty(inputTag.ctag))
                return "Tag - Liên kết (cTag và pTag) không hợp lệ.";

            outputMessage += csLib.isValidString("Tag - Mã", inputTag.id, false, false, true);
            outputMessage += csLib.isValidString("Tag - Tên", inputTag.name, true, false, true);
            outputMessage += csLib.isValidString("Tag - Cấp trên", inputTag.ptag, true, true, true);
            outputMessage += csLib.isValidString("Tag - Cấp dưới", inputTag.ctag, true, true, true);
            if (inputTag.state != 0 && inputTag.state != 1)
                outputMessage += "Tag - Trạng thái: Giá trị không hợp lệ.";

            return outputMessage;
        }

        public string[] getMyCTagId(string inputTagId, bool includePTag)
        {
            string[] outputTagId = new string[0];

            string s = csLib.isValidString("Mã tag", inputTagId, false, false, true);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, inputTagId);
                return new string[0];
            }
            else
            {
                try
                {
                    if (includePTag)
                    {
                        Array.Resize(ref outputTagId, 1);
                        outputTagId[0] = inputTagId;
                    }
                    else
                        Array.Resize(ref outputTagId, 0);

                    string tempString = dtLib.getData_StrToStr("ctag", "TableTag", "id", inputTagId);
                    if (!string.IsNullOrEmpty(tempString))
                    {
                        string[] ctagIdArray = csLib.toStrArray(tempString);
                        outputTagId = outputTagId.Concat(ctagIdArray).ToArray();
                    }
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputTagId);
                    return new string[0];
                }
            }

            return outputTagId;
        }

        public string[] getAllPTagId(bool isAllowDisabled)
        {
            string[] outputTagId = new string[0];

            string strIsAllowDisabled = "";
            if (!isAllowDisabled)
                strIsAllowDisabled = "AND TableTag.state = 1";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                int tempInt = -1;
                sqlcomm.CommandText = "SELECT COUNT(*) FROM TableTag WHERE ((TableTag.ptag IS NULL OR TableTag.ptag = '') " + strIsAllowDisabled + ")";
                tempInt = (int)sqlcomm.ExecuteScalar();

                if (tempInt > 0)
                {
                    for (int i = 0; i < tempInt; i++)
                    {
                        sqlcomm.CommandText = "SELECT TTag.id";
                        sqlcomm.CommandText += " FROM (SELECT TableTag.id, row_number() OVER (ORDER BY TableTag.id ASC) as rn FROM TableTag WHERE ((TableTag.ptag IS NULL OR TableTag.ptag = '') " + strIsAllowDisabled + "))";
                        sqlcomm.CommandText += " AS TTag";
                        sqlcomm.CommandText += " WHERE rn BETWEEN " + Convert.ToString(i + 1) + " AND " + Convert.ToString(i + 1);
                        Array.Resize(ref outputTagId, outputTagId.Length + 1);
                        outputTagId[i] = sqlcomm.ExecuteScalar() as string ?? "";
                    }
                }
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return new string[0];
            }
            return outputTagId;
        }

        public string[] getAllTagId(bool isAllowDisabled)
        {
            string[] outputTagId = new string[0];

            string strIsAllowDisabled = "";
            if (!isAllowDisabled)
            {
                strIsAllowDisabled = "AND TableTag.state = 1";
            }
            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();
                int tempInt = -1;
                sqlcomm.CommandText = "SELECT count(*) FROM TableTag WHERE (TableTag.id IS NOT NULL " + strIsAllowDisabled + ")";
                tempInt = (int)sqlcomm.ExecuteScalar();

                if (tempInt > 0)
                {
                    for (int i = 0; i < tempInt; i++)
                    {
                        sqlcomm.CommandText = "SELECT TTag.id";
                        sqlcomm.CommandText += " FROM (SELECT TableTag.id, row_number() OVER (ORDER BY TableTag.id ASC) as rn FROM TableTag WHERE (TableTag.id IS NOT NULL " + strIsAllowDisabled + "))";
                        sqlcomm.CommandText += " AS TTag";
                        sqlcomm.CommandText += " WHERE rn BETWEEN " + Convert.ToString(i + 1) + " AND " + Convert.ToString(i + 1);
                        Array.Resize(ref outputTagId, outputTagId.Length + 1);
                        outputTagId[i] = sqlcomm.ExecuteScalar() as string ?? "";
                    }
                }
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return new string[0];
            }
            return outputTagId;
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;

namespace cdbd_daphuongtien.App_Code
{
    public class Author
    {
        CsLib csLib = new CsLib();
        DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public int id;
        public string name;
        public string contact;
        public string media;
        public int state;

        public Author authorGet(int inputId)
        {
            Author currentAuthor = new Author();
            int x = dtLib.checkIntRecordExist("id", "TableAuthor", inputId);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return new Author();
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tác giả này trong cơ sở dữ liệu.", "");
                return new Author();
            }

            try
            {
                currentAuthor.id = inputId;
                currentAuthor.name = dtLib.getData_IntToStr("name", "TableAuthor", "id", inputId);
                currentAuthor.contact = dtLib.getData_IntToStr("contact", "TableAuthor", "id", inputId);
                currentAuthor.media = dtLib.getData_IntToStr("media", "TableAuthor", "id", inputId);
                currentAuthor.state = dtLib.getData_IntToInt("state", "TableAuthor", "id", inputId);
                string s = isValidAuthor(currentAuthor);
                if (!string.IsNullOrEmpty(s))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, inputId.ToString());
                    return new Author();
                }
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputId.ToString());
                return new Author();
            }
            return currentAuthor;
        }

        public bool authorUpdate(Author currentAuthor)
        {
            string s = isValidAuthor(currentAuthor);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentAuthor.id.ToString());
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentAuthor.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentAuthor.name);
                    sqlcomm.Parameters.AddWithValue("@Contact", currentAuthor.contact);
                    sqlcomm.Parameters.AddWithValue("@Media", currentAuthor.media);
                    sqlcomm.Parameters.AddWithValue("@State", currentAuthor.state);
                    sqlcomm.CommandText = "UPDATE TableAuthor SET "
                                        + "name=@Name, contact=@Contact, media=@Media, state=@State "
                                        + "WHERE id=@Id";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentAuthor.id.ToString());
                    return false;
                }
            }
        }

        public bool authorInsert(Author currentAuthor)
        {
            string s = isValidAuthor(currentAuthor);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentAuthor.id.ToString());
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentAuthor.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentAuthor.name);
                    sqlcomm.Parameters.AddWithValue("@Contact", currentAuthor.contact);
                    sqlcomm.Parameters.AddWithValue("@Media", currentAuthor.media);
                    sqlcomm.Parameters.AddWithValue("@State", currentAuthor.state);

                    sqlcomm.CommandText = "INSERT INTO TableAuthor (id, name, contact, media, state) "
                                        + "VALUES "
                                        + "(@Id, @Name, @Contact, @Media, @State) ";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentAuthor.id.ToString());
                    return false;
                }
            }
        }

        public bool authorDelete(string authorId)
        {
            return false;
        }

        public string isValidAuthor(Author inputAuthor)
        {
            string outputMessage = "";

            outputMessage += csLib.isValidString("Author - Tên", inputAuthor.name, true, false, true);
            outputMessage += csLib.isValidString("Author - Liên hệ", inputAuthor.contact, true, false, true);
            outputMessage += csLib.isValidString("Author - Tác phẩm", inputAuthor.media, true, true, true);
            if (inputAuthor.state != 0 && inputAuthor.state != 1)
                outputMessage += "Author - Trạng thái: Giá trị không hợp lệ.";

            return outputMessage;
        }
    }
}
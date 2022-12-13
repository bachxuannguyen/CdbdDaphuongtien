using System;
using System.Data;
using System.Data.SqlClient;

namespace cdbd_daphuongtien.App_Code
{
    public class Media
    {
        MediaFile mediaFile = new MediaFile();
        CsLib csLib = new CsLib();
        DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public int id;
        public string name;
        public int type;
        public int album;
        public string url;
        public string description;
        public string tag;
        public DateTime datetime;
        public int state;
        public int author;

        public Media mediaGet(int inputId)
        {
            Media currentMedia = new Media();

            int x = dtLib.checkIntRecordExist("id", "TableMedia", inputId);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return new Media();
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã media này trong cơ sở dữ liệu.", inputId.ToString());
                return new Media();
            }

            try
            {
                currentMedia.id = inputId;
                currentMedia.name = dtLib.getData_IntToStr("name", "TableMedia", "id", inputId);
                currentMedia.album = dtLib.getData_IntToInt("album", "TableMedia", "id", inputId);
                currentMedia.type = dtLib.getData_IntToInt("type", "TableMedia", "id", inputId);
                currentMedia.url = dtLib.getData_IntToStr("url", "TableMedia", "id", inputId);
                currentMedia.description = dtLib.getData_IntToStr("description", "TableMedia", "id", inputId);
                currentMedia.tag = dtLib.getData_IntToStr("tag", "TableMedia", "id", inputId);
                currentMedia.datetime = dtLib.getData_IntToDt("datetime", "TableMedia", "id", inputId);
                currentMedia.state = dtLib.getData_IntToInt("state", "TableMedia", "id", inputId);
                currentMedia.author = dtLib.getData_IntToInt("author", "TableMedia", "id", inputId);
                string s = isValidMedia(currentMedia);
                if (!string.IsNullOrEmpty(s))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, inputId.ToString());
                    return new Media();
                }
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputId.ToString());
                return new Media();
            }
            return currentMedia;
        }

        public bool mediaUpdate(Media currentMedia)
        {
            string s = isValidMedia(currentMedia);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentMedia.id.ToString());
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentMedia.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentMedia.name);
                    sqlcomm.Parameters.AddWithValue("@Type", currentMedia.type);
                    sqlcomm.Parameters.AddWithValue("@Album", currentMedia.album);
                    sqlcomm.Parameters.AddWithValue("@Url", currentMedia.url);
                    sqlcomm.Parameters.AddWithValue("@Description", currentMedia.description);
                    sqlcomm.Parameters.AddWithValue("@Tag", currentMedia.tag);
                    sqlcomm.Parameters.AddWithValue("@Datetime", currentMedia.datetime);
                    sqlcomm.Parameters.AddWithValue("@State", currentMedia.state);
                    sqlcomm.Parameters.AddWithValue("@Author", currentMedia.author);

                    sqlcomm.CommandText = "UPDATE TableMedia SET "
                                        + "name=@Name, type=@Type, album=@Album, url=@Url, description=@Description, "
                                        + "tag=@Tag, datetime=@Datetime, state=@State, author=@Author "
                                        + "WHERE id=@Id";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentMedia.id.ToString());
                    return false;
                }
            }
        }

        public bool mediaInsert(Media currentMedia)
        {
            string s = isValidMedia(currentMedia);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentMedia.id.ToString());
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentMedia.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentMedia.name);
                    sqlcomm.Parameters.AddWithValue("@Type", currentMedia.type);
                    sqlcomm.Parameters.AddWithValue("@Album", currentMedia.album);
                    sqlcomm.Parameters.AddWithValue("@Url", currentMedia.url);
                    sqlcomm.Parameters.AddWithValue("@Description", currentMedia.description);
                    sqlcomm.Parameters.AddWithValue("@Tag", currentMedia.tag);
                    sqlcomm.Parameters.AddWithValue("@Datetime", currentMedia.datetime);
                    sqlcomm.Parameters.AddWithValue("@State", currentMedia.state);
                    sqlcomm.Parameters.AddWithValue("@Author", currentMedia.author);

                    sqlcomm.CommandText = "INSERT INTO TableMedia (id, name, type, album, url, description, tag, datetime, state, author) "
                                        + "VALUES "
                                        + "(@Id, @Name, @Type, @Album, @Url, @Description, @Tag, @Datetime, @State, @Author)";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentMedia.id.ToString());
                    return false;
                }
            }
        }

        public int mediaDelete(Media media, bool deleteFile)
        {
            int x = dtLib.checkIntRecordExist("id", "TableMedia", media.id);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return 0;
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã media này trong cơ sở dữ liệu.", media.id.ToString());
                return 0;
            }

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(true);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.Parameters.AddWithValue("@Id", media.id);
                sqlcomm.CommandText = "DELETE FROM TableMedia WHERE id=@Id;";
                sqlcomm.ExecuteScalar();

                sqlconn.Close();

                if (deleteFile)
                {
                    if (!mediaFile.deleteFile(media.url))
                    {
                        csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", media.id.ToString());
                        return 2;
                    }
                }

                return 1;
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, media.id.ToString());
                return 0;
            }
        }

        public string isValidMedia(Media inputMedia)
        {
            string outputMessage = "";

            outputMessage += csLib.isValidString("Media - Tên", inputMedia.name, true, false, true);
            outputMessage += csLib.isValidString("Media - Đường dẫn", inputMedia.url, true, false, true);
            outputMessage += csLib.isValidString("Media - Mô tả", inputMedia.description, true, true, true);
            outputMessage += csLib.isValidString("Media - Tag", inputMedia.tag, true, true, true);
            if (inputMedia.state != 0 && inputMedia.state != 1)
                outputMessage += "Media - Trạng thái: Giá trị không hợp lệ.";

            return outputMessage;
        }
    }
}
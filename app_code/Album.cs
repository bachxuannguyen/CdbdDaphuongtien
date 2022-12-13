using System;
using System.Data;
using System.Data.SqlClient;

namespace cdbd_daphuongtien.App_Code
{
    public class Album
    {
        MediaFile mediaFile = new MediaFile();
        CsLib csLib = new CsLib();
        DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public int id;
        public string name;
        public string description;
        public DateTime datetime;
        public string tag;
        public string media;
        public int state;

        public Album albumGet(int albumId)
        {
            Album currentAlbum = new Album();

            int x = dtLib.checkIntRecordExist("id", "TableAlbum", albumId);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return new Album();
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã album này trong cơ sở dữ liệu.", albumId.ToString());
                return new Album();
            }

            try
            {
                currentAlbum.id = albumId;
                currentAlbum.name = dtLib.getData_IntToStr("name", "TableAlbum", "id", albumId);
                currentAlbum.media = dtLib.getData_IntToStr("media", "TableAlbum", "id", albumId);
                currentAlbum.description = dtLib.getData_IntToStr("description", "TableAlbum", "id", albumId);
                currentAlbum.tag = dtLib.getData_IntToStr("tag", "TableAlbum", "id", albumId);
                currentAlbum.datetime = dtLib.getData_IntToDt("datetime", "TableAlbum", "id", albumId);
                currentAlbum.state = dtLib.getData_IntToInt("state", "TableAlbum", "id", albumId);
                string s = isValidAlbum(currentAlbum);
                if (!string.IsNullOrEmpty(s))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, albumId.ToString());
                    return new Album();
                }
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, albumId.ToString());
                return new Album();
            }
            return currentAlbum;
        }

        public bool albumUpdate(Album currentAlbum)
        {
            string s = isValidAlbum(currentAlbum);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentAlbum.id.ToString());
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentAlbum.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentAlbum.name);
                    sqlcomm.Parameters.AddWithValue("@Media", currentAlbum.media);
                    sqlcomm.Parameters.AddWithValue("@State", currentAlbum.state);
                    sqlcomm.Parameters.AddWithValue("@Description", currentAlbum.description);
                    sqlcomm.Parameters.AddWithValue("@Tag", currentAlbum.tag);
                    sqlcomm.Parameters.AddWithValue("@Datetime", currentAlbum.datetime);


                    sqlcomm.CommandText = "UPDATE TableAlbum SET "
                                        + "name=@Name, media=@Media, state=@State, "
                                        + "description=@Description, tag=@Tag, datetime=@Datetime "
                                        + "WHERE id=@Id";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentAlbum.id.ToString());
                    return false;
                }
            }
        }

        public bool albumInsert(Album currentAlbum)
        {
            string s = isValidAlbum(currentAlbum);
            if (!string.IsNullOrEmpty(s))
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, s, currentAlbum.id.ToString());
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

                    sqlcomm.Parameters.AddWithValue("@Id", currentAlbum.id);
                    sqlcomm.Parameters.AddWithValue("@Name", currentAlbum.name);
                    sqlcomm.Parameters.AddWithValue("@Media", currentAlbum.media);
                    sqlcomm.Parameters.AddWithValue("@State", currentAlbum.state);
                    sqlcomm.Parameters.AddWithValue("@Description", currentAlbum.description);
                    sqlcomm.Parameters.AddWithValue("@Tag", currentAlbum.tag);
                    sqlcomm.Parameters.AddWithValue("@Datetime", currentAlbum.datetime);

                    sqlcomm.CommandText = "INSERT INTO TableAlbum (id, name, media, state, description, tag, datetime) "
                                        + "VALUES "
                                        + "(@Id, @Name, @Media, @State, @Description, @Tag, @Datetime)";
                    sqlcomm.ExecuteScalar();
                    sqlconn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, currentAlbum.id.ToString());
                    return false;
                }
            }
        }

        public int albumDelete(Album album, bool deleteMedia, bool deleteFile)
        {
            int x = dtLib.checkIntRecordExist("id", "TableAlbum", album.id);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return 0;
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã album này trong cơ sở dữ liệu.", album.id.ToString());
                return 0;
            }

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(true);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.Parameters.AddWithValue("@Id", album.id);
                sqlcomm.CommandText = "DELETE FROM TableAlbum WHERE id=@Id;";
                sqlcomm.ExecuteScalar();

                sqlconn.Close();

                bool allMediaDeleted = true;
                if (deleteMedia)
                {
                    if (!string.IsNullOrEmpty(album.media))
                    {
                        int[] mediaId = csLib.toIntArray(album.media);
                        if (mediaId.Length > 0)
                        {
                            for (int i = 0; i < mediaId.Length; i++)
                            {
                                Media media = new Media();
                                media = media.mediaGet(mediaId[i]);
                                if (media.id == 0)
                                    continue;
                                if (media.mediaDelete(media, deleteFile) != 1)
                                {
                                    allMediaDeleted = false;
                                    continue;
                                }
                            }
                        }
                    }
                }
                if (allMediaDeleted)
                    return 1;
                else
                    return 2;
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, album.id.ToString());
                return 0;
            }
        }

        public string isValidAlbum(Album inputAlbum)
        {
            string outputMessage = "";

            outputMessage += csLib.isValidString("Album - Tên", inputAlbum.name, true, false, true);
            outputMessage += csLib.isValidString("Album - Media", inputAlbum.media, true, false, true);
            outputMessage += csLib.isValidString("Album - Mô tả", inputAlbum.description, true, true, true);
            outputMessage += csLib.isValidString("Album - Tag", inputAlbum.tag, true, true, true);
            if (inputAlbum.state != 0 && inputAlbum.state != 1)
                outputMessage += "Album - Trạng thái: Giá trị không hợp lệ.";

            return outputMessage;
        }

        public int[] getMediaCount(int albumId)
        {
            int[] output = { 0, 0, 0, 0, 0 };
            int[] errArray = { -1 };

            int x = dtLib.checkIntRecordExist("id", "TableAlbum", albumId);
            if (x < 0)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "", "");
                return errArray;
            }
            else if (x != 1)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã album này trong cơ sở dữ liệu.", "");
                return errArray;
            }

            string albumMedia = dtLib.getData_IntToStr("media", "TableAlbum", "id", albumId);
            if (!string.IsNullOrEmpty(albumMedia))
            {
                string[] albumMediaArray = csLib.toStrArray(albumMedia);
                if (albumMediaArray.Length > 0)
                {
                    for (int i = 0; i < albumMediaArray.Length; i++)
                    {
                        int type = dtLib.getData_StrToInt("type", "TableMedia", "id", albumMediaArray[i]);
                        switch (type)
                        {
                            case 1:
                                output[0]++;
                                break;
                            case 2:
                                output[1]++;
                                break;
                            case 3:
                                output[2]++;
                                break;
                            case 4:
                                output[3]++;
                                break;
                            case 5:
                                output[4]++;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                    return errArray;
            }

            return output;
        }
    }
}
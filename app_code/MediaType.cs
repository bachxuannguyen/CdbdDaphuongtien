using System.Data.SqlClient;

namespace cdbd_daphuongtien.App_Code
{
    public class MediaType
    {
        CsLib csLib = new CsLib();
        DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public int id;
        public string name;
        public int state;

        public int[] getAllId()
        {
            int[] output = { 1, 2, 3, 4, 5 };
            return output;
        }

        public string[] getAllName()
        {
            string[] output = { "Hình ảnh", "Infographic", "Video", "Âm thanh", "Hoạt họa" };
            return output;
        }

        public string getNameById(int inputId)
        {
            switch (inputId)
            {
                case 1:
                    return "Hình ảnh";
                case 2:
                    return "Infographic";
                case 3:
                    return "Video";
                case 4:
                    return "Âm thanh";
                case 5:
                    return "Hoạt họa";
                default:
                    return "";
            }
        }

        public int getIdByExtension(string fileExtension)
        {
            int output = -1;
            switch (fileExtension.ToLower())
            {
                case ".bmp":
                case ".gif":
                case ".png":
                case ".jpg":
                case ".jpeg":
                    output = 1;
                    break;
                case ".mp4":
                case ".wmv":
                case ".mov":
                    output = 3;
                    break;
                case ".mp3":
                case ".wma":
                    output = 4;
                    break;
                case ".swf":
                case ".fla":
                    output = 5;
                    break;
                default:
                    break;
            }
            return output;
        }
    }
}
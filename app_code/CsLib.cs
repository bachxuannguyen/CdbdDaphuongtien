using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace cdbd_daphuongtien.App_Code
{
    public class CsLib
    {
        public string varServerName = ".";
        public string varUsrName = "cdbd_mm_usr";
        public string varAdmName = "cdbd_mm_adm";
        public string varUsrPassword = "@Bb1234567890";
        public string varAdmPassword = "@Bb1234567890";
        public string varDatabase = "cdbd_mm";
        public int varRowsPerPage = 10;
        public bool varEncodeQs = true;

        public string varRootAccount = "administrator";

        public int varSmallViewPer = 20;
        public int varMediumViewPer = 50;
        public int varLargeViewPer = 100;
        public int varSmallViewPx = 150;
        public int varMediumViewPx = 300;
        public int varLargeViewPx = 500;

        public int varMaxMediaPreview = 7;
        public string varHomePage = "http://www.cdbd.edu.vn/sites/daphuongtien";
        public string varSiteName = "Hệ thống Đa phương tiện";
        public string varUploadFolder = "./files/media";

        public string varStrEmpty = "[Không]";
        public string varStrDisabled = "[Không khả dụng]";
        public string varMediaDisabled = "./images/broken.png";

        public string[] varAllowedFileTypes = { "bmp", "gif", "png", "jpg", "jpeg", "mp3", "wma", "mp4", "wmv", "avi", "mov", "swf", "fla" };

        public string varColorGreenBg = "6bac68";
        public string varColorRedBg = "ba6262";
        public string varColorBlueBg = "498ca9";
        public string varColorGreen = "358739";
        public string varColorRed = "a42323";
        public string varColorBlue = "2c629d";

        int maxStringLength = 1024;

        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();
        Random random = new Random();

        public bool isLoggedIn()
        {
            if ((string)HttpContext.Current.Session["account_isLoggedIn"] == "Y")
                return true;
            else
                return false;
            //return true;
        }

        public bool isRootLogin()
        {
            if ((string)HttpContext.Current.Session["account_isLoggedIn"] == "Y")
            {
                if (varRootAccount == (string)HttpContext.Current.Session["account_userId"])
                {
                    return true;
                }
            }

            return false;
        }

        public string getConnectionString(bool isDatareader)
        {
            if (!isDatareader)
                return "server='" + varServerName + "'; UID='" + varUsrName + "'; PWD='" + varUsrPassword + "'; database=" + varDatabase;
            else
                return "server='" + varServerName + "'; UID='" + varAdmName + "'; PWD='" + varAdmPassword + "'; database=" + varDatabase;
        }

        public string encodeString(string inputString)
        {
            if (varEncodeQs)
            {
                string outputString = "";
                int keyLength = 16;
                string[] randomString = new string[30];
                Random r = new Random();

                if (String.IsNullOrEmpty(inputString))
                    return "";
                else if (inputString.Length > 255)
                    return "";
                else if (keyLength < 0 | keyLength >= 20)
                    return "";
                else
                {
                    Array.Resize(ref randomString, inputString.Length);
                    for (int i = 0; i < inputString.Length; i++)
                    {
                        randomString[i] = randomString[i] + Convert.ToString(r.Next(10));
                    }
                    for (int j = 0; j < inputString.Length; j++)
                    {
                        outputString = outputString + randomString[j];
                        outputString = outputString + String.Format("{0:X}", Convert.ToInt32(inputString[j]) - keyLength);
                    }
                    return outputString;
                }
            }
            else
                return inputString;
        }

        public string decodeString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";

            if (varEncodeQs)
            {
                string outputString = "";
                string tempString = "";
                int keyLength = 16;

                try
                {
                    if (inputString.Length < 3 || inputString.Length > 1024)
                        return "";
                    else if (keyLength < 0 | keyLength >= 20)
                        return "";
                    else
                    {
                        for (int i = 2; i < inputString.Length; i = i + 3)
                        {
                            tempString = Convert.ToString(Convert.ToChar(Int32.Parse(inputString.Substring(i - 1, 2), System.Globalization.NumberStyles.HexNumber) + keyLength));
                            outputString = outputString + tempString;
                        }
                        return outputString;
                    }
                }
                catch
                {
                    return "";
                }
            }
            else
                return inputString;
        }

        public string isValidString(string stringSender, string inputString, bool isAllowSpecialChar, bool isAllowNull, bool isLengthLimit)
        {
            string outputMessage = "";

            if (isAllowSpecialChar == false)
            {
                if (isContainSpecialChar(inputString))
                {
                    outputMessage += stringSender + ": Ký tự đặc biệt không được chấp nhận.<br />";
                }
            }
            if (isAllowNull == false)
            {
                if (string.IsNullOrEmpty(inputString))
                {
                    outputMessage += stringSender + ": Trường này không được để trống.<br />";
                }
            }
            if (isLengthLimit == true)
            {
                if (string.IsNullOrEmpty(inputString) == false)
                {
                    if (inputString.Length > maxStringLength)
                    {
                        outputMessage += stringSender + ": Số ký tự vượt quá giới hạn cho phép.<br />";
                    }
                }
            }
            return outputMessage;
        }

        public string stringStandardization(string inputString)
        {
            string outputString = "";

            if (string.IsNullOrEmpty(inputString))
                outputString = "";
            else
            {
                inputString = inputString.Trim();
                outputString = System.Text.RegularExpressions.Regex.Replace(inputString, " {2,}", " ");
            }
            return outputString;
        }

        public bool isContainSpecialChar(string inputString)
        {
            bool output = false;

            if (string.IsNullOrEmpty(inputString) == false)
            {
                foreach (char c in inputString)
                {
                    if (!char.IsWhiteSpace(c) && !char.IsDigit(c) && !char.IsLetter(c) && c != '-')
                    {
                        output = true;
                    }
                }
            }

            return output;
        }

        public void toErrControl(string errSender, string errMessage, string linkDisplay, string linkUrl)
        {
            HttpContext.Current.Session["err_sender"] = errSender;
            HttpContext.Current.Session["err_message"] = errMessage;
            HttpContext.Current.Session["err_linkUrl"] = linkUrl;
            HttpContext.Current.Session["err_linkDisplay"] = linkDisplay;
            HttpContext.Current.Response.Redirect("ErrControl.aspx", false);
        }

        public string[] toStrArray(string inputString)
        {
            string[] outputArray = new string[0];

            if (!string.IsNullOrEmpty(inputString))
            {
                if (inputString.Substring(inputString.Length - 1, 1) != ",")
                    return new string[0];
                else
                {
                    try
                    {
                        inputString = inputString.Substring(0, inputString.Length - 1);
                        string[] tempArray = inputString.Split(',');
                        Array.Resize(ref outputArray, tempArray.Length);
                        outputArray = tempArray;
                    }
                    catch (Exception e)
                    {
                        errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputString);
                        return new string[0];
                    }
                }
            }

            return outputArray;
        }

        public int[] toIntArray(string inputString)
        {
            int[] outputArray = new int[0];

            if (!string.IsNullOrEmpty(inputString))
            {
                if (inputString.Substring(inputString.Length - 1, 1) != ",")
                    return new int[0];
                else
                {
                    try
                    {
                        inputString = inputString.Substring(0, inputString.Length - 1);
                        int[] tempArray = Array.ConvertAll(inputString.Split(','), int.Parse);
                        Array.Resize(ref outputArray, tempArray.Length);
                        outputArray = tempArray;
                    }
                    catch (Exception e)
                    {
                        errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputString);
                        return new int[0];
                    }
                }
            }

            return outputArray;
        }

        public DateTime[] toDtArray(string inputString)
        {
            DateTime[] outputArray = new DateTime[0];

            if (!string.IsNullOrEmpty(inputString))
            {
                if (inputString.Substring(inputString.Length - 1, 1) != ",")
                    return new DateTime[0];
                else
                {
                    try
                    {
                        DateTime[] tempArray = Array.ConvertAll(inputString.Split(','), DateTime.Parse);
                        Array.Resize(ref outputArray, tempArray.Length);
                        outputArray = tempArray;
                    }
                    catch (Exception e)
                    {
                        errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, inputString);
                        return new DateTime[0];
                    }
                }
            }

            return outputArray;
        }

        public string createFileNamePrefix(int strLen)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string createRandom = new string(Enumerable.Repeat(chars, strLen).Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();
            /*
            DateTime now = DateTime.Now;
            string year = (9999 - now.Year).ToString();
            if (year.Length < 2)
                year = "000" + year;
            else if (year.Length < 3)
                year = "00" + year;
            else if (year.Length < 4)
                year = "0" + year;
            string month = (99 - now.Month).ToString();
            if (month.Length < 2)
                month = "0" + month;
            string day = (99 - now.Day).ToString();
            if (day.Length < 2)
                day = "0" + day;
            string hour = (99 - now.Hour).ToString();
            if (hour.Length < 2)
                hour = "0" + hour;
            string minute = (99 - now.Minute).ToString();
            if (minute.Length < 2)
                minute = "0" + minute;
            string second = (99 - now.Second).ToString();
            if (second.Length < 2)
                second = "0" + second;
            createRandom = year + month + day + hour + minute + second + "-" + createRandom;
            */
            return createRandom;
        }

        public void errorTracer(string procedureName, string procedureMsg, string procedurePar)
        {
            if (string.IsNullOrEmpty(procedureName))
                procedureName = "-";
            if (string.IsNullOrEmpty(procedureMsg))
                procedureMsg = "-";
            if (string.IsNullOrEmpty(procedurePar))
                procedurePar = "-";
            HttpContext.Current.Session["procedure_Time"] += DateTime.Now.ToString() + ",";
            HttpContext.Current.Session["procedure_Name"] += procedureName + ",";
            HttpContext.Current.Session["procedure_Message"] += procedureMsg + ",";
            HttpContext.Current.Session["procedure_Parameter"] += procedurePar + ",";
        }
    }
}
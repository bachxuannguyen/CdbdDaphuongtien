using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace cdbd_daphuongtien.App_Code
{
    public class SearchEngine
    {
        public CsLib csLib = new CsLib();
        public DtLib dtLib = new DtLib();
        SqlConnection sqlconn = new SqlConnection();
        SqlCommand sqlcomm = new SqlCommand();

        public int[] mediaSearch(bool isByName, string nameKey, bool isByType, int typeKey, bool isByDesc, string descKey, bool isByState, int stateKey, bool isByAuthor, string authorKey, bool isByDatetime, int byDatetimeType, DateTime fDatetime, DateTime sDatetime)
        {
            int[] outputMediaId = new int[0];
            string tempString = "";

            string[] mdlString = { "", "", "", "", "", "" };

            if (isByName)
            {
                nameKey = csLib.stringStandardization(nameKey);
                tempString = csLib.isValidString("Media Search - nameKey", nameKey, true, false, true);
                if (!string.IsNullOrEmpty(tempString))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, tempString, nameKey);
                    return new int[0];
                }
                else
                    //mdlString[0] = " AND CONTAINS (TableMedia.name, '\"*" + nameKey + "*\"')";
                    mdlString[0] = " AND TableMedia.name LIKE N'%" + nameKey + "%'";
            }

            if (isByType)
            {
                if (typeKey == 0 || typeKey == 1 || typeKey == 2 || typeKey == 3 || typeKey == 4 || typeKey == 5)
                    mdlString[1] = " AND TableMedia.type = " + typeKey.ToString();
            }

            if (isByDesc)
            {
                descKey = csLib.stringStandardization(descKey);
                tempString = csLib.isValidString("Media Search - descKey", descKey, true, false, true);
                if (!string.IsNullOrEmpty(tempString))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, tempString, descKey);
                    return new int[0];
                }
                else
                    mdlString[2] = " AND TableMedia.description LIKE N'%" + descKey + "%'";
            }

            if (isByState)
            {
                if (stateKey == 0 || stateKey == 1)
                    mdlString[3] = " AND TableMedia.state = " + stateKey.ToString();
            }

            if (isByAuthor)
            {
                mdlString[4] = " AND TableMedia.author = " + authorKey;
            }

            if (isByDatetime)
            {
                string dtOperator = "";
                string dtFirst = "";
                string dtSecond = "";
                dtFirst = fDatetime.ToString("dd/MM/yyyy");
                dtSecond = sDatetime.ToString("dd/MM/yyyy");
                if (byDatetimeType != 1)
                {
                    if (byDatetimeType == 2)
                    {
                        dtOperator = "<";
                        mdlString[5] = " AND TableMedia.datetime " + dtOperator + " '" + dtFirst + "'";
                    }
                    else if (byDatetimeType == 3)
                    {
                        dtOperator = ">";
                        mdlString[5] = " AND TableMedia.datetime " + dtOperator + " dateadd(day, 1,  '" + dtFirst + "')";
                    }
                    else
                    {
                        mdlString[5] = " AND TableMedia.datetime >= " + "'" + dtFirst + "' AND TableMedia.datetime < dateadd(day, 1, '" + dtSecond + "')";
                    }
                }
                else
                {
                    mdlString[5] = " AND TableMedia.datetime " + " >= '" + dtFirst + "' AND TableMedia.datetime " + " < dateadd(day, 1, '" + dtFirst + "')";
                }
            }

            string queryMediaCount = "SET DATEFORMAT dmy;";
            queryMediaCount += "SELECT count(*)";
            queryMediaCount += " FROM TableMedia WHERE TableMedia.id > -1";
            string queryMediaId = "SET DATEFORMAT dmy;";
            queryMediaId += "SELECT id";
            queryMediaId += " FROM (SELECT TableMedia.id, row_number() OVER (ORDER BY TableMedia.id DESC) as rn FROM TableMedia";
            queryMediaId += " WHERE TableMedia.id > -1";

            for (int i = 0; i < mdlString.Length; i++)
            {
                queryMediaCount += mdlString[i];
                queryMediaId += mdlString[i];
            }

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.CommandText = queryMediaCount;
                int mediaCount = (Int32)sqlcomm.ExecuteScalar();
                if (mediaCount > 0)
                {
                    for (int i = 0; i < mediaCount; i++)
                    {
                        Array.Resize(ref outputMediaId, i + 1);
                        sqlcomm.CommandText = queryMediaId + ") A WHERE rn BETWEEN " + Convert.ToString(i + 1) + " AND " + Convert.ToString(i + 1);
                        outputMediaId[i] = (Int32)sqlcomm.ExecuteScalar();
                    }
                }
                else
                    return new int[0];

                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return new int[0];
            }

            return outputMediaId;
        }

        public int[] albumSearch(bool isByName, string nameKey, bool isByDesc, string descKey, bool isByState, int stateKey, bool isByDatetime, int byDatetimeType, DateTime fDatetime, DateTime sDatetime)
        {
            int[] outputAlbumId = new int[0];
            string tempString = "";

            string[] mdlString = { "", "", "", "" };

            if (isByName)
            {
                nameKey = csLib.stringStandardization(nameKey);
                tempString = csLib.isValidString("Album Search - nameKey", nameKey, true, false, true);
                if (!string.IsNullOrEmpty(tempString))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, tempString, nameKey);
                    return new int[0];
                }
                else
                    mdlString[0] = " AND TableAlbum.name LIKE N'%" + nameKey + "%'";
            }

            if (isByDesc)
            {
                descKey = csLib.stringStandardization(descKey);
                tempString = csLib.isValidString("Album Search - descKey", descKey, true, false, true);
                if (!string.IsNullOrEmpty(tempString))
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, tempString, descKey);
                    return new int[0];
                }
                else
                    mdlString[1] = " AND TableAlbum.description LiKE N'%" + descKey + "%'";
            }

            if (isByState)
            {
                if (stateKey == 0 || stateKey == 1)
                    mdlString[2] = " AND TableAlbum.state = " + stateKey.ToString();
            }

            if (isByDatetime)
            {
                string dtOperator = "";
                string dtFirst = "";
                string dtSecond = "";
                dtFirst = fDatetime.ToString("dd/MM/yyyy");
                dtSecond = sDatetime.ToString("dd/MM/yyyy");
                if (byDatetimeType != 1)
                {
                    if (byDatetimeType == 2)
                    {
                        dtOperator = "<";
                        mdlString[3] = " AND TableAlbum.datetime " + dtOperator + " '" + dtFirst + "'";
                    }
                    else if (byDatetimeType == 3)
                    {
                        dtOperator = ">";
                        mdlString[3] = " AND TableAlbum.datetime " + dtOperator + " dateadd(day, 1,  '" + dtFirst + "')";
                    }
                    else
                    {
                        mdlString[3] = " AND TableAlbum.datetime >= " + "'" + dtFirst + "' AND TableAlbum.datetime < dateadd(day, 1, '" + dtSecond + "')";
                    }
                }
                else
                {
                    mdlString[3] = " AND TableAlbum.datetime " + " >= '" + dtFirst + "' AND TableAlbum.datetime " + " < dateadd(day, 1, '" + dtFirst + "')";
                }
            }

            string queryAlbumCount = "SET DATEFORMAT dmy;";
            queryAlbumCount += "SELECT count(*)";
            queryAlbumCount += " FROM TableAlbum WHERE TableAlbum.id > -1";
            string queryAlbumId = "SET DATEFORMAT dmy;";
            queryAlbumId += "SELECT id";
            queryAlbumId += " FROM (SELECT TableAlbum.id, row_number() OVER (ORDER BY TableAlbum.id DESC) as rn FROM TableAlbum";
            queryAlbumId += " WHERE TableAlbum.id > -1";

            for (int i = 0; i < mdlString.Length; i++)
            {
                queryAlbumCount += mdlString[i];
                queryAlbumId += mdlString[i];
            }

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.CommandText = queryAlbumCount;
                int mediaCount = (Int32)sqlcomm.ExecuteScalar();
                if (mediaCount > 0)
                {
                    for (int i = 0; i < mediaCount; i++)
                    {
                        Array.Resize(ref outputAlbumId, i + 1);
                        sqlcomm.CommandText = queryAlbumId + ") A WHERE rn BETWEEN " + Convert.ToString(i + 1) + " AND " + Convert.ToString(i + 1);
                        outputAlbumId[i] = (Int32)sqlcomm.ExecuteScalar();
                    }
                }
                else
                    return new int[0];

                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return new int[0];
            }

            return outputAlbumId;
        }

        public int[] tagBasedSearch(string[] inputTagId, int searchType, bool isByState, int stateKey, bool isByDatetime, int byDatetimeType, DateTime fDatetime, DateTime sDatetime)
        {
            int[] outputMediaId = new int[0];
            int[] outputAlbumId = new int[0];

            if (searchType == 1)
            {
                string stateSearchString = "";
                if (stateKey == 1)
                    stateSearchString = " AND TableMedia.state = 1";

                string dtMSearchString = "";
                if (isByDatetime)
                {
                    string dtOperator = "";
                    string dtFirst = "";
                    string dtSecond = "";
                    dtFirst = fDatetime.ToString("dd/MM/yyyy");
                    dtSecond = sDatetime.ToString("dd/MM/yyyy");
                    if (byDatetimeType != 1)
                    {
                        if (byDatetimeType == 2)
                        {
                            dtOperator = "<";
                            dtMSearchString = " AND TableMedia.datetime " + dtOperator + " '" + dtFirst + "'";
                        }
                        else if (byDatetimeType == 3)
                        {
                            dtOperator = ">";
                            dtMSearchString = " AND TableMedia.datetime " + dtOperator + " dateadd(day, 1,  '" + dtFirst + "')";
                        }
                        else
                        {
                            dtMSearchString = " AND TableMedia.datetime >= " + "'" + dtFirst + "' AND TableMedia.datetime < dateadd(day, 1, '" + dtSecond + "')";
                        }
                    }
                    else
                    {
                        dtMSearchString = " AND TableMedia.datetime " + " >= '" + dtFirst + "' AND TableMedia.datetime " + " < dateadd(day, 1, '" + dtFirst + "')";
                    }
                }

                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(false);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();

                    int tempInt = -1;
                    int[] tempOutputMediaId = { -1 };

                    for (int i = 0; i < inputTagId.Length; i++)
                    {
                        sqlcomm.CommandText = "SET DATEFORMAT dmy;SELECT count(*) FROM TableMedia WHERE TableMedia.tag LIKE '%" + inputTagId[i] + "%'" + dtMSearchString + stateSearchString;
                        try
                        {
                            tempInt = (int)sqlcomm.ExecuteScalar();
                        }
                        catch (Exception e)
                        {
                            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                            return new int[0];
                        }

                        if (tempInt > 0)
                        {
                            for (int j = 0; j < tempInt; j++)
                            {
                                sqlcomm.CommandText = "SET DATEFORMAT dmy;SELECT id";
                                sqlcomm.CommandText += " FROM (SELECT TableMedia.id, TableMedia.tag, TableMedia.datetime, row_number() OVER (ORDER BY TableMedia.id DESC) as rn FROM TableMedia";
                                sqlcomm.CommandText += " WHERE TableMedia.tag LIKE '%" + inputTagId[i] + "%'" + dtMSearchString + stateSearchString;
                                sqlcomm.CommandText += ") A WHERE rn BETWEEN " + Convert.ToString(j + 1) + " AND " + Convert.ToString(j + 1);
                                if (j != 0)
                                    Array.Resize(ref tempOutputMediaId, tempOutputMediaId.Length + 1);
                                tempOutputMediaId[j] = (int)sqlcomm.ExecuteScalar();
                            }
                            outputMediaId = outputMediaId.Concat(tempOutputMediaId).ToArray();
                            Array.Resize(ref tempOutputMediaId, 1);
                            tempOutputMediaId[0] = -1;
                        }
                    }
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                    return new int[0];
                }
                outputMediaId = outputMediaId.Distinct().ToArray();
                return outputMediaId;
            }
            else
            {
                string dtASearchString = "";
                if (isByDatetime)
                {
                    string dtOperator = "";
                    string dtFirst = "";
                    string dtSecond = "";
                    dtFirst = fDatetime.ToString("dd/MM/yyyy");
                    dtSecond = sDatetime.ToString("dd/MM/yyyy");
                    if (byDatetimeType != 1)
                    {
                        if (byDatetimeType == 2)
                        {
                            dtOperator = "<";
                            dtASearchString = " AND TableAlbum.datetime " + dtOperator + " '" + dtFirst + "'";
                        }
                        else if (byDatetimeType == 3)
                        {
                            dtOperator = ">";
                            dtASearchString = " AND TableAlbum.datetime " + dtOperator + " dateadd(day, 1,  '" + dtFirst + "')";
                        }
                        else
                        {
                            dtASearchString = " AND TableAlbum.datetime >= " + "'" + dtFirst + "' AND TableAlbum.datetime < dateadd(day, 1, '" + dtSecond + "')";
                        }
                    }
                    else
                    {
                        dtASearchString = " AND TableAlbum.datetime " + " >= '" + dtFirst + "' AND TableAlbum.datetime " + " < dateadd(day, 1, '" + dtFirst + "')";
                    }
                }

                try
                {
                    if (sqlconn.State != ConnectionState.Closed)
                        sqlconn.Close();
                    sqlconn.ConnectionString = csLib.getConnectionString(false);
                    sqlconn.Open();
                    sqlcomm = sqlconn.CreateCommand();

                    int tempInt = -1;
                    int[] tempOutputAlbumId = { -1 };

                    for (int i = 0; i < inputTagId.Length; i++)
                    {
                        sqlcomm.CommandText = "SET DATEFORMAT dmy;SELECT count(*) FROM TableAlbum WHERE TableAlbum.tag LIKE '%" + inputTagId[i] + "%'" + dtASearchString;
                        try
                        {
                            tempInt = (int)sqlcomm.ExecuteScalar();
                        }
                        catch (Exception e)
                        {
                            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                            return new int[0];
                        }

                        if (tempInt > 0)
                        {
                            for (int j = 0; j < tempInt; j++)
                            {
                                sqlcomm.CommandText = "SET DATEFORMAT dmy;SELECT id";
                                sqlcomm.CommandText += " FROM (SELECT TableAlbum.id, TableAlbum.tag, TableAlbum.datetime, row_number() OVER (ORDER BY TableAlbum.id DESC) as rn FROM TableAlbum";
                                sqlcomm.CommandText += " WHERE TableAlbum.tag LIKE '%" + inputTagId[i] + "%'" + dtASearchString;
                                sqlcomm.CommandText += ") A WHERE rn BETWEEN " + Convert.ToString(j + 1) + " AND " + Convert.ToString(j + 1);
                                if (j != 0)
                                    Array.Resize(ref tempOutputAlbumId, tempOutputAlbumId.Length + 1);
                                tempOutputAlbumId[j] = (int)sqlcomm.ExecuteScalar();
                            }
                            outputAlbumId = outputAlbumId.Concat(tempOutputAlbumId).ToArray();
                            Array.Resize(ref tempOutputAlbumId, 1);
                            tempOutputAlbumId[0] = -1;
                        }
                    }
                }
                catch (Exception e)
                {
                    csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                    return new int[0];
                }
                outputAlbumId = outputAlbumId.Distinct().ToArray();
                return outputAlbumId;
            }
        }

        public int getAlbumCount(bool allowDisabled)
        {
            int output = -1;

            string stateQuery = "";
            if (!allowDisabled)
                stateQuery = " AND TableAlbum.state = 1";
            else
                stateQuery = " AND (TableAlbum.state = 1 OR TableAlbum.state = 0)";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.CommandText = "SELECT count(*) FROM TableAlbum WHERE TableAlbum.id > 0" + stateQuery;
                output = (int)sqlcomm.ExecuteScalar();

                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return -1;
            }

            return output;
        }

        public int getMediaCount(bool allowDisabled, int mediaType)
        {
            int output = -1;

            string stateQuery = "";
            if (!allowDisabled)
                stateQuery = " AND TableMedia.state = 1";
            else
                stateQuery = " AND (TableMedia.state = 1 OR TableMedia.state = 0)";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.Parameters.AddWithValue("@Type", mediaType);

                if (mediaType > 0)
                    sqlcomm.CommandText = "SELECT count(*) FROM TableMedia WHERE TableMedia.id > 0 AND TableMedia.type=@Type" + stateQuery;
                else
                    sqlcomm.CommandText = "SELECT count(*) FROM TableMedia WHERE TableMedia.id > 0" + stateQuery;
                output = (int)sqlcomm.ExecuteScalar();

                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return -1;
            }

            return output;
        }

        public int getTagCount(bool allowDisabled)
        {
            int output = -1;

            string stateQuery = "";
            if (!allowDisabled)
                stateQuery = " AND TableTag.state = 1";
            else
                stateQuery = " AND (TableTag.state = 1 OR TableTag.state = 0)";

            try
            {
                if (sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
                sqlconn.ConnectionString = csLib.getConnectionString(false);
                sqlconn.Open();
                sqlcomm = sqlconn.CreateCommand();

                sqlcomm.CommandText = "SELECT count(*) FROM TableTag WHERE TableTag.id IS NOT NULL" + stateQuery;
                output = (int)sqlcomm.ExecuteScalar();

                sqlconn.Close();
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, "");
                return -1;
            }

            return output;
        }
    }
}
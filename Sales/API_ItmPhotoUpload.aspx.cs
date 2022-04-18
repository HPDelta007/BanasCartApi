using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Data;
using System.Collections;
using System.Web.Script.Serialization;
using System.Text;
using System.Data.SqlClient;


public partial class API_ItmPhotoUpload : System.Web.UI.Page
{
    string UserId;
    string ItmCode;
    string descriptions;
    string features;
    string specifications;

    string APIKey;

    HttpPostedFile ItmPhoto;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                descriptions = "";
                features = "";
                specifications = "";

                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);
                ItmCode = (((Request.Form["ItmCode"] != null && Request.Form["ItmCode"] != "")) ? Request.Form["ItmCode"].ToString() : null);
                ItmPhoto = (((Request.Files["ItmPhoto"] != null)) ? Request.Files["ItmPhoto"] : null);
                descriptions = (((Request.Form["descriptions"] != null && Request.Form["descriptions"] != "")) ? Request.Form["descriptions"].ToString() : null);
                features = (((Request.Form["features"] != null && Request.Form["features"] != "")) ? Request.Form["features"].ToString() : null);
                specifications = (((Request.Form["specifications"] != null && Request.Form["specifications"] != "")) ? Request.Form["specifications"].ToString() : null);

                if (descriptions != null)
                    descriptions = descriptions.Replace("'", "`");

                if (features != null)
                    features = features.Replace("'", "`");

                if (specifications != null)
                    specifications = specifications.Replace("'", "`");

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(selectdata());
                }
                else
                {
                    string sw = "";
                    StringBuilder s = new StringBuilder();
                    s.Append("Authentication Key is wrong.");
                    sw = GetReturnValue("209", "Authentication Key is wrong.", s);
                    Response.ContentType = "application/json";
                    Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                } 
            }
            catch (Exception ex)
            {
                string sw = "";
                StringBuilder s = new StringBuilder();
                s.Append(ex.Message);
                sw = GetReturnValue("209", "Data Get Issue", s);
                Response.ContentType = "application/json";
                Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
            }
        }
    }

    public class ResponseData
    {
        public int? status { get; set; }
        public string message { get; set; }
        public ArrayList Result { get; set; }
    }

    public string DataTableToJsonObj(DataTable dt)
    {
        DataSet ds = new DataSet();
        ds.Merge(dt);
        StringBuilder JsonString = new StringBuilder();
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            JsonString.Append("[");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                JsonString.Append("{");
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (j < ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                    }
                }
                if (i == ds.Tables[0].Rows.Count - 1)
                {
                    JsonString.Append("}");
                }
                else
                {
                    JsonString.Append("},");
                }
            }
            JsonString.Append("]");
            return JsonString.ToString();
        }
        else
        {
            return null;
        }
    }

    public string selectdata()
    {
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";

        try
        {
            DataTable dtCount = new DataTable();
            dtCount = _aPI_BLL.returnDataTable(" select * from ItmPhotos where ItmCode = '" + ItmCode + "' ");

            if (ItmPhoto != null && ItmPhoto.FileName != "")
            {
                string[] getExtenstion = ItmPhoto.FileName.Split('.');
                string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                string FileNameForInsert = ItmPhoto.FileName.Replace("." + oExtension, "");

                string filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + "" + ItmCode + "-PRODPHOTO." + oExtension;

                ItmPhoto.SaveAs(filePath);

                if (dtCount.Rows.Count > 0)
                {
                    _aPI_BLL.InsertUpdateNonQuery(" update ItmPhotos set PhotoPath = '" + filePath + "' " +
                                                  " , descriptions= " + ((descriptions == null) ? "NULL" : "'" + descriptions + "'") + "  " +
                                                  " , features= " + ((features == null) ? "NULL" : "'" + features + "'") + "  " +
                                                  " , specifications= " + ((specifications == null) ? "NULL" : "'" + specifications + "'") + "  " +
                                                  " where ItmCode = '" + ItmCode + "' ");
                }
                else
                {
                    _aPI_BLL.InsertUpdateNonQuery(" insert into ItmPhotos (ItmCode, PhotoPath,descriptions,features,specifications) " +
                                                  " values ('" + ItmCode + "', '" + filePath + "' " +
                                                  " , " + ((descriptions == null) ? "NULL" : "'" + descriptions + "'") + " " +
                                                  " , " + ((features == null) ? "NULL" : "'" + features + "'") + " " +
                                                  " , " + ((specifications == null) ? "NULL" : "'" + specifications + "'") + " " +
                                                  " ) ");
                }

                da = _aPI_BLL.returnDataTable(" select * from ItmPhotos where ItmCode = '" + ItmCode + "' ");
            }
            else
            {
                if (dtCount.Rows.Count > 0)
                {
                    _aPI_BLL.InsertUpdateNonQuery(" update ItmPhotos set " + 
                                                  " descriptions= " + ((descriptions == null) ? "NULL" : "'" + descriptions + "'") + "  " +
                                                  " , features= " + ((features == null) ? "NULL" : "'" + features + "'") + "  " +
                                                  " , specifications= " + ((specifications == null) ? "NULL" : "'" + specifications + "'") + "  " +
                                                  " where ItmCode = '" + ItmCode + "' ");
                }
                else
                {
                    _aPI_BLL.InsertUpdateNonQuery(" insert into ItmPhotos (ItmCode,descriptions,features,specifications) " +
                                                  " values ('" + ItmCode + "' " +
                                                  " , " + ((descriptions == null) ? "NULL" : "'" + descriptions + "'") + " " +
                                                  " , " + ((features == null) ? "NULL" : "'" + features + "'") + " " +
                                                  " , " + ((specifications == null) ? "NULL" : "'" + specifications + "'") + " " +
                                                  " ) ");
                }

                da = _aPI_BLL.returnDataTable(" select * from ItmPhotos where ItmCode = '" + ItmCode + "' ");
            }

            st.Append(DataTableToJsonObj(da));

            if (da == null)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (st.ToString() == "[]" || st.ToString() == "")
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (da.Rows.Count > 0)
            {
                ReturnVal = GetReturnValue("200", "Data Saved", st);
            }

            if (st.ToString() != "[]")
                return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
            else
                return ReturnVal.Replace("\\", "").Replace("\"[]\"", "[]");
        }
        catch (Exception ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append(ex.Message);
            ReturnVal = GetReturnValue("209", "Data Get Issue", s);
            return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
        }
    }

    public string GetReturnValue(string Status, string Message, StringBuilder PassStringDataTable)
    {
        var r = new ReturnValue
        {
            status = Status,
            message = Message,
            result = PassStringDataTable.ToString()
        };
        return new JavaScriptSerializer().Serialize(r);
    }

    public string GetNo(DateTime date, SqlCommand pSqlCmd)
    {
        SqlCommand sqlCmd = new SqlCommand();
        SqlDataReader sqlDR;
        GeneralDAL _generalDAL = new GeneralDAL();
        string no;

        //If passed psqlCmd is null means This function is called from UI while data entry, then it need to open connection and return no
        //If Passed pSqlCmd is not null means, this function is called at time of Insert , So it should return no and  increment the count and if 
        //the record of the date is not present it should insert one new record for the date.
        if (pSqlCmd == null)
        {
            _generalDAL.OpenSQLConnection();
            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
        }
        else
        {
            sqlCmd = pSqlCmd;
        }

        //Sql Query to Return no
        string sql = "SELECT 'SO/'+ Convert(varchar,a.Ct+1) +'/" + date.ToString("ddMMyy") + "' AS No" +
                     " FROM SONos a" +
                     " WHERE  a.Dt='" + ((DateTime)date).ToString("dd-MMM-yyyy") + "'";

        sqlCmd.CommandType = CommandType.Text;
        sqlCmd.CommandText = sql;
        sqlDR = sqlCmd.ExecuteReader();

        if (sqlDR.Read())
        {
            no = sqlDR["No"].ToString();
        }
        else
        {
            //If the record is not present in table for the specified date, it should generate new no start from 1 with date
            no = "SO/1/" + date.ToString("ddMMyy");
        }
        sqlDR.Close();
        //Sql Query to Return no

        //Increment Count if it is called from Insert function of DAL means pSqlCmd != null
        if (pSqlCmd != null)
        {
            //sqlCmd.CommandText = " Declare @Count AS int;" +
            //                     " SELECT @Count=COUNT(*)FROM SONos a WHERE  a.Dt='" + date.ToString("dd-MMM-yyyy") + "'" +
            //                     " IF(@Count=0)" +
            //                     "   BEGIN" +
            //                     "       INSERT INTO SONos (SONoId,Dt,Ct) VALUES (NEWID(),'" + date.ToString("dd-MMM-yyyy") + "',1)" +
            //                     "   END" +
            //                     " ELSE" +
            //                     "   BEGIN" +
            //                     "       UPDATE SONos SET Ct=Ct+1 WHERE Dt='" + date.ToString("dd-MMM-yyyy") + "'" +
            //                     "   END";
            //sqlCmd.ExecuteNonQuery();
        }
        else
        {
            //If pSqlCmd is null, means This function is called at UI Entry time, so it should close the connection
            _generalDAL.CloseSQLConnection();
        }

        return no;
    }
}
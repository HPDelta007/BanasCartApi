using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.Configuration;

public partial class API_DeleteIteamFromOrderList : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    string SoLnId;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["SoLnId"] != null && Request.Form["SoLnId"] != "")
                    SoLnId = Request.Form["SoLnId"].ToString();  
                else
                    SoLnId = null;

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {               
                    Response.Write(GetData(SoLnId));
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

    public string DataTableToJSONWithJSONNet(System.Data.DataTable table)
    {
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);
        return JSONString;
    }

    private void priConnect()
    {
        itsDriver = new tungComponents.tungDbDriver.DriverSqlServer(ConfigurationManager.AppSettings["SQLSERVER_SERVER"],
            ConfigurationManager.AppSettings["SQLSERVER_USER"],
            ConfigurationManager.AppSettings["SQLSERVER_PASSWORD"],
            ConfigurationManager.AppSettings["SQLSERVER_DB"]
            );
    }

    private void priDisconnect()
    {
        itsDriver.fClose();
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

    public string GetData(string SoLnId)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        DataTable da1 = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        string where = "";
        string tempResult = null;

        try
        {
            if (SoLnId != null)
            {
                //    if (where != "")
                //    {
                //        where += " AND ";
                //    }
                //    where += " a.SoLnId = '" + SoLnId + "' ";
                //}
                //if (where != "")
                //{
                //    where = " WHERE " + where;
                //}

                priConnect();

                string sql = " Declare @Count AS int " +
                             " SELECT @Count=COUNT(*)FROM SOLns a WHERE  a.SOId in (select s.SOId from SOLns s where s.SOLnId ='" + SoLnId + "' )" +
                             " IF(@Count>1) " +
                             " BEGIN " +
                             " Delete From SOLns WHERE  SOLnId='" + SoLnId + "'" +
                             " END " +
                             " ELSE If(@Count=1) " +
                             " BEGIN " +
                             " Delete From  SOs where SOId in (Select SOId From SOLns where SOLnId = '" + SoLnId + "') " +
                             " Delete From SOLns WHERE  SOLnId='" + SoLnId + "' " +
                             " END ";

                itsDriver.fExecuteNonQuery(sql);

                tempResult = "0";

                if (tempResult == null)
                {
                    da1.Columns.Add("status");
                    da1.Columns.Add("message");
                    da1.Columns.Add("result");
                    da1.Rows.Add("209", "Duplicate Data", tempResult);
                }
                else if (tempResult == "0")
                {
                    da1.Columns.Add("status");
                    da1.Columns.Add("message");
                    da1.Columns.Add("result");
                    da1.Rows.Add("200", "Data Deleted");
                }

                priDisconnect();


            }
            else
            {
                da1.Columns.Add("status");
                da1.Columns.Add("message");
                da1.Columns.Add("result");
                da1.Rows.Add("209", "So Line ID Not Found in API Parameter");
            }

            var s = ClassA.FromDataRow(da1.Rows[0]);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(s);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Cannot insert duplicate key in object"))
            {
                da1.Columns.Add("status");
                da1.Columns.Add("message");
                da1.Columns.Add("result");
                da1.Rows.Add("209", "Data is duplicate", ex.Message);
                priDisconnect();
                var s1 = ClassA.FromDataRow(da1.Rows[0]);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(s1);
            }
            else
            {
                if (da1.Rows.Count == 0)
                {
                    da1.Columns.Add("status");
                    da1.Columns.Add("message");
                    da1.Columns.Add("result");
                    da1.Rows.Add("209", "Data Save Failed", ex.Message);
                    priDisconnect();
                    var s1 = ClassA.FromDataRow(da1.Rows[0]);
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    return jss.Serialize(s1);
                }
                else
                {
                    priDisconnect();
                    var s1 = ClassA.FromDataRow(da1.Rows[0]);
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    return jss.Serialize(s1);
                }
            }
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
}
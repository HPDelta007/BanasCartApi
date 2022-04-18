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
using System.Web.Configuration;


public partial class API_SubGrps : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    string APIKey;
    string CustomerCode;
    string UserId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                if (Request.Form["CustomerCode"] != null && Request.Form["CustomerCode"] != "")
                {
                    CustomerCode = Request.Form["CustomerCode"].ToString();
                }
                else
                {
                    CustomerCode = null;
                }

                if (Request.Form["UserId"] != null && Request.Form["UserId"] != "")
                {
                    UserId = Request.Form["UserId"].ToString();
                }
                else
                {
                    UserId = null;
                }

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                //if (ConfigAPIKey == APIKey)
                //{
                Response.Write(GetData());
                //}
                //else
                //{
                //    string sw = "";
                //    StringBuilder s = new StringBuilder();
                //    s.Append("Authentication Key is wrong.");
                //    sw = GetReturnValue("209", "Authentication Key is wrong.", s);
                //    Response.ContentType = "application/json";
                //    Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                //}
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

    public string DataTableToJSONWithJSONNet(DataTable table)
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

    public string GetData()
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        string where = "";
        priConnect();
        try
        {
            //if (CustomerCode != null)
            //{
            //    if (where != "")
            //    {
            //        where += " AND ";
            //    }
            //    where += " CustomerCode = '" + CustomerCode + "'";
            //}
            //else
            //{
            //    if (where != "")
            //    {
            //        where += " AND ";
            //    }
            //    where += " CustomerCode in (select DistributorDealerId from UserRights where UserId = '" + UserId + "') ";
            //}

            if (where != "")
            {
                where = " and " + where;
            }

            int tempResult1 = 0;

            //tempResult1 = itsDriver.fSelectAndFillDataSet(" SELECT Distinct SubGrpId,ProductGrp as [Name] FROM vwDispatchSummaryRpt " + where +
            //              " ORDER BY Name ASC", Dt);

            tempResult1 = itsDriver.fSelectAndFillDataSet(" SELECT Distinct SubGrpId, Name as [Name]"+
            " , replace(PhotoPath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') as ImageNameShow " +
            "FROM SubGrps where SubGrpId not in ('EE5D2A29-BEC8-43CD-A97D-9DD4C0B9E3DB') " + where +
                          " ORDER BY Name ASC", Dt);

            //da = _aPI_BLL.returnDataTable(" select ItmId, Name, MRP " +
            //                            " , replace(ImageName, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') as ImageNameShow " +
            //                            " from Itms  Where  SubGrpId = '" + SubGrpId.ToString() + "' order by Name ");

            st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (tempResult1 == 0)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (st.ToString() == "[]")
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (Dt.Tables[0].Rows.Count > 0)
            {
                ReturnVal = GetReturnValue("200", "Data Get", st);
            }
            else
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            priDisconnect();
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
            priDisconnect();
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
}
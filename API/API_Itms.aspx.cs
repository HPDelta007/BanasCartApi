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

public partial class API_Itms : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    string DivisionID;
    string SubGrpID;
    string HP;
    string Stage;
    string Length;
    string Thickness;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["DivisionID"] != null && Request.Form["DivisionID"] != "")
                {
                    DivisionID = Request.Form["DivisionID"].ToString();
                }
                else
                {
                    DivisionID = null;
                }

                if (Request.Form["SubGrpID"] != null && Request.Form["SubGrpID"] != "")
                {
                    SubGrpID = Request.Form["SubGrpID"].ToString();
                }
                else
                {
                    SubGrpID = null;
                }

                if (Request.Form["HP"] != null && Request.Form["HP"] != "")
                {
                    HP = Request.Form["HP"].ToString();
                }
                else
                {
                    HP = null;
                }

                if (Request.Form["Stage"] != null && Request.Form["Stage"] != "")
                {
                    Stage = Request.Form["Stage"].ToString();
                }
                else
                {
                    Stage = null;
                }

                if (Request.Form["Length"] != null && Request.Form["Length"] != "")
                {
                    Length = Request.Form["Length"].ToString();
                }
                else
                {
                    Length = null;
                }

                if (Request.Form["Thickness"] != null && Request.Form["Thickness"] != "")
                {
                    Thickness = Request.Form["Thickness"].ToString();
                }
                else
                {
                    Thickness = null;
                }

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(GetData(DivisionID, SubGrpID, HP, Stage, Length, Thickness));
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

    public string GetData(string DivisionID, string SubGrpID, string HP, string Stage, string Length, string Thickness)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        string where = "";
        priConnect();
        try
        {

            if (DivisionID != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " DivTextListId ='" + DivisionID + "'";
            }

            if (SubGrpID != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " SubGrpId ='" + SubGrpID + "'";
            }

            if (HP != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " HP ='" + HP + "'";
            }

            if (Stage != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " Stage ='" + Stage + "'";
            }

            if (Length != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " convert(decimal, length) = " + Convert.ToDecimal(Length).ToString("0.##") + " ";
            }

            if (Thickness != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " Thikness ='" + Thickness + "'";
            }

            if (where != "")
            {
                where = " WHERE " + where;
            }

            int tempResult1 = 0;

            tempResult1 = itsDriver.fSelectAndFillDataSet(" select ItmId, replace(Name, '\"', '''''') as Name, HP, Stage, Thikness, Length from " + WebConfigurationManager.AppSettings["DBName"] + "..Itms " + where + " ", Dt);

            st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (Dt.Tables[0].Rows.Count == 0)
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
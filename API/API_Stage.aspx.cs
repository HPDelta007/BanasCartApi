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


public partial class API_Stage : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;
    string ProductGroupId;
    string HP;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["ProductGroupId"] != null && Request.Form["ProductGroupId"] != "")
                {
                    ProductGroupId = Request.Form["ProductGroupId"].ToString();
                }
                else
                {
                    ProductGroupId = null;
                }

                if (Request.Form["HP"] != null && Request.Form["HP"] != "")
                {
                    HP = Request.Form["HP"].ToString();
                }
                else
                {
                    HP = null;
                }

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(GetData(ProductGroupId, HP));
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

    public string GetData(string ProductGroupId,string HP)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        priConnect();
        try
        {
            int tempResult1 = 0;


            if(ProductGroupId.ToString().ToUpper() == "A73EC4D1-1BED-4A23-BF9C-576FA2447BF0")
            {
            tempResult1 = itsDriver.fSelectAndFillDataSet(" select distinct Stage from  " + WebConfigurationManager.AppSettings["DBName"] + "..Itms " +
                                " where Stage is not null and Stage <> '' and Stage <> 'null' AND SubGrpId= '" + ProductGroupId + "'and HP = '" + HP + "'" +
                                " Order by Stage", Dt);
            }
            else
            {
                tempResult1 = itsDriver.fSelectAndFillDataSet(" select distinct Stage from  " + WebConfigurationManager.AppSettings["DBName"] + "..Itms " +
                                    " where Stage is not null and Stage <> '' and Stage <> 'null' AND SubGrpId= '" + ProductGroupId + "'" +
                                    " Order by Stage", Dt);
            }


            st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (tempResult1 == 0) //Dt == null
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
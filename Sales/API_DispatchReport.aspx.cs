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

public partial class API_DispatchReport : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    DateTime? FromDate;
    DateTime? ToDate;
    string No;
    string CustomerCode;
    string UserId;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["FromDate"] != null && Request.Form["FromDate"] != "")
                {
                    FromDate = Convert.ToDateTime(Request.Form["FromDate"]);
                }
                else
                {
                    FromDate = null;
                }

                if (Request.Form["ToDate"] != null && Request.Form["ToDate"] != "")
                {
                    ToDate = Convert.ToDateTime(Request.Form["ToDate"]);
                }
                else
                {
                    ToDate = null;
                }

                if (Request.Form["No"] != null && Request.Form["No"] != "")
                {
                    No = Request.Form["No"].ToString();
                }
                else
                {
                    No = null;
                }

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

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(GetData());
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
        string[] Specialchars = new string[] { "\\", "/", "\"", "\n", "\r", "\t", "\x08", "\x0c" };
        string[] ReplacedChar = new string[] { "\\\\", "\\/", "\\\"", "\\n", "\\r", "\\t", "\\f", "\\b" };

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
                        string replaceText;

                        replaceText = ds.Tables[0].Rows[i][j].ToString();

                        for (int h = 0; h < Specialchars.Length; h++)
                        {
                            if (replaceText.Contains(Specialchars[h]))
                            {
                                replaceText = replaceText.Replace(Specialchars[h], ReplacedChar[h]);
                            }
                        }

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        string replaceText;

                        replaceText = ds.Tables[0].Rows[i][j].ToString();

                        for (int h = 0; h < Specialchars.Length; h++)
                        {
                            if (replaceText.Contains(Specialchars[h]))
                            {
                                replaceText = replaceText.Replace(Specialchars[h], ReplacedChar[h]);
                            }
                        }

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\"");
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
        DataTable DtMain = new DataTable();
        StringBuilder st = new StringBuilder();
        
        string ReturnVal = "";
        string where = "";
        priConnect();
        try
        {
            //FromDt
            if (FromDate != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.Dt >='" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "'";
            }

            //ToDt
            if (ToDate != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.Dt <='" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "'";
            }

            //No
            if (No != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.No Like '%" + No + "%'";
            }

            //CustomerCode
            if (CustomerCode != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.CustomerCode = '" + CustomerCode + "'";
            }
            else
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.CustomerCode in (select DistributorDealerId from UserRights where UserId = '" + UserId + "') ";
            }

            if (where != "")
            {
                where = " WHERE " + where;
            }
            
            int tempResult1 = 0;

            tempResult1 = itsDriver.fSelectAndFillDataTable(" Select a.* " +
                                                            " ,RptPath = '" + ConfigurationSettings.AppSettings["RptPath"] + "&RptId=' + Convert(varchar(36),a.DeliNoteId) + '&ReportPrintType=Defult&Division=HO&FromApp=1'" +
                                                            " from vwDispatchRpt a " + 
                                                            " inner join Debtors d on d.Code = a.CustomerCode " + 
                                                            where + " order by a.InsertedOn desc ", DtMain);

            st.Append(DataTableToJsonObj(DtMain));

            if (st.ToString().Length > 10)
            {
                ReturnVal = GetReturnValue("200", "Data Get.", st);
            }
            else if (Dt == null)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }
            else if (st.ToString() == "[]" || st.ToString() == "" || st.ToString() == "[")
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
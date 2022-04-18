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

public partial class API_PendingOrderList : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    DateTime? FromDate;
    DateTime? ToDate;
    string No;
    string ItmCode;
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

                if (Request.Form["ItmCode"] != null && Request.Form["ItmCode"] != "")
                {
                    ItmCode = Request.Form["ItmCode"].ToString();
                }
                else
                {
                    ItmCode = null;
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
            //FromDt
            if (FromDate != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " s.SODt >='" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "'";
            }

            //ToDt
            if (ToDate != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " s.SODt <='" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "'";
            }

            //No
            if (No != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " s.SONo Like '%" + No + "%'";
            }

            //CustomerCode
            if (CustomerCode != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " s.CustomerCode = '" + CustomerCode + "'";
            }

            if (ItmCode != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " sl.ItmCode ='" + ItmCode + "' ";
            }

            if (where != "")
            {
                where += " AND ";
            }
            where += " isnull((sl.Qty - dl.DispatchQty), sl.Qty) > 0 ";

            if (where != "")
            {
                where = " WHERE " + where;
            }

            int tempResult1 = 0;

            tempResult1 = itsDriver.fSelectAndFillDataSet(" select s.SOId, s.SODt, s.SONo, replace(d.Name, '\"', 'U+0022') as 'CustomerName', sl.ItmCode, replace(m.Name, '\"', 'U+0022') as 'ItmName', " +
                                                          " sl.Qty as 'OrderQty', sl.ExpectedDeliDt, isnull((sl.Qty - dl.DispatchQty), sl.Qty) as 'PendingQty', dl.DispatchQty, sl.SOLnId " +
                                                          " ,u.FirstName + ' '+ u.LastName  + case when  s.LastUpdatedByUserId is null then '' else 'Update:' + u.FirstName + ' '+ u.LastName end as UserName " +
                                                          " from SOs s " +
                                                          " inner join SOLns sl on sl.SOId = s.SOId " +
                                                          " inner join Master1 m on m.Code = sl.ItmCode and MasterType = 6 " +
                                                          " inner join Debtors d on d.Code = s.CustomerCode " +
                                                          " inner join Users u on s.InsertedByUserId = u.UserId left join  Users uu on s.LastUpdatedByUserId = uu.UserId " + 
                                                          " left join (select sum(dlx.Qty) as 'DispatchQty', slx.SOLnId from DistributorDispatchLns dlx  inner join SOLns slx on slx.SOLnId = dlx.SOLnId group by slx.SOLnId) dl on dl.SOLnId = sl.SOLnId " +
                                                          " " + where + " order by s.InsertedOn desc ", Dt);

            st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (Dt == null)
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
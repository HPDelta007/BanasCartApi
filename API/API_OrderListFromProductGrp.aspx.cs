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

public partial class API_OrderListFromProductGrp : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    DateTime? FromDate;
    DateTime? ToDate;
    string No;
    string CustomerCode;
    string ItmCode;

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

                if (Request.Form["ItmCode"] != null && Request.Form["ItmCode"] != "")
                {
                    ItmCode = Request.Form["ItmCode"].ToString();
                }
                else
                {
                    ItmCode = null;
                }

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(GetData(FromDate, ToDate, No, CustomerCode, ItmCode));
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

    private static string replaceSplChar(string Data)
    {
        Data = Data.Replace("%", "%25");
        Data = Data.Replace("<", "%3C");
        Data = Data.Replace(">", "%3E");
        Data = Data.Replace("&", "%26");
        Data = Data.Replace("+", "%2B");
        Data = Data.Replace("#", "%23");
        Data = Data.Replace("*", "%2A");
        Data = Data.Replace("!", "%21");
        Data = Data.Replace(",", "%2C");
        Data = Data.Replace("'", "%27");
        Data = Data.Replace("=", "%3D");
        Data = Data.Replace("â‚¬", "%E2%82%AC");
        Data = Data.Replace("?", "%3F");
        Data = Data.Replace("$", "%24");
        Data = Data.Replace("[", "");
        Data = Data.Replace("]", "");
        Data = Data.Replace("\"", "`");

        return Data;
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
                        string replaceText;

                        replaceText = replaceSplChar(ds.Tables[0].Rows[i][j].ToString());

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        string replaceText;

                        replaceText = replaceSplChar(ds.Tables[0].Rows[i][j].ToString());

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

    public string GetData(DateTime? FromDt, DateTime? ToDt, string No, string CustomerCode, string ItmCode)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        string where = "";
        string where1 = " ksl.SOLnId is null ";
        priConnect();
        try
        {
            //FromDt
            if (FromDt != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.SODt >='" + Convert.ToDateTime(FromDt).ToString("dd-MMM-yyyy") + "'";
            }

            //ToDt
            if (ToDt != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.SODt <='" + Convert.ToDateTime(ToDt).ToString("dd-MMM-yyyy") + "'";
            }

            //No
            if (No != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.SONo Like '%" + No + "%'";
            }

            if (CustomerCode != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " a.CustomerCode = '" + CustomerCode + "'";
            }

            if (ItmCode != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " sl.ItmCode = '" + ItmCode + "'";
            }

            if (where != "")
            {
                where = " WHERE " + where;
            }
            //FromDt
            if (FromDt != null)
            {
                if (where1 != "")
                {
                    where1 += " AND ";
                }
                where1 += " a.Dt >='" + Convert.ToDateTime(FromDt).ToString("dd-MMM-yyyy") + "'";
            }

            //ToDt
            if (ToDt != null)
            {
                if (where1 != "")
                {
                    where1 += " AND ";
                }
                where1 += " a.Dt <='" + Convert.ToDateTime(ToDt).ToString("dd-MMM-yyyy") + "'";
            }

            //No
            if (No != null)
            {
                if (where1 != "")
                {
                    where1 += " AND ";
                }
                where1 += " a.No Like '%" + No + "%'";
            }

            if (CustomerCode != null)
            {
                if (where1 != "")
                {
                    where1 += " AND ";
                }
                where1 += " l.Autotrno = '" + CustomerCode + "'";
            }

            if (ItmCode != null)
            {
                if (where1 != "")
                {
                    where1 += " AND ";
                }
                where1 += " i.BusyCode = '" + ItmCode + "'";
            }

            if (where1 != "")
            {
                where1 = " WHERE " + where1;
            }

            int tempResult1 = 0;

            tempResult1 = itsDriver.fSelectAndFillDataSet(" SELECT a.SOId,a.CustomerCode as VendorLgrId, Replace(Convert(varchar(12),a.SODt ,106),' ','-') as Dt , a.SONo as No, a.Remarks as Notes " + 
                                 " ,Replace(Convert(varchar(12),a.InsertedOn ,106),' ','-') as InsertedOn " +
                                 " , Replace(Convert(varchar(12),a.LastUpdatedOn ,106),' ','-') as LastUpdatedOn " +
                                 " , CASE WHEN ISNULL(s.ApprovedDisapproved,'O') = 'D' THEN 'ORDER REJECTED' WHEN ISNULL(s.ApprovedDisapproved,'O') <> 'O' THEN 'ORDER CONFIRMED' ELSE 'ORDER BOOKED' END AS ApprovedDisapproved " +
                                 " ,ISNULL(b.Alias + '-','') + replace(b.Name, '\"', 'U+0022') AS VendorLgrName, a.ApprovedDisapprovedOn, a.LastUpdatedOn " +
                                 " ,SUM(sl.Qty) as TotalOrderQty,SUM(ISNULL(dsl.DisQty,0)) as TotalDispQty " +
                                 " FROM SOs a " +
                                 " left JOIN SOLns sl On sl.SOId = a.SOId " +
                                 " left JOIN Debtors b On a.CustomerCode = b.Code" +
                                 " left join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOLns ksl on ksl.SOLnId = sl.SOLnId " +
                                 " left join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOs s on ksl.SOId = s.SOId " +
                                 " left join (Select a.SOLnId,SUM(ISNULL(QtyPriUM,0)) as DisQty,MAX(c.Dt) as LastDispDt " +
                                 " from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteSOLns a " +
                                 " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteLns b on b.DeliNoteLnId = a.DeliNoteLnId " +
                                 " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNotes c on c.DeliNoteId = b.DeliNoteId " +
                                 " group by a.SOLnId,c.IsLRReceived) dsl on dsl.SOLnId = ksl.SOLnId " +
                                 where +
                                 " group by a.SOId,a.CustomerCode,a.SODt, a.SONo, a.Remarks,a.InsertedOn,a.LastUpdatedOn,s.ApprovedDisapproved,b.Alias,b.Name, a.ApprovedDisapprovedOn, a.LastUpdatedOn  " +
                                 " UNION " +
                                 " SELECT a.SOId,b.Code as VendorLgrId, Replace(Convert(varchar(12),a.Dt ,106),' ','-') as Dt , a.No as No, a.Notes as Notes,Replace(Convert(varchar(12),a.InsertedOn ,106),' ','-') as InsertedO " +
                                 " , Replace(Convert(varchar(12),a.LastUpdatedOn ,106),' ','-') as LastUpdatedOn " +
                                 " , CASE WHEN ISNULL(a.ApprovedDisapproved,'O') = 'D' THEN 'ORDER REJECTED' WHEN ISNULL(a.ApprovedDisapproved,'O') <> 'O' THEN 'ORDER CONFIRMED' ELSE 'ORDER BOOKED' END AS ApprovedDisapproved " +
                                 " ,ISNULL(b.Alias + '-','') + replace(b.Name, '\"', 'U+0022') AS VendorLgrName, a.ApprovedDisapprovedOn, a.LastUpdatedOn " +
                                 " ,SUM(sl.QtyPriUM) as TotalOrderQty,SUM(ISNULL(dsl.DisQty,0)) as TotalDispQty " +
                                 " FROM " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOs a " +
                                 " left JOIN " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOLns sl On sl.SOId = a.SOId " +
                                 " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..Lgrs l On l.LgrId = a.VendorLgrId " +
                                 " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..Itms i On i.ItmId = sl.ItmId " +
                                 " left JOIN Debtors b On l.Autotrno = b.Code" +
                                 " left join SOLns ksl on ksl.SOLnId = sl.SOLnId " +
                                 " left join (Select a.SOLnId,SUM(ISNULL(QtyPriUM,0)) as DisQty,MAX(c.Dt) as LastDispDt " +
                                 " ,(ISNULL(c.IsLRReceived,0)) as IsLRReceived,MAX(c.LRRecDt) as LRRecDt,MAX(ISNULL(c.TransporterName,'')) as TransporterName,MAX(c.LRNo) as LRNo " +
                                 " from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteSOLns a " +
                                 " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteLns b on b.DeliNoteLnId = a.DeliNoteLnId " +
                                 " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNotes c on c.DeliNoteId = b.DeliNoteId " +
                                 " group by a.SOLnId,c.IsLRReceived) dsl on dsl.SOLnId = sl.SOLnId " +
                                 where1 +
                                 " group by a.SOId,b.Code,a.Dt, a.No, a.Notes,a.InsertedOn ,a.ApprovedDisapproved,b.Alias,b.Name, a.ApprovedDisapprovedOn, a.LastUpdatedOn   " +
                                 " ORDER BY a.LastUpdatedOn desc", Dt);

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
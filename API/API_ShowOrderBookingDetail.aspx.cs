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

public partial class API_ShowOrderBookingDetail : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    string SOId;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["SOId"] != null && Request.Form["SOId"] != "")
                {
                    SOId = (Request.Form["SOId"]).ToString();
                }
                else
                {
                    SOId = null;
                }

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(GetData(SOId));
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

    public string GetData(string SOId)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        string where = "";
        string where1 = "";
        priConnect();
        try
        {
            if (SOId != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " where s.SOId ='" + SOId + "'";
            }

            if (SOId != null)
            {
                where1 += " AND s.SOId ='" + SOId + "'";
            }

            int tempResult1 = 0;

            tempResult1 = itsDriver.fSelectAndFillDataSet("select s.SONo as No, s.SODt as Date, sl.Qty as BookedQty, ISNULL(dsl.DisQty,0) as DispatchQty, ksl.QtyPriUM - ISNULL(ksl.QtyPriUMShortClose,0) as ConfirmedQty " +
                                                          " , CASE WHEN s.ApprovedDisapproved = 'D' THEN 'ORDER REJECTED' " + 
                                                          " WHEN ksl.SOLnId is not null THEN (CASE WHEN ISNULL((ksl.QtyPriUM - ISNULL(ksl.QtyPriUMShortClose,0) - ISNULL(dsl.DisQty,0)),0) = 0 and ISNULL(dsl.IsLRReceived,0) = 1 " + 
                                                          " THEN 'LR RECEIVED' + ISNULL(' Dtd. ' + FORMAT(LRRecDt,'dd-MMM-yyyy'),'') + ISNULL(' LR No. : ' + dsl.LRNo,'') + ISNULL(' Transporter Name : ' + dsl.TransporterName,'') " + 
                                                          " WHEN ISNULL((ksl.QtyPriUM - ISNULL(dsl.DisQty,0)),0) = 0 then 'ORDER FULLY DISPATCHED [TOTAL DISPATCHED QTY. : ' + FORMAT(ISNULL(dsl.DisQty,0),'0') + ']' " + 
                                                          " WHEN ISNULL((ksl.QtyPriUM - ISNULL(ksl.QtyPriUMShortClose,0) - ISNULL(dsl.DisQty,0)),0) < (ksl.QtyPriUM - ISNULL(ksl.QtyPriUMShortClose,0)) then 'PARTLLY DISPATCHED [DISP. QTY : ' + FORMAT(ISNULL(dsl.DisQty,0),'0') + ' PENDING QTY : ' + FORMAT(ISNULL((ksl.QtyPriUM - ISNULL(ksl.QtyPriUMShortClose,0) - ISNULL(dsl.DisQty,0)),0),'0') + ' ]' " + 
                                                          " WHEN ksl.QtyPriUM - ISNULL(ksl.QtyPriUMShortClose,0) = 0 then 'ORDER CANCELLED [CANCELLED QTY : ' +  FORMAT(ISNULL(ksl.QtyPriUMShortClose,0),'0') + ']' else 'ORDER CONFIRMED' end) ELSE 'ORDER BOOKED' END AS ApprovedDisapproved " +
                                                          " ,dsl.LastDispDt as LastDispDt, sl.ExpectedDeliDt as DiliveryDate, sl.Qty as Quantity, " +
                                                          "  sl.LnNo, sl.SOLnId, s.SOId, ISNULL(d.Alias + '-','') + replace(d.Name, '\"', 'U+0022') as 'CustomerName', m.Code as PriceListSeriesId, " +
                                                          "  ISNULL(m.Alias + '-','') + replace(m.Name, '\"', 'U+0022') as 'PriceListSeriesName' " +
                                                          "  ,ISNULL(ksl.QtyPriUMShortClose,0) as CancelledQty " +
                                                          " from SOs s " +
                                                          " inner join SOLns sl on sl.SOId = s.SOId" +
                                                          " inner join Debtors d on d.Code = s.CustomerCode" +
                                                          " inner join Master1 m on m.Code =  sl.ItmCode and m.MasterType = '6'" +
                                                          " left join Master1 m1 on m1.Code =  m.ParentGrp" +
                                                          " left join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOLns ksl on ksl.SOLnId = sl.SOLnId " +
                                                          " left join (Select a.SOLnId,SUM(ISNULL(QtyPriUM,0)) as DisQty,MAX(c.Dt) as LastDispDt " +
                                                          " ,(ISNULL(c.IsLRReceived,0)) as IsLRReceived,MAX(c.LRRecDt) as LRRecDt,MAX(ISNULL(c.TransporterName,'')) as TransporterName,MAX(c.LRNo) as LRNo " +
                                                          " from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteSOLns a " +
                                                          " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteLns b on b.DeliNoteLnId = a.DeliNoteLnId " +
                                                          " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNotes c on c.DeliNoteId = b.DeliNoteId " +
                                                          " group by a.SOLnId,ISNULL(c.IsLRReceived,0)) dsl on dsl.SOLnId = ksl.SOLnId " +
                                                          where +
                                                          " UNION " +
                                                          " select s.No as No, s.Dt as Date, sl.QtyPriUM as BookedQty, ISNULL(dsl.DisQty,0) as DispatchQty, sl.QtyPriUM - ISNULL(sl.QtyPriUMShortClose,0) as ConfirmedQty " +
                                                          " ,CASE WHEN s.ApprovedDisapproved = 'D' THEN 'ORDER REJECTED' " + 
                                                          " WHEN ISNULL((sl.QtyPriUM - ISNULL(sl.QtyPriUMShortClose,0) - ISNULL(dsl.DisQty,0)),0) = 0 and ISNULL(dsl.IsLRReceived,0) = 1 " + 
                                                          " THEN 'LR RECEIVED' + ISNULL(' Dtd. ' + FORMAT(LRRecDt,'dd-MMM-yyyy'),'') + ISNULL(' LR No. : ' + dsl.LRNo,'') + ISNULL(' Transporter Name : ' + dsl.TransporterName,'') " + 
                                                          " WHEN ISNULL((sl.QtyPriUM - ISNULL(dsl.DisQty,0)),0) = 0 then 'ORDER FULLY DISPATCHED [TOTAL DISPATCHED QTY. : ' + FORMAT(ISNULL(dsl.DisQty,0),'0') + ']' " + 
                                                          " WHEN ISNULL((sl.QtyPriUM - ISNULL(sl.QtyPriUMShortClose,0) - ISNULL(dsl.DisQty,0)),0) < (sl.QtyPriUM - ISNULL(sl.QtyPriUMShortClose,0)) then 'PARTLLY DISPATCHED [DISP. QTY : ' + FORMAT(ISNULL(dsl.DisQty,0),'0') + ' PENDING QTY : ' + FORMAT(ISNULL((sl.QtyPriUM - ISNULL(sl.QtyPriUMShortClose,0) - ISNULL(dsl.DisQty,0)),0),'0') + ' ]' " + 
                                                          " WHEN sl.QtyPriUM - ISNULL(sl.QtyPriUMShortClose,0) = 0 then 'ORDER CANCELLED [CANCELLED QTY : ' +  FORMAT(ISNULL(sl.QtyPriUMShortClose,0),'0') + ']' else 'ORDER CONFIRMED' END AS ApprovedDisapproved " +
                                                          " ,dsl.LastDispDt as LastDispDt, sl.DueDt as DiliveryDate, sl.QtyPriUM as Quantity, " +
                                                          "  sl.LnNo, sl.SOLnId, s.SOId, ISNULL(d.Alias + '-','') + replace(d.Name, '\', 'U+0022') as 'CustomerName', m.Code as PriceListSeriesId, " +
                                                          "  ISNULL(m.Alias + '-','') + replace(m.Name, '\', 'U+0022') as 'PriceListSeriesName'  " +
                                                          "  ,ISNULL(sl.QtyPriUMShortClose,0) as CancelledQty " +
                                                          "  from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOs s " +
                                                          "  inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..SOLns sl on sl.SOId = s.SOId " +
                                                          " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..Lgrs l On l.LgrId = s.VendorLgrId " +
                                                          "  inner join Debtors d on l.AutotrNo = d.Code " +
                                                          "  inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..Itms i on i.ItmId = sl.ItmId " +
                                                          "  inner join Master1 m on m.Code =  i.BusyCode and m.MasterType = '6' " +
                                                          "  left join Master1 m1 on m1.Code =  m.ParentGrp " +
                                                          "  left join SOLns ksl on ksl.SOLnId = sl.SOLnId " +
                                                          "  left join (Select a.SOLnId,SUM(ISNULL(QtyPriUM,0)) as DisQty,MAX(c.Dt) as LastDispDt " +
                                                          "  ,(ISNULL(c.IsLRReceived,0)) as IsLRReceived,MAX(c.LRRecDt) as LRRecDt,MAX(ISNULL(c.TransporterName,'')) as TransporterName,MAX(c.LRNo) as LRNo " +
                                                          "  from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteSOLns a " +
                                                          "  inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteLns b on b.DeliNoteLnId = a.DeliNoteLnId " +
                                                          "  inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNotes c on c.DeliNoteId = b.DeliNoteId " +
                                                          "  group by a.SOLnId,ISNULL(c.IsLRReceived,0)) dsl on dsl.SOLnId = sl.SOLnId " +
                                                          "  where ksl.SOLnId is null " + where1 +
                                                          " order by sl.LnNo ", Dt);



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
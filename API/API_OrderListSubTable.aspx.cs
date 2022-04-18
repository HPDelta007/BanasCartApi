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

public partial class API_OrderListSubTable : System.Web.UI.Page
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
        DataTable DtMain = new DataTable();
        StringBuilder st = new StringBuilder();
        st.Append("[");
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
            else
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " s.CustomerCode in (select DistributorDealerId from UserRights where UserId = '" + UserId + "') ";
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
                where = " WHERE " + where;
            }
            else
            {
                where = " Where 1 = 1 ";
            }

            int tempResult1 = 0;

            //tempResult1 = itsDriver.fSelectAndFillDataTable(" select s.SOId, s.SODt, s.SONo, replace(d.Name, '\"', 'U+0022') as 'CustomerName', replace(m.Name, '\"', 'U+0022') as 'ItmName', sl.Qty as 'OrderQty', sl.ExpectedDeliDt, sl.Qty as 'PendingQty' " +
            //                                              " ,u.FirstName + ' '+ u.LastName  + case when  s.LastUpdatedByUserId is null then '' else 'Update:' + u.FirstName + ' '+ u.LastName end as UserName,sl.SOLnID from SOs s " +
            //                                              " inner join SOLns sl on sl.SOId = s.SOId " +
            //                                              " inner join Master1 m on m.Code = sl.ItmCode and MasterType = 6 " +
            //                                              " inner join Debtors d on d.Code = s.CustomerCode " +
            //                                              " inner join Users u on s.InsertedByUserId = u.UserId left join  Users uu on s.LastUpdatedByUserId = uu.UserId " + where + " order by s.InsertedOn desc ", DtMain);
            tempResult1 = itsDriver.fSelectAndFillDataTable(" select s.SOId, s.SODt, s.SONo, replace(d.Name, '\"', 'U+0022') as 'CustomerName' " +
                                                         " ,u.FirstName + ' '+ u.LastName  + case when  s.LastUpdatedByUserId is null then '' else 'Update:' + u.FirstName + ' '+ u.LastName end as UserName " +
                                                         " , (select Max(sl.ExpectedDeliDt) from SoLns sl Where sl.SoId= s.SoId ) as ExpectedDeliDt from SOs s " +
                                                         " inner join Debtors d on d.Code = s.CustomerCode " +
                                                         " inner join Users u on s.InsertedByUserId = u.UserId left join  Users uu on s.LastUpdatedByUserId = uu.UserId " + where + " order by s.InsertedOn desc ", DtMain);

            //st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (DtMain.Rows.Count > 0)
            {
                for (int i = 0; i <= DtMain.Rows.Count - 1; i++)
                {
                    DataTable Dt1 = new DataTable();


                    itsDriver.fSelectAndFillDataTable(" select s.SOId, s.SODt, s.SONo, replace(d.Name, '\"', 'U+0022') as 'CustomerName', replace(m.Name, '\"', 'U+0022') as 'ItmName', sl.Qty as 'OrderQty', sl.ExpectedDeliDt, sl.Qty as 'PendingQty' " +
                                                      " ,u.FirstName + ' '+ u.LastName  + case when  s.LastUpdatedByUserId is null then '' else 'Update:' + u.FirstName + ' '+ u.LastName end as UserName,sl.SOLnID,sl.Remarks from SOs s " +
                                                      " inner join SOLns sl on sl.SOId = s.SOId " +
                                                      " inner join Master1 m on m.Code = sl.ItmCode and MasterType = 6 " +
                                                      " inner join Debtors d on d.Code = s.CustomerCode " +
                                                      " inner join Users u on s.InsertedByUserId = u.UserId left join  Users uu on s.LastUpdatedByUserId = uu.UserId " + where + " and s.SOid  ='" + DtMain.Rows[i]["SOId"].ToString() + "'  order by ItmName", Dt1);
                    

                    string SubData = null;

                    for (int inner = 0; inner <= Dt1.Rows.Count - 1; inner++)
                    {
                        SubData += " {" +
                              "    \"SOId\": \"" + ((Dt1.Rows[inner]["SOId"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["SOId"].ToString() + "") + "\"," +
                              "    \"SOLnID\": \"" + ((Dt1.Rows[inner]["SOLnID"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["SOLnID"].ToString() + "") + "\"," +
                              "    \"ItmName\": \"" + ((Dt1.Rows[inner]["ItmName"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["ItmName"].ToString() + "") + "\"," +
                              "    \"ExpectedDeliDt\": \"" + ((Dt1.Rows[inner]["ExpectedDeliDt"] == DBNull.Value) ? "NULL" : "" +  Convert.ToDateTime(Dt1.Rows[inner]["ExpectedDeliDt"]).ToString("dd-MM-yy") + "") + "\"," +
                              "    \"OrderQty\": \"" + ((Dt1.Rows[inner]["OrderQty"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["OrderQty"].ToString() + "") + "\"," +
                              "    \"PendingQty\": \"" + ((Dt1.Rows[inner]["PendingQty"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["PendingQty"].ToString() + "") + "\"," +
                              "    \"Remarks\": \"" + ((Dt1.Rows[inner]["Remarks"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["Remarks"].ToString() + "") + "\"" +
                              "  }, ";
                    }
                    if (SubData != null && SubData.ToString().Contains(","))
                    {
                        SubData = SubData.Remove(SubData.LastIndexOf(","));
                        SubData = "[" + SubData + "]";
                    }
                    else
                    {
                        SubData = "[]";
                    }
                    if (Dt1.Rows.Count > 0)
                    {
                        st.Append("{" +
                            "    \"SOId\": \"" + ((DtMain.Rows[i]["SOId"].ToString() == "" || DtMain.Rows[i]["SOId"].ToString() == null) ? "NULL" : "" + DtMain.Rows[i]["SOId"].ToString() + "") + "\"," +
                            "    \"SODt\": \"" + ((DtMain.Rows[i]["SODt"] == DBNull.Value) ? "NULL" : "" + Convert.ToDateTime(DtMain.Rows[i]["SODt"].ToString()).ToString("dd-MM-yy") + "") + "\"," +
                            "    \"SONo\": \"" + ((DtMain.Rows[i]["SONo"] == DBNull.Value) ? "NULL" : "" + DtMain.Rows[i]["SONo"].ToString() + "") + "\"," +
                            "    \"CustomerName\": \"" + ((DtMain.Rows[i]["CustomerName"] == DBNull.Value) ? "NULL" : "" + DtMain.Rows[i]["CustomerName"].ToString() + "") + "\"," +
                            "    \"UserName\": \"" + ((DtMain.Rows[i]["UserName"] == DBNull.Value) ? "NULL" : "" + DtMain.Rows[i]["UserName"].ToString() + "") + "\"," +
                            "    \"ExpectedDeliDt\": \"" + ((DtMain.Rows[i]["ExpectedDeliDt"] == DBNull.Value) ? "NULL" : "" + Convert.ToDateTime(DtMain.Rows[i]["ExpectedDeliDt"]).ToString("dd-MM-yy") + "") + "\"," +
                            "    \"Itmcount\":  \"" + (Dt1.Rows.Count) + "\"," +
                            "    \"ItmDetails\": " + ((SubData == null) ? "" : SubData) + "" +
                            "},");
                    }
                }
            }
            if (st != null && st.ToString().Contains(","))
            {
                st = st.Remove(st.Length - 1, 1);
                st.Append("]");
            }
            //st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (st.ToString() == "[")
            {
                st = new StringBuilder();
                st.Append("[]");
            }


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
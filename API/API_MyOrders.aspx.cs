using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Collections;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Script.Serialization;

public partial class API_API_MyOrders : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    DateTime? FromDate;
    DateTime? ToDate;
    string Name;
    string ItmId;
    string UserId;
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

                if (Request.Form["Name"] != null && Request.Form["Name"] != "")
                {
                    Name = Request.Form["Name"].ToString();
                }
                else
                {
                    Name = null;
                }

                if (Request.Form["ItmId"] != null && Request.Form["ItmId"] != "")
                {
                    ItmId = Request.Form["ItmId"].ToString();
                }
                else
                {
                    ItmId = null;
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

                Response.Write(GetData());

            }
            catch(Exception ex)
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
            if (Name != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " i.Name Like '%" + Name + "%'";
            }

            if (ItmId != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " sl.ItmId ='" + ItmId + "' ";
            }
            if (UserId != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " u.UserId ='" + UserId + "' "; 
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

            tempResult1 = itsDriver.fSelectAndFillDataTable(" select s.SOId, s.SODt, s.SONo" +
                                                         " ,u.FirstName + ' '+ u.LastName  + case when  s.LastUpdatedByUserId is null then '' else 'Update:' + u.FirstName + ' '+ u.LastName end as UserName " +
                                                         " , (select Max(sl.ExpectedDeliDt) from OnlineSOLns sl Where sl.SoId= s.SoId ) as ExpectedDeliDt from OnlineSos s " +
                                                          " inner join OnlineSOLns sl on sl.SOId = s.SOId " +
                                                         " inner join Itms i on i.ItmId = sl.ItmId" +
                                                         " inner join Users u on s.InsertedByUserId = u.UserId left join  Users uu on s.LastUpdatedByUserId = uu.UserId " + where + " order by s.InsertedOn desc ", DtMain);

            //st.Append(DataTableToJsonObj(Dt.Tables[0]));

            if (DtMain.Rows.Count > 0)
            {
                for (int i = 0; i <= DtMain.Rows.Count - 1; i++)
                {
                    DataTable Dt1 = new DataTable();


                    itsDriver.fSelectAndFillDataTable(" select s.SOId, s.SODt, s.SONo, replace(i.Name, '\"', 'U+0022') as 'ItmName',replace(i.ImageName,'"+ ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') as ImageNameShow" +
                                                    ",i.ItmId as itmid, sl.Qty as 'OrderQty',sl.Amount as amount, s.InsertedOn as ExpectedDeliDt, sl.Qty as 'PendingQty',ua.Country as country,ua.State as state,ua.City as city,ua.StreetAddress as saddress,ua.PinCode as pin,ua.MobileNo as MN,ua.FullName as FN,ua.AlternateMobileNumber as AMN" +
                                                      " ,u.FirstName + ' '+ u.LastName  + case when  s.LastUpdatedByUserId is null then '' else 'Update:' + u.FirstName + ' '+ u.LastName end as UserName,sl.SOLnID,sl.Remarks from OnlineSos s " +
                                                      " inner join OnlineSOLns sl on sl.SOId = s.SOId " +
                                                      " inner join UserAddresses ua on ua.UserAddressId = s.UserAddressId" +
                                                      " inner join Itms i on i.ItmId = sl.ItmId"+
                                                      " inner join Users u on s.InsertedByUserId = u.UserId left join  Users uu on s.LastUpdatedByUserId = uu.UserId " + where + " and s.SOid  ='" + DtMain.Rows[i]["SOId"].ToString() + "'", Dt1);


                    string SubData = null;

                    string UserAddress = null;

                    for (int inner = 0; inner <= Dt1.Rows.Count - 1; inner++)
                    {
                        SubData += " {" +
                              "    \"SOId\": \"" + ((Dt1.Rows[inner]["SOId"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["SOId"].ToString() + "") + "\"," +
                              "    \"SOLnID\": \"" + ((Dt1.Rows[inner]["SOLnID"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["SOLnID"].ToString() + "") + "\"," +
                              "    \"ItmId\": \"" + ((Dt1.Rows[inner]["itmid"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["itmid"].ToString() + "") + "\"," +
                              "    \"ItmName\": \"" + ((Dt1.Rows[inner]["ItmName"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["ItmName"].ToString() + "") + "\"," +
                               "    \"ItmImageName\": \"" + ((Dt1.Rows[inner]["ImageNameShow"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["ImageNameShow"].ToString() + "") + "\"," +
                              "    \"ExpectedDeliDt\": \"" + ((Dt1.Rows[inner]["ExpectedDeliDt"] == DBNull.Value) ? "NULL" : "" + Convert.ToDateTime(Dt1.Rows[inner]["ExpectedDeliDt"]).ToString("dd-MM-yy") + "") + "\"," +
                              "    \"OrderQty\": \"" + ((Dt1.Rows[inner]["OrderQty"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["OrderQty"].ToString() + "") + "\"," +
                              "    \"PendingQty\": \"" + ((Dt1.Rows[inner]["PendingQty"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["PendingQty"].ToString() + "") + "\"," +
                              "    \"Amount\": \"" + ((Dt1.Rows[inner]["amount"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["amount"].ToString() + "") + "\"," +
                              "    \"Remarks\": \"" + ((Dt1.Rows[inner]["Remarks"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["Remarks"].ToString() + "") + "\"" +
                              "  }, ";
                        UserAddress += " {" +
                            "    \"Country\": \"" + ((Dt1.Rows[inner]["country"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["country"].ToString() + "") + "\"," +
                             "    \"State\": \"" + ((Dt1.Rows[inner]["state"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["state"].ToString() + "") + "\"," +
                             "    \"City\": \"" + ((Dt1.Rows[inner]["city"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["city"].ToString() + "") + "\"," +
                             "    \"StreetAddress\": \"" + ((Dt1.Rows[inner]["saddress"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["saddress"].ToString() + "") + "\"," +
                             "    \"PinCode\": \"" + ((Dt1.Rows[inner]["pin"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["pin"].ToString() + "") + "\"," +
                             "    \"MobileNo\": \"" + ((Dt1.Rows[inner]["MN"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["MN"].ToString() + "") + "\"," +
                             "    \"FullName\": \"" + ((Dt1.Rows[inner]["FN"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["FN"].ToString() + "") + "\"," +
                             "    \"AlternateMobileNumber\": \"" + ((Dt1.Rows[inner]["AMN"] == DBNull.Value) ? "NULL" : "" + Dt1.Rows[inner]["AMN"].ToString() + "") + "\"" +
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
                    if (UserAddress != null && UserAddress.ToString().Contains(","))
                    {
                        UserAddress = UserAddress.Remove(UserAddress.LastIndexOf(","));
                        UserAddress = "[" + UserAddress + "]";
                    }
                    else
                    {
                        UserAddress = "[]";
                        }
                    if (Dt1.Rows.Count > 0)
                    {
                        st.Append("{" +
                            "    \"SOId\": \"" + ((DtMain.Rows[i]["SOId"].ToString() == "" || DtMain.Rows[i]["SOId"].ToString() == null) ? "NULL" : "" + DtMain.Rows[i]["SOId"].ToString() + "") + "\"," +
                            "    \"SODt\": \"" + ((DtMain.Rows[i]["SODt"] == DBNull.Value) ? "NULL" : "" + Convert.ToDateTime(DtMain.Rows[i]["SODt"].ToString()).ToString("dd-MM-yy") + "") + "\"," +
                            "    \"SONo\": \"" + ((DtMain.Rows[i]["SONo"] == DBNull.Value) ? "NULL" : "" + DtMain.Rows[i]["SONo"].ToString() + "") + "\"," +
                            "    \"UserName\": \"" + ((DtMain.Rows[i]["UserName"] == DBNull.Value) ? "NULL" : "" + DtMain.Rows[i]["UserName"].ToString() + "") + "\"," +
                            "    \"ExpectedDeliDt\": \"" + ((DtMain.Rows[i]["ExpectedDeliDt"] == DBNull.Value) ? "NULL" : "" + Convert.ToDateTime(DtMain.Rows[i]["ExpectedDeliDt"]).ToString("dd-MM-yy") + "") + "\"," +
                            "    \"Itmcount\":  \"" + (Dt1.Rows.Count) + "\"," +
                            "    \"ItmDetails\": " + ((SubData == null) ? "" : SubData) + "," +
                            "    \"UserAddress\": " + ((UserAddress == null) ? "" : UserAddress) + "" +
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
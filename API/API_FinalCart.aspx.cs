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

public partial class API_FinalCart : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    DateTime? DueDt;
    string UserId;
    string UserAddressId;
    DateTime? Date;
    //string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["DueDt"] != null && Request.Form["DueDt"] != "")
                {
                    DueDt = Convert.ToDateTime(Request.Form["DueDt"]);
                }
                else
                {
                    DueDt = null;
                }

                //if (Request.Form["LgrId"] != null && Request.Form["LgrId"] != "")
                //{
                //    LgrId = Request.Form["LgrId"].ToString();
                //}
                //else
                //{
                //    LgrId = null;
                //}

                if (Request.Form["UserId"] != null && Request.Form["UserId"] != "")
                {
                    UserId = Request.Form["UserId"].ToString();
                }
                else
                {
                    UserId = null;
                }
                if (Request.Form["UserAddressId"] != null && Request.Form["UserAddressId"] != "")
                {
                    UserAddressId = Request.Form["UserAddressId"].ToString();
                }
                else
                {
                    UserAddressId = null;
                }
                //if (Request.Form["voucherTypeId"] != null && Request.Form["voucherTypeId"] != "")
                //{
                //    voucherTypeId = Request.Form["voucherTypeId"].ToString();
                //}
                //else
                //{
                //    voucherTypeId = null;
                //}

                if (Request.Form["Date"] != null && Request.Form["Date"] != "")
                {
                    Date = Convert.ToDateTime(Request.Form["Date"]);
                }
                else
                {
                    Date = null;
                }

                //if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                //    APIKey = Request.Form["APIKey"].ToString();
                //else
                //    APIKey = null;

                Response.ContentType = "application/json";

                //string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                //if (ConfigAPIKey == APIKey)
                //{
                    Response.Write(GetData(DueDt,UserId,Date,UserAddressId));
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

    public string GetData(DateTime? DueDt, string UserId, DateTime? date,string UserAddressId)
    {
        DataSet Dt = new DataSet();
        DataTable dt = new DataTable();
        DataTable da = new DataTable();
        DataTable da1 = new DataTable();

        StringBuilder st = new StringBuilder();
        ResponseData _ResponseData = new ResponseData();
        string tempResult = null;
        int j = 1;
        string SOId = "";
        string SOLnId = "";

        string PSOId = "";
        string PSOLnId = "";
        int Pk = 1;
        bool pass = false;

        date = DateTime.Today;

        priConnect();
        try
        {
            //itsDriver.fSelectAndFillDataTable(" select a.* from " + WebConfigurationManager.AppSettings["DBName"] + "..Cart a " +
            //                                  " inner join " + WebConfigurationManager.AppSettings["DBName"] + "..PriceListSeries b on b.PriceListSeriesId = a.ItmId " +
            //                                  " inner join " + WebConfigurationManager.AppSettings["DBName"] + "..PumpSeries c on c.PumpSeriesId = b.PumpPipeSeriesId " +
            //                                  " where a.InsertedByUserId = '" + UserId + "'", dt);


            itsDriver.fSelectAndFillDataTable(" select * from MyCarts a where a.UserId = '" + UserId + "'", dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //if (dt.Rows[i]["DivTextListId"].ToString().ToUpper() == "A35A6A86-7448-4607-86D0-CAD3B1DDB0BA") //For Pump
                    //{
                    if (j == 1)
                    {
                        //string sql = " SELECT TOP 1 b.Prfx + CASE WHEN b.Ct >= 0 THEN  Convert(VARCHAR,b.Ct+1) ELSE Convert(VARCHAR,b.StartFromCt) END + b.Sufx AS No" +
                        //             " FROM VoucherTypes a" +
                        //             " INNER JOIN VoucherNos b ON a.VoucherTypeId = b.VoucherTypeId" +
                        //             " WHERE a.VoucherTypeId='" + "F2546D98-0453-49F8-8DB4-5C613D69B514" + "'" +
                        //             " AND b.WEFDt<='" + Convert.ToDateTime(date).ToString("dd-MMM-yyyy") + "'" +
                        //             " ORDER BY b.WEFDt DESC";

                        string sql = " SELECT TOP 1 b.Prfx + CASE WHEN b.Ct >= 0 THEN  Convert(VARCHAR,b.Ct+1) ELSE Convert(VARCHAR,b.StartFromCt) END + b.Sufx AS No" +
                                    " FROM VoucherTypes a" +
                                    " INNER JOIN VoucherNos b ON a.VoucherTypeId = b.VoucherTypeId" +
                                    " where b.WEFDt<='" + Convert.ToDateTime(date).ToString("dd-MMM-yyyy") + "'" +
                                    " ORDER BY b.WEFDt DESC";

                        string PumpNo = itsDriver.fExecuteScalar(sql).ToString();

                        //string sql11 =  " UPDATE VoucherNos " +
                        //                " SET Ct= CASE WHEN Ct > 0 THEN Ct+1 ELSE StartFromCt+1 END" +
                        //                " WHERE VoucherNoId =" +
                        //                " (SELECT TOP 1 VoucherNoId" +
                        //                " FROM VoucherTypes a" +
                        //                " INNER JOIN VoucherNos b ON a.VoucherTypeId = b.VoucherTypeId" +
                        //                " WHERE a.VoucherTypeId='" + "F2546D98-0453-49F8-8DB4-5C613D69B514" + "'" +
                        //                " AND b.WEFDt<='" + Convert.ToDateTime(date).ToString("dd-MMM-yyyy") + "'" +
                        //                " ORDER BY b.WEFDt DESC)";

                        string sql11 = " UPDATE VoucherNos " +
                                        " SET Ct= CASE WHEN Ct > 0 THEN Ct+1 ELSE StartFromCt+1 END" +
                                        " WHERE VoucherNoId =" +
                                        " (SELECT TOP 1 VoucherNoId" +
                                        " FROM VoucherTypes a" +
                                        " INNER JOIN VoucherNos b ON a.VoucherTypeId = b.VoucherTypeId" +
                                        " where b.WEFDt<='" + Convert.ToDateTime(date).ToString("dd-MMM-yyyy") + "'" +
                                        " ORDER BY b.WEFDt DESC)";

                        itsDriver.fExecuteNonQuery(sql11);

                        string q = " DECLARE @SOId uniqueidentifier;" +
                                    " SET @SOId = NEWID() " +
                                    " INSERT INTO OnlineSOs (SOId,SODt,SONo, UserId, InsertedOn, LastUpdatedOn" +
                                    " ,InsertedByUserId,LastUpdatedByUserId,ApprovedDisapproved,ApprovedDisapprovedOn," +
                                    " UserAddressId,Total,Discount,GrandTotal" +
                                    " )" +
                                    " VALUES (@SOId,'" + ((DateTime)System.DateTime.Now).ToString("dd-MMM-yyyy") + "','" + PumpNo + "'," +
                                    " '" + UserId + "',GETDATE(),GETDATE(),'" + UserId + "'," +
                                    " NULL,0,GETDATE(),(select top 1 UserAddressId from UserAddresses where UserAddressId = '" + UserAddressId + "'), " +
                                    " (select Sum(A.Amount) from MyCarts A where UserId = '" + UserId + "'), " +
                                    " NULL, " +
                                    " NULL);" +
                                    " SELECT @SOId";

                        SOId = itsDriver.fExecuteScalar(q + ";SELECT @SOId;").ToString();
                    }

                    if (SOId != null && SOId != "")
                    {
                        string q1 = " DECLARE  @SOLnId uniqueidentifier;" +
                                    " SET @SOLnId = NewId()" +
                                    " INSERT INTO OnlineSOLns (SOLnId,SOId,LnNo,Qty,ExpectedDeliDt,Remarks,InsertedOn,LastUpdatedOn,InsertedByUserId,LastUpdatedByUserId,ItmId,Rate,Amount" +
                                    " )" +
                                    " VALUES (@SOLnId" +
                                    " ,'" + SOId + "'," + j + ",'" + dt.Rows[i]["Qty"].ToString() + "'" +
                                    " ,'" + Convert.ToDateTime(DateTime.Today).ToString("dd-MMM-yyyy") + "',NULL,GETDATE(),GETDATE(),'" + UserId + "',NULL" +
                                    ",(select top 1 ItmId from Itms where ItmId = '" + dt.Rows[i]["ItmId"].ToString() + "'),'" + dt.Rows[i]["Rate"] + "' ,'" + Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["Qty"]) * Convert.ToDecimal(dt.Rows[i]["Rate"])) + "'" +
                                    ");" +
                                    " SELECT @SOLnId";
                        
                        tempResult = itsDriver.fExecuteScalar(q1 + ";SELECT @SOLnId;").ToString();
                        j++;
                    }
                }
            }

            dt.Rows.Clear();

            j = 1;

            itsDriver.fSelectAndFillDataTable(" select * from MyCarts a where a.UserId = '" + UserId + "'", dt);
            if (tempResult == null)
            {
                da1.Columns.Add("status");
                da1.Columns.Add("message");
                da1.Columns.Add("Id");
                da1.Rows.Add("209", "Duplicate Data", SOId);
            }
            else if (SOId != "0" && SOId != null)
            {
                da1.Columns.Add("status");
                da1.Columns.Add("message");
                da1.Columns.Add("Id");
                da1.Rows.Add("200", "Data Saved", SOId);

                dt.Rows.Clear();
                itsDriver.fSelectAndFillDataTable(" select a.* from MyCarts a " +
                                                  " where a.UserId = '" + UserId + "'", dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //string sql2 = " Insert Into " + WebConfigurationManager.AppSettings["DBName"] + "..CartHistory(CartId, ItmId, ItmName, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId, Qty, DivTextListId)" +
                    //            " Select CartId, ItmId, ItmName, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId, Qty, DivTextListId" +
                    //            " FROM MyCarts where ItmId = '" + dt.Rows[i]["ItmId"].ToString() + "' and InsertedByUserId = '" + UserId + "'";
                    //itsDriver.fExecuteNonQuery(sql2);

                    string sql2 = "delete from MyCarts where ItmId = '" + dt.Rows[i]["ItmId"].ToString() + "' and UserId ='" + UserId + "' ";
                    itsDriver.fExecuteNonQuery(sql2);
                }
            }

            priDisconnect();
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
                da1.Columns.Add("Id");
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
                    da1.Columns.Add("Id");
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
    public class ClassA
    {
        public string status { get; set; }
        public string message { get; set; }
        public ClassB result { get; set; }

        public static ClassA FromDataRow(DataRow row)
        {
            DataTable Dt = new DataTable();
            ClassB result1 = new ClassB();
            if (row["Id"] != DBNull.Value)
            {
                result1.Id = row["Id"].ToString();
                //result1.Id = Guid.Parse(row["Id"].ToString()).ToString();
            }
            var classA = new ClassA
            {
                status = (string)row["status"],
                message = (string)row["message"],
                result = result1
            };
            return classA;
        }
    }

    public class ClassB
    {
        public string Id { get; set; }
    }
}
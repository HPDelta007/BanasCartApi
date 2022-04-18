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

public partial class API_SaveCart : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;
    string ItmName;
    string ItmId;
    string Qty;
    string UserId;
    string CartId;
    string LgrId;
    DateTime? DiliveryDate;
    string Cable;
    string NoOFCable;
    string Voltage;
    string Phase;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["ItmId"] != null && Request.Form["ItmId"] != "")
                {
                    ItmId = Request.Form["ItmId"].ToString();
                }
                else
                {
                    ItmId = null;
                }

                if (Request.Form["ItmName"] != null && Request.Form["ItmName"] != "")
                {
                    ItmName = Request.Form["ItmName"].ToString();
                }
                else
                {
                    ItmName = null;
                }

                if (Request.Form["Qty"] != null && Request.Form["Qty"] != "")
                {
                    Qty = Request.Form["Qty"].ToString();
                }
                else
                {
                    Qty = null;
                }

                if (Request.Form["UserId"] != null && Request.Form["UserId"] != "")
                {
                    UserId = Request.Form["UserId"].ToString();
                }
                else
                {
                    UserId = null;
                }
                if (Request.Form["LgrId"] != null && Request.Form["LgrId"] != "")
                {
                    LgrId = Request.Form["LgrId"].ToString();
                }
                else
                {
                    LgrId = null;
                }
                if (Request.Form["DiliveryDate"] != null && Request.Form["DiliveryDate"] != "")
                {
                    DiliveryDate = Convert.ToDateTime(Request.Form["DiliveryDate"]);
                }
                else
                {
                    DiliveryDate = null;
                }
                if (Request.Form["Cable"] != null && Request.Form["Cable"] != "")
                {
                    Cable = Request.Form["Cable"].ToString();
                }
                else
                {
                    Cable = null;
                }
                if (Request.Form["NoOFCable"] != null && Request.Form["NoOFCable"] != "")
                {
                    NoOFCable = Request.Form["NoOFCable"].ToString();
                }
                else
                {
                    NoOFCable = null;
                }
                if (Request.Form["Voltage"] != null && Request.Form["Voltage"] != "")
                {
                    Voltage = Request.Form["Voltage"].ToString();
                }
                else
                {
                    Voltage = null;
                }
                if (Request.Form["Phase"] != null && Request.Form["Phase"] != "")
                {
                    Phase = Request.Form["Phase"].ToString();
                }
                else
                {
                    Phase = null;
                }
                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                //if (ConfigAPIKey == APIKey)
                //{
                    Response.Write(GetData(ItmId, ItmName, Qty, UserId, LgrId, DiliveryDate, Cable, NoOFCable, Voltage, Phase));
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

    public string GetData(string ItmId, string ItmName, string Qty, string UserId,String LgrId,DateTime? DiliveryDate ,string Cable,string NoOfCable,string Voltage,string Phase)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        DataTable da1 = new DataTable();
        StringBuilder st = new StringBuilder();
        ResponseData _ResponseData = new ResponseData();
        string tempResult = null;
        priConnect();
        try
        {
            itsDriver.fSelectAndFillDataTable(" select * from " + WebConfigurationManager.AppSettings["DBName"] + "..Cart where ItmId = '" + ItmId + "' and  NoOfCable = '" + NoOfCable + "'  and  Voltage = '" + Voltage + "'  and  Phase = '" + Phase + "' and  InsertedByUserId = '" + UserId + "'", da);

            if (da.Rows.Count > 0)
            {
                decimal TotalQty = 0;

                if (da.Rows[0]["Qty"] != DBNull.Value)
                    TotalQty = Convert.ToDecimal(Qty) + Convert.ToDecimal(da.Rows[0]["Qty"]);
                else
                    TotalQty = Convert.ToDecimal(Qty);

                string q1 = " Update " + WebConfigurationManager.AppSettings["DBName"] + "..Cart set LastUpdatedOn = getdate(), " +
                                     " LastUpdatedByUserId = '" + UserId + "', " +
                                     " Qty = " + TotalQty + " where CartId = '" + da.Rows[0]["CartId"].ToString() + "' ";
               
                itsDriver.fExecuteNonQuery(q1);
                tempResult = da.Rows[0]["CartId"].ToString();
            }
            else
            {
                string q2 = " DECLARE  @CartId uniqueidentifier; SET @CartId = NewId() " +
                             "INSERT INTO " + WebConfigurationManager.AppSettings["DBName"] + "..Cart (CartId, ItmId, ItmName, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId, Qty, DivTextListId,LgrId,DiliveryDate,Cable,NoOfCable,Voltage,Phase) " +
                              " VALUES (@CartId," + ((ItmId == null) ? "NULL" : "'" + ItmId.ToString() + "'") + "," +
                              ((ItmName == null) ? "NULL" : "'" + ItmName.ToString().Replace("'", "''") + "'") + "," +
                              "GETDATE(),GETDATE(),'" + UserId + "','" + UserId + "'" + ", " + Qty + ", " +
                              " (select DivTextListId from " + WebConfigurationManager.AppSettings["DBName"] + "..Itms where ItmId = '" + ItmId.ToString() + "') " + "," +
                              ((LgrId == null) ? "NULL" : "'" + LgrId.ToString() + "'") + "," +
                              ((DiliveryDate == null) ? "NULL" : "'" + Convert.ToDateTime(DiliveryDate).ToString("dd-MMM-yyyy") + "'") + "," +
                              ((Cable == null) ? "NULL" : "'" + Cable.ToString().Replace("'", "''") + "'") + "," +
                              ((NoOfCable == null) ? "NULL" : "'" + NoOfCable.ToString().Replace("'", "''") + "'") + "," +
                              ((Voltage == null) ? "NULL" : "'" + Voltage.ToString().Replace("'", "''") + "'") + "," +
                              ((Phase == null) ? "NULL" : "'" + Phase.ToString().Replace("'", "''") + "'") + "" +
                              ")";
                tempResult = itsDriver.fExecuteScalar(q2 + ";SELECT @CartId;").ToString();
            }

            if (tempResult == null)
            {
                da1.Columns.Add("status");
                da1.Columns.Add("message");
                da1.Columns.Add("CartId");
                da1.Rows.Add("209", "Duplicate Data", tempResult);
            }
            else if (tempResult != "0" && tempResult != null)
            {
                da1.Columns.Add("status");
                da1.Columns.Add("message");
                da1.Columns.Add("CartId");
                da1.Rows.Add("200", "Data Saved", tempResult);
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
                da1.Columns.Add("CartId");
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
                    da1.Columns.Add("CartId");
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
            if (row["CartId"] != DBNull.Value)
            {
                result1.CartId = row["CartId"].ToString();
               // result1.CartId = Guid.Parse(row["CartId"].ToString()).ToString();
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
        public string CartId { get; set; }
    }
}
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
using System.Net;
using System.IO;

public partial class API_API_NotificationSendFor : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;
    string Type;
    string Value;
    string UserId;
    string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["Type"] != null && Request.Form["Type"] != "")
                    Type = Request.Form["Type"].ToString();
                else
                    Type = null;

                if (Request.Form["Value"] != null && Request.Form["Value"] != "")
                    Value = Request.Form["Value"].ToString();
                else
                    Value = null;

                if (Request.Form["UserId"] != null && Request.Form["UserId"] != "")
                    UserId = Request.Form["UserId"].ToString();
                else
                    UserId = null;

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";
                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(selectdata());
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

    public string selectdata()
    {       
        DataTable dt = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = ""; priConnect();
        int tempResult = 0;
        try
        {
            if(Type != null)
            {
                if(Type == "Order")
                {
                     string sql11 = " update Users set " +
                                       "  IsSendForOrder = " + ((Value == null) ? "NULL" : "'" + Convert.ToBoolean(Value) + "'") + " " +
                                       " , LastUpdatedOn = GETDATE()" +
                                       " , LastUpdatedByUserId = '" + UserId + "' " +
                                       " where UserId = '" + UserId.ToString() + "' ";

                    itsDriver.fExecuteNonQuery(sql11);
                    tempResult = itsDriver.fSelectAndFillDataTable("Select UserId,IsSendForOrder,IsSendForInvoice,IsSendForLrDetails,IsSendForPromotional from Users where UserId = '" + UserId + "'",dt);
                }
               else if (Type == "Invoice")
                {
                     string sql11 = " update Users set " +
                                       "  IsSendForInvoice =  " + ((Value == null) ? "NULL" : "'" + Convert.ToBoolean(Value) + "'") + " " +
                                       " , LastUpdatedOn = GETDATE()" +
                                       " , LastUpdatedByUserId = '" + UserId + "' " +
                                       " where UserId = '" + UserId.ToString() + "' ";

                    itsDriver.fExecuteNonQuery(sql11);
                    tempResult = itsDriver.fSelectAndFillDataTable("Select UserId,IsSendForOrder,IsSendForInvoice,IsSendForLrDetails,IsSendForPromotional from Users where UserId = '" + UserId + "'", dt);
                }
                else if (Type == "LRDetails")
                {
                     string sql11 = " update Users set " +
                                       "  IsSendForLrDetails = " + ((Value == null) ? "NULL" : "'" + Convert.ToBoolean(Value) + "'") + " " +
                                       " , LastUpdatedOn = GETDATE()" +
                                       " , LastUpdatedByUserId = '" + UserId + "' " +
                                       " where UserId = '" + UserId.ToString() + "' ";

                    itsDriver.fExecuteNonQuery(sql11);
                    tempResult = itsDriver.fSelectAndFillDataTable("Select UserId,IsSendForOrder,IsSendForInvoice,IsSendForLrDetails,IsSendForPromotional from Users where UserId = '" + UserId + "'", dt);
                }
                else if (Type == "Promotional")
                {
                     string sql11 = " update Users set " +
                                       "  IsSendForPromotional = " + ((Value == null) ? "NULL" : "'" + Convert.ToBoolean(Value) + "'") + " " +
                                       " , LastUpdatedOn = GETDATE()" +
                                       " , LastUpdatedByUserId = '" + UserId + "' " +
                                       " where UserId = '" + UserId.ToString() + "' ";

                    itsDriver.fExecuteNonQuery(sql11);
                    tempResult = itsDriver.fSelectAndFillDataTable("Select UserId,IsSendForOrder,IsSendForInvoice,IsSendForLrDetails,IsSendForPromotional  from Users where UserId = '" + UserId + "'", dt);
                }
                else if(Type == "Get")
                {
                    tempResult = itsDriver.fSelectAndFillDataTable("Select UserId,IsSendForOrder,IsSendForInvoice,IsSendForLrDetails,IsSendForPromotional  from Users where UserId = '" + UserId + "'", dt);
                }
            }

            st.Append(DataTableToJsonObj(dt));

            if (dt == null)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (st.ToString() == "[]" || st.ToString() == "")
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (dt.Rows.Count > 0)
            {
                ReturnVal = GetReturnValue("200", "Data Saved", st);
            }
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
            return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
        }
    }


    private static string replaceSplChar(string SMSstring)
    {
        SMSstring = SMSstring.Replace("%", "%25");
        SMSstring = SMSstring.Replace("<", "%3C");
        SMSstring = SMSstring.Replace(">", "%3E");
        SMSstring = SMSstring.Replace("&", "%26");
        SMSstring = SMSstring.Replace("+", "%2B");
        SMSstring = SMSstring.Replace("#", "%23");
        SMSstring = SMSstring.Replace("*", "%2A");
        SMSstring = SMSstring.Replace("!", "%21");
        SMSstring = SMSstring.Replace(",", "%2C");
        SMSstring = SMSstring.Replace("'", "%27");
        SMSstring = SMSstring.Replace("=", "%3D");
        SMSstring = SMSstring.Replace("â‚¬", "%E2%82%AC");
        SMSstring = SMSstring.Replace("?", "%3F");
        SMSstring = SMSstring.Replace(" ", "+");
        SMSstring = SMSstring.Replace("$", "%24");
        return SMSstring;
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

    protected string GenerateOTP()
    {
        string numbers = "1234567890";

        string characters = numbers;

        characters += numbers;


        string otp = string.Empty;

        for (int i = 0; i < 4; i++)
        {
            string character = string.Empty;
            do
            {
                int index = new Random().Next(0, characters.Length);
                character = characters.ToCharArray()[index].ToString();
            } while (otp.IndexOf(character) != -1);
            otp += character;
        }

        return otp;
    }
}
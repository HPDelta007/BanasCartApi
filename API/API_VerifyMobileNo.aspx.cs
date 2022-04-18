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


public partial class API_VerifyMobileNo : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;
    string MobileNo;
    string UserType;
    string Key;
    string SendOTP;
    string APIKey;

    //API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["MobileNo"] != null && Request.Form["MobileNo"] != "")
                    MobileNo = Request.Form["MobileNo"].ToString();
                else
                    MobileNo = null;

                if (Request.Form["Key"] != null && Request.Form["Key"] != "")
                    Key = Request.Form["Key"].ToString();
                else
                    Key = null;

                if (Request.Form["SendOTP"] != null && Request.Form["SendOTP"] != "")
                    SendOTP = Request.Form["SendOTP"].ToString();
                else
                    SendOTP = "YES";

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

    public class ResponseData
    {
        public int? status { get; set; }
        public string message { get; set; }
        public ArrayList Result { get; set; }
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

        DataTable da = new DataTable();
        DataTable dt = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        priConnect();
        try
        {
            int tempResult1 = 0;
            tempResult1 = itsDriver.fSelectAndFillDataTable("select * from Users where MobileNo = '" + MobileNo.ToString() + "' " +
                                          " and UserTypeTextListId in (select TextListId from TextLists where [Group] = 'UserType' ) " +  //and [Text]  ='" + UserType.ToString() + "'
                                          " and isnull(IsDisabled, 0) = 0 ", dt);
            if (dt.Rows.Count > 0)
            {
                string OTP = GenerateOTP();

                string sql11 = " update Users set OTP = '" + OTP.ToString() + "'  where UserId = '" + dt.Rows[0]["UserId"].ToString() + "' ";

                itsDriver.fExecuteNonQuery(sql11);

                int tempResult0 = 0;
                tempResult0 = itsDriver.fSelectAndFillDataTable(" select u.OTP,t.Text as UserType from Users u inner join TextLists t on t.TextListId = u.UserTypeTextListId  where u.MobileNo = '" + MobileNo.ToString() + "' " +
                                              " and isnull(u.IsDisabled, 0) = 0 ", da);

                st.Append(DataTableToJsonObj(da));

                if (SendOTP == "YES")
                {
                    string Message = "<#> " + OTP + " for Krishna Distribution House dealer Application Login.- " + Key + "";

                    DataTable dtGeneralSettings = new DataTable();

                    itsDriver.fSelectAndFillDataTable(" select * from Facets where FacetName = 'OTPSMSString' ", dtGeneralSettings);

                    string strUrl = dtGeneralSettings.Rows[0]["FacetText"].ToString();

                    Message = replaceSplChar(Message);

                    strUrl = strUrl.Replace("[MOBILENO]", MobileNo);
                    strUrl = strUrl.Replace("[TEXT]", Message);
                    WebRequest request = HttpWebRequest.Create(strUrl);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)response.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    response.Close();
                    s.Close();
                    readStream.Close();
                }
            }
            else
            {
                st.Append(DataTableToJsonObj(dt));
            }

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
                ReturnVal = GetReturnValue("200", "Data Get", st);
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
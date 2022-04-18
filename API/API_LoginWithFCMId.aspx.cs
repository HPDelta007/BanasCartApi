using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;
using System.Collections;
using System.Configuration;
using Newtonsoft.Json;

public partial class API_LoginWithFCMId : System.Web.UI.Page
{
    private tungComponents.tungDbDriver.DriverSqlServer itsDriver;

    string UserName;
    string Password;
    //string IMEI;
    //string FCMId;
    //string APIKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["UserName"] != null && Request.Form["UserName"] != "")
                {
                    UserName = Request.Form["UserName"].ToString();
                }
                else
                {
                    UserName = null;
                }

                if (Request.Form["Password"] != null && Request.Form["Password"] != "")
                {
                    Password = Request.Form["Password"].ToString();
                }
                else
                {
                    Password = null;
                }

                //if (Request.Form["IMEI"] != null && Request.Form["IMEI"] != "")
                //{
                //    IMEI = Request.Form["IMEI"].ToString();
                //}
                //else
                //{
                //    IMEI = null;
                //}

                //if (Request.Form["FCMId"] != null && Request.Form["FCMId"] != "")
                //{
                //    FCMId = Request.Form["FCMId"].ToString();
                //}
                //else
                //{
                //    FCMId = null;
                //}
                //if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                //{
                //    APIKey = Request.Form["APIKey"].ToString();
                //}
                //else
                //{
                //    APIKey = null;
                //}

                //Response.ContentType = "application/json";

                Response.ContentType = "application/x-www-form-urlencoded";

                //string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();
                //if (ConfigAPIKey == APIKey)
                //{
                    Response.Write(GetLogin(UserName, Password));
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

    public string GetLogin(string UserName,string Password)
    {
        DataSet Dt = new DataSet();
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        priConnect();
        try
        {
            UserName = UserName.Replace("'", "''");
            UserName = UserName.Replace(";", "");
            UserName = UserName.Replace("=", "");

            #region Valid User or not
            int IsValidUser = 0;
            IsValidUser = Convert.ToInt16(itsDriver.fExecuteScalar(" select count(*) from Users where MobileNo = '" + UserName + "' and isnull(IsDisabled, 0) = 0 "));

            if (IsValidUser == 0)
            {
                ReturnVal = GetReturnValue("201", "Your Username is invalid. Please try again.", st);
            }
            else
            {
                #region Check IsApprovedDisapproved
                string UserNameCnt = itsDriver.fExecuteScalar(" select count(*) from Users where MobileNo = '" + UserName + "'  and Password = '" + Password + "' and isnull(IsDisabled, 0) = 0").ToString();

                if (UserNameCnt == "0")
                {
                    int Cnt = Convert.ToInt16(itsDriver.fExecuteScalar(" select count(*) from Users where MobileNo = '" + UserName + "' "));

                    if (Cnt == 0)
                    {
                    }
                    else
                    {
                        itsDriver.fExecuteNonQuery("Update Users set Password = '" + Password + "' where MobileNo = '" + UserName + "' ");
                    }

                    ReturnVal = GetReturnValue("209", "User is not Approved For Mobile Login!!!", st);
                }
                else
                {
                    string pwd = itsDriver.fExecuteScalar(" select Password from Users where MobileNo = '" + UserName + "' and isnull(IsDisabled, 0) = 0").ToString();

                    #region Check IMEI Code
                    if (pwd == Password)
                    {
                        DataTable dt = new DataTable();

                        itsDriver.fSelectAndFillDataTable(" select m.*" +
                                                          " from Users m " +
                                                          " where m.MobileNo = '" + UserName + "' and isnull(IsDisabled, 0) = 0", dt);

                        st.Append(DataTableToJsonObj(dt));

                        itsDriver.fExecuteNonQuery("Update Users set Password = '" + Password + "' where MobileNo = '" + UserName + "'");

                        ReturnVal = GetReturnValue("200", "Data Get", st);
                    }
                    else
                    {
                        ReturnVal = GetReturnValue("209", "User is not Approved For Mobile Login!!!", st);
                    }
                    #endregion
                }
            }
            #endregion
            #endregion

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
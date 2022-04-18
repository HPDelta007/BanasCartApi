using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections;
using System.Data;

public partial class API_API_InsertUserAddress : System.Web.UI.Page
{
    string UserId;
    string PinCode;
    string Country;
    string State;
    string City;
    string StreetAddress;
    string MobileNo;
    string FullName;
    string AlternateMobileNumber;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);
                PinCode = (((Request.Form["PinCode"] != null && Request.Form["PinCode"] != "")) ? Request.Form["PinCode"].ToString() : null);
                Country = (((Request.Form["Country"] != null && Request.Form["Country"] != null)) ? Request.Form["Country"].ToString() : null);
                State = (((Request.Form["State"] != null && Request.Form["State"] != null)) ? Request.Form["State"].ToString() : null);
                City = (((Request.Form["City"] != null && Request.Form["City"] != null)) ? Request.Form["City"].ToString() : null);
                StreetAddress = (((Request.Form["StreetAddress"] != null && Request.Form["StreetAddress"] != null)) ? Request.Form["StreetAddress"].ToString() : null);
                MobileNo = (((Request.Form["MobileNo"] != null && Request.Form["MobileNo"] != null)) ? Request.Form["MobileNo"].ToString() : null);
                FullName = (((Request.Form["FullName"] != null && Request.Form["FullName"] != null)) ? Request.Form["FullName"].ToString() : null);
                AlternateMobileNumber = (((Request.Form["AlternateMobileNumber"] != null && Request.Form["AlternateMobileNumber"] != null)) ? Request.Form["AlternateMobileNumber"].ToString() : null);
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
            Response.Write(selectdata());
        }
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
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        try
        {

            UserId = _aPI_BLL.InsertUpdateWithReturnIdentity(" DECLARE  @UserAddressId uniqueidentifier;" +
                " SET @UserAddressId = NewId()" +
                " insert into UserAddresses (UserAddressId,UserId, PinCode, Country, State, City, StreetAddress, MobileNo, FullName, AlternateMobileNumber, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId)" +
                "values(@UserAddressId," +
                ((UserId == null) ? "NULL" : "'" + UserId.Replace("'","''") + "'")+ "," +
                ((PinCode == null) ? "NULL" : "'" + PinCode.Replace("'","''") + "'")+ "," +
                ((Country == null) ? "NULL" : "'" + Country.Replace("'","''") + "'")+ "," +
                ((State == null) ? "NULL" : "'" + State.Replace("'","''") + "'")+ "," +
                ((City == null) ? "NULL" : "'" + City.Replace("'","''") + "'")+ "," +
                ((StreetAddress == null) ? "NULL" : "'" + StreetAddress.Replace("'","''") + "'")+ "," +
                ((MobileNo == null) ? "NULL" : "'" + MobileNo.Replace("'", "''") + "'") + "," +
                ((FullName == null) ? "NULL" : "'" + FullName.Replace("'", "''") + "'") + "," +
                ((AlternateMobileNumber == null) ? "NULL" : "'" + AlternateMobileNumber.Replace("'", "''") + "'") + "," +
                 " GetDate(),GetDate(),'" + UserId.Replace("'","''") + "',NULL" + ");select @UserAddressId;");
           
            ReturnVal = GetReturnValue("200", "Data Inserted", st);
            return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
        }
        catch (Exception ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append(ex.Message);
            ReturnVal = GetReturnValue("209", "Data Get Issue", s);
            return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
        }
    }

    private string GetReturnValue(string Status, string Message, StringBuilder PassStringDataTable)
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
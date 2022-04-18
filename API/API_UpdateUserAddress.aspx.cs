using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Collections;
using System.Web.Script.Serialization;

public partial class API_API_UpdateUserAddress : System.Web.UI.Page
{
    string UserId;
    string UserAddressId;
    string Country;
    string State;
    string City;
    string PinCode;
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
                UserAddressId = (((Request.Form["UserAddressId"] != null && Request.Form["UserAddressId"] != "")) ? Request.Form["UserAddressId"].ToString() : null);
                Country = (((Request.Form["Country"] != null && Request.Form["Country"] != "")) ? Request.Form["Country"].ToString() : null);
                State = (((Request.Form["State"] != null && Request.Form["State"] != "")) ? Request.Form["State"].ToString() : null);
                City = (((Request.Form["City"] != null && Request.Form["City"] != "")) ? Request.Form["City"].ToString() : null);
                PinCode = (((Request.Form["PinCode"] != null && Request.Form["PinCode"] != "")) ? Request.Form["PinCode"].ToString() : null);
                StreetAddress = (((Request.Form["StreetAddress"] != null && Request.Form["StreetAddress"] != "")) ? Request.Form["StreetAddress"].ToString() : null);
                MobileNo = (((Request.Form["MobileNo"] != null && Request.Form["MobileNo"] != "")) ? Request.Form["MobileNo"].ToString() : null);
                FullName = (((Request.Form["FullName"] != null && Request.Form["FullName"] != null)) ? Request.Form["FullName"].ToString() : null);
                AlternateMobileNumber = (((Request.Form["AlternateMobileNumber"] != null && Request.Form["AlternateMobileNumber"] != null)) ? Request.Form["AlternateMobileNumber"].ToString() : null);

                Response.ContentType = "application/json";

                Response.Write(selectdata());
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
        DataTable da2 = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";

        try
        {
            da2 = _aPI_BLL.returnDataTable("select * from UserAddresses where UserAddressId = " + "'" + UserAddressId + "'");

            if (da2.Rows.Count > 0)
            {
                _aPI_BLL.InsertUpdateNonQuery(" update UserAddresses set Country = '" + Country + "',State = '" + State + 
                                              "',City = '" + City + "',PinCode = '" + PinCode +
                                              "',StreetAddress = '" + StreetAddress + "',MobileNo = '" + MobileNo +
                                              "',FullName = '" + FullName + "',AlternateMobileNumber = '" + AlternateMobileNumber +
                                              "', LastUpdatedOn = getdate(), LastUpdatedByUserId = '" + UserId + "' where UserAddressId = '" + UserAddressId + "' ");

                da = _aPI_BLL.returnDataTable(" select * from UserAddresses where UserAddressId = '" + UserAddressId + "' ");

                st.Append(DataTableToJsonObj(da));

                if (da == null)
                {
                    ReturnVal = GetReturnValue("209", "No Record Found", st);
                }

                if (st.ToString() == "[]" || st.ToString() == "")
                {
                    ReturnVal = GetReturnValue("209", "No Record Found", st);
                }

                if (da.Rows.Count > 0)
                {
                    ReturnVal = GetReturnValue("200", "Data Saved", st);
                }

                if (st.ToString() != "[]")
                    return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
                else
                    return ReturnVal.Replace("\\", "").Replace("\"[]\"", "[]");
            }

            else
            {
                StringBuilder s = new StringBuilder();
                ReturnVal = GetReturnValue("209", "User Not Registered", s);
                return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
            }


        }
        catch (Exception ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append(ex.Message);
            ReturnVal = GetReturnValue("209", "Data Get Issue", s);
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
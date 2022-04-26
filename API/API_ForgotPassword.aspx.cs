using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;

public partial class API_API_ForgotPassword : System.Web.UI.Page
{
    string MobileNo;
    string NewPassword;
    string ConfirmPassword;

    API_BLL _aPI_BLL = new API_BLL();
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

                if (Request.Form["NewPassword"] != null && Request.Form["NewPassword"] != "")
                    NewPassword = Request.Form["NewPassword"].ToString();
                else
                    NewPassword = null;

                if (Request.Form["ConfirmPassword"] != null && Request.Form["ConfirmPassword"] != "")
                    ConfirmPassword = Request.Form["ConfirmPassword"].ToString();
                else
                    ConfirmPassword = null;

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
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        try
        {
            da = _aPI_BLL.returnDataTable("select * from Users where MobileNo = '" + MobileNo + "' ");

            if (da.Rows.Count > 0)
            {
                if (NewPassword == ConfirmPassword)
                {
                    _aPI_BLL.InsertUpdateNonQuery("update Users set Password = '" + NewPassword.ToString() + "' where UserId = '" + da.Rows[0]["UserId"].ToString() + "' ");

                    da = _aPI_BLL.returnDataTable("select * from Users u where u.MobileNo = '" + MobileNo.ToString() + "'" +
                        " and isnull(IsDisabled, 0) = 0 ");

                    st.Append(DataTableToJsonObj(da));
                    ReturnVal = GetReturnValue("200", "Password change successfully", st);
                }
            }
            
            //else
            //{
            //    st.Append(DataTableToJsonObj(da));
            //    ReturnVal = GetReturnValue("209", "New Password and Confirm Password Do not Match", st);
            //}

            //if (da.Rows.Count > 0)
            //{
            //    st.Append(DataTableToJsonObj(da));
            //}
            //else
            //{
            //    st.Append(DataTableToJsonObj(da));
            //}

            if (da == null)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (st.ToString() == "[]" || st.ToString() == "")
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (NewPassword != ConfirmPassword)
            {
                ReturnVal = GetReturnValue("209", "New Password and Confirm Password Do not Match", st);
            }

            //if (da.Rows.Count > 0)
            //{
            //    ReturnVal = GetReturnValue("200", "Data Get", st);
            //}

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
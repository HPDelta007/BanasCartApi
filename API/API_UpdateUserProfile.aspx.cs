using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Web.Script.Serialization;

public partial class API_API_UpdateUserProfile : System.Web.UI.Page
{
    string UserId;
    string FirstName;
    string LastName;
    string EmailId;
    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);
                FirstName = (((Request.Form["FirstName"] != null && Request.Form["FirstName"] != "")) ? Request.Form["FirstName"].ToString() : null);
                LastName = (((Request.Form["LastName"] != null && Request.Form["LastName"] != "")) ? Request.Form["LastName"].ToString() : null);
                EmailId = (((Request.Form["EmailId"] != null && Request.Form["EmailId"] != "")) ? Request.Form["EmailId"].ToString() : null);

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
            da2 = _aPI_BLL.returnDataTable("select * from Users where UserId = '" + UserId + "' ");

            if (da2.Rows.Count > 0)
            {
                _aPI_BLL.InsertUpdateNonQuery(" update Users set FirstName = '" + FirstName + "',LastName = '" + LastName + 
                    "',EmailId = '" + EmailId + "', LastUpdatedOn = getdate(), LastUpdatedByUserId = '" + UserId + "' where UserId = '" + UserId + "' ");

                da = _aPI_BLL.returnDataTable(" select * from Users where UserId = '" + UserId + "' ");

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
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


public partial class API_ListOfComplaints : System.Web.UI.Page
{
    string UserId;
    string Status;
    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["UserId"] != null && Request.Form["UserId"] != "")
                    UserId = Request.Form["UserId"].ToString();
                else
                    UserId = null;

                if (Request.Form["Status"] != null && Request.Form["Status"] != "")
                    Status = Request.Form["Status"].ToString();
                else
                    Status = null;

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
            if (Status != null)
            {
                da = _aPI_BLL.returnDataTable(" select c.*, tp.[Text] as 'Problem Name' " +
                                              " , File1 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 1 ) " +
                                              " , File2 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 2 ) " +
                                              " , File3 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 3 ) " +
                                              " , File4 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 4 ) " +
                                              " , InVoiceImage = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 4 ) " +
                                              " , u.FirstName +' '+ u.LastName as 'AssignVisitUserName' "+
                                              " from Complaints c " +
                                              " inner join TextLists tp on tp.TextListId = c.ProblemTextListId " +
                                              " inner join Users ug on ug.UserId = c.GeneratedByUserId " +
                                              " left join Users u on u.UserId = c.AssignVisitTo " +
                                              " where c.GeneratedByUserId = '" + UserId.ToString() + "' and Status = '"+ Status +"' " +
                                              " order by c.Dt ");
            }
            else
            {
                da = _aPI_BLL.returnDataTable(" select c.*, tp.[Text] as 'Problem Name' " +
                                                 " , File1 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 1 ) " +
                                                 " , File2 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 2 ) " +
                                                 " , File3 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 3 ) " +
                                                 " , File4 = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 4 ) " +
                                                 " , InvoiceImage = ( select top 1 replace(x.FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from FU x where x.Record1Id = c.ComplaintId and x.LnNo = 4 ) " +
                                                 " , u.FirstName +' '+ u.LastName as 'AssignVisitUserName' " +
                                                 " from Complaints c " +
                                                 " inner join TextLists tp on tp.TextListId = c.ProblemTextListId " +
                                                 " inner join Users ug on ug.UserId = c.GeneratedByUserId " +
                                                 " left join Users u on u.UserId= c.AssignVisitTo " +
                                                 " where c.GeneratedByUserId = '" + UserId.ToString() + "' " +
                                                 " order by c.Dt ");
            }

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
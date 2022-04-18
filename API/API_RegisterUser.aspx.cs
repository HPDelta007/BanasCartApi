using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Data;
using System.Collections;

public partial class API_API_RegisterUser : System.Web.UI.Page
{
    string UserTypeTextListId;
    string FirstName;
    string LastName;
    string UserName;
    string Password;
    string MobileNo;
    string EmailId;
    string Address;

    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try 
            {

                UserTypeTextListId = (((Request.Form["UserTypeTextListId"] != null && Request.Form["UserTypeTextListId"] != "")) ? Request.Form["UserTypeTextListId"].ToString() : null);
                FirstName = (((Request.Form["FirstName"] != null && Request.Form["FirstName"] != "")) ? Request.Form["FirstName"].ToString() : null);
                LastName = (((Request.Form["LastName"] != null && Request.Form["LastName"] != "")) ? Request.Form["LastName"].ToString() : null);
                UserName = (((Request.Form["UserName"] != null && Request.Form["UserName"] != "")) ? Request.Form["UserName"].ToString() : null);
                Password = (((Request.Form["Password"] != null && Request.Form["Password"] != null)) ? Request.Form["Password"].ToString() : null);
                MobileNo = (((Request.Form["MobileNo"] != null && Request.Form["MobileNo"] != null)) ? Request.Form["MobileNo"].ToString() : null);
                EmailId = (((Request.Form["EmailId"] != null && Request.Form["EmailId"] != null)) ? Request.Form["EmailId"].ToString() : null);
                Address = (((Request.Form["Address"] != null && Request.Form["Address"] != null)) ? Request.Form["Address"].ToString() : null);

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();
                //if (ConfigAPIKey == APIKey)
                //{
                    Response.Write(selectdata());
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
            catch(Exception ex)
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
      DataSet ds= new DataSet();
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

            da = _aPI_BLL.returnDataTable(" select * from Users where Mobileno = '"+ MobileNo +"' ");

            if (da.Rows.Count == 0)
            {
                UserTypeTextListId = _aPI_BLL.InsertUpdateWithReturnIdentity(" DECLARE  @UserId uniqueidentifier;" +
                " SET @UserId = NewId()" +
               "insert into Users (UserId , UserTypeTextListId,FirstName, LastName, UserName, Password, Mobileno, EmailId, Address,InsertedOn,LastUpdatedOn,InsertedByUserId,LastUpdatedByUserId)" +
               "values(@UserId," +
               ((UserTypeTextListId == null) ? "NULL" : "'" + UserTypeTextListId.Replace("'","''") + "'") + "," +
               ((FirstName == null) ? "NULL" : "'" + FirstName.Replace("'", "''") + "'") + "," +
               ((LastName == null) ? "NULL" : "'" + LastName.Replace("'", "''") + "'") + "," +
               ((UserName == null) ? "NULL" : "'" + UserName.Replace("'", "''") + "'") + "," +
               ((Password == null) ? "NULL" : "'" + Password.Replace("'", "''") + "'") + "," +
               ((MobileNo == null) ? "NULL" : "'" + MobileNo.Replace("'", "''") + "'") + "," +
               ((EmailId == null) ? "NULL" : "'" + EmailId.Replace("'", "''") + "'") + "," +
               ((Address == null) ? "NULL" : "'" + Address.Replace("'", "''") + "'") + "," +
               " GetDate(),GetDate(),NULL,NULL" + ");select @UserId;");

                ReturnVal = GetReturnValue("200", "Data Inserted", st);
                return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
            }
            else 
            { 
                StringBuilder s = new StringBuilder();
                ReturnVal = GetReturnValue("209", "Mobile Number Allready Reistered Try Defferent", s);
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
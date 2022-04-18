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


public partial class API_Parameters : System.Web.UI.Page
{
    string Code;
    string ParemeterNo;
    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                Code = (((Request.Form["Code"] != null && Request.Form["Code"] != "")) ? Request.Form["Code"].ToString() : null);
                ParemeterNo = (((Request.Form["ParemeterNo"] != null && Request.Form["ParemeterNo"] != "")) ? Request.Form["ParemeterNo"].ToString() : null);

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
        string[] Specialchars = new string[] { "\\", "/", "\"", "\n", "\r", "\t", "\x08", "\x0c" };
        string[] ReplacedChar = new string[] { "\\\\", "\\/", "\\\"", "\\n", "\\r", "\\t", "\\f", "\\b" };

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
                        string replaceText;

                        replaceText = ds.Tables[0].Rows[i][j].ToString();

                        for (int h = 0; h < Specialchars.Length; h++)
                        {
                            if (replaceText.Contains(Specialchars[h]))
                            {
                                replaceText = replaceText.Replace(Specialchars[h], ReplacedChar[h]);
                            }
                        }

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        string replaceText;

                        replaceText = ds.Tables[0].Rows[i][j].ToString();

                        for (int h = 0; h < Specialchars.Length; h++)
                        {
                            if (replaceText.Contains(Specialchars[h]))
                            {
                                replaceText = replaceText.Replace(Specialchars[h], ReplacedChar[h]);
                            }
                        }

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\"");
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
        string sql="";
        try
        {

            sql = "declare @c1 varchar(25);  " +
                " declare @m1 nvarchar(256); " +
                " declare @code nvarchar(256); " +
                " declare @name nvarchar(256); " +
                " declare @ParentGrp int; " +
                " select @code=m.code,@name=m.name,@ParentGrp=m.ParentGrp,@c1=c.C" + ParemeterNo + ",@m1=c.m1 " +
                "  from Master1 m ,[Config] c " +
                "  where m.code =  c.L1 " +
                "  and  m.code = (select ParentGrp from Master1 where Code = " + Code + "); " +
                " select  * from [dbo].[splitstringForBusy](@m1,@c1," + ParemeterNo + ")   ";

            da = _aPI_BLL.returnDataTable(sql);


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
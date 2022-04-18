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
using System.IO;

public partial class API_BannerImages : System.Web.UI.Page
{
    string APIKey;
    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
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
    private string replaceSplChar(string Data)
    {
        Data = Data.Replace("%", "");
        Data = Data.Replace("<", "");
        Data = Data.Replace(">", "");
        Data = Data.Replace("&", "");
        Data = Data.Replace("+", "");
        Data = Data.Replace("#", "");
        Data = Data.Replace("*", "");
        Data = Data.Replace("!", "");
        Data = Data.Replace(",", "");
        Data = Data.Replace("'", "");
        Data = Data.Replace("=", "");
        Data = Data.Replace("â‚¬", "");
        Data = Data.Replace("?", "");
        Data = Data.Replace("$", "");
        Data = Data.Replace("[", "");
        Data = Data.Replace("]", "");
        Data = Data.Replace("\"", "`");
        Data = Data.Replace("\t", " ");
        Data = Data.Replace("\r\n", " ");
        Data = Data.Replace("\\", "\"");

        return Data;
    }
    public string selectdata()
    {
        DataTable da = new DataTable();
        DataTable dt = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";

        try
        {
            //da = _aPI_BLL.returnDataTable(" select '" + ConfigurationManager.AppSettings["FolderPathShow"].ToString() + "banner1.jfif" +
            //                              "|" + ConfigurationManager.AppSettings["FolderPathShow"].ToString() + "banner2.jfif" +
            //                              "|" + ConfigurationManager.AppSettings["FolderPathShow"].ToString() + "banner3.jfif' as Photo ");

            da = _aPI_BLL.returnDataTable(" select BannerImageId ,Title " +
                                          " , replace(FilePath, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') as BannerImageNameShow " +
                                          " from BannerImages ");
            //string photolink = "";
            //if (dt.Rows.Count > 0)
            //{
                
            //    string path = "";                
            //    for (int i=0; i<dt.Rows.Count ;i++)
            //    {
            //        path = dt.Rows[i]["FilePath"].ToString().Replace("\"","\\");
            //        string filename = null;
            //        filename = Path.GetFileName(path);
            //        if (photolink == "")
            //        {
            //            photolink = ConfigurationManager.AppSettings["FolderPathShow"].ToString() + filename;
            //        }
            //        else
            //        {
            //            photolink += "|" + ConfigurationManager.AppSettings["FolderPathShow"].ToString() + filename;
            //        }
            //    }
            //}

            //da = _aPI_BLL.returnDataTable(" select '" + photolink + "' as Photo ");

            //da = _aPI_BLL.returnDataTable(" select BannerImageId, Title, FilePath from BannerImages ");

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
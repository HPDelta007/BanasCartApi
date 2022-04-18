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


public partial class API_ListOfDistributorDealers : System.Web.UI.Page
{
    string InsertedByUserId;
    string DistributorDealerId;
    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["InsertedByUserId"] != null && Request.Form["InsertedByUserId"] != "")
                    InsertedByUserId = Request.Form["InsertedByUserId"].ToString();
                else
                    InsertedByUserId = null;

                if (Request.Form["DistributorDealerId"] != null && Request.Form["DistributorDealerId"] != "")
                    DistributorDealerId = Request.Form["DistributorDealerId"].ToString();
                else
                    DistributorDealerId = null;

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
        try
        {
            string where = null;

            where = "";

            if (InsertedByUserId != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " InsertedByUserId = '" + InsertedByUserId + "' ";
            }

            if (DistributorDealerId != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " DistributorDealerId = '" + DistributorDealerId + "' ";
            }

            if (where != "")
            {
                where = " WHERE " + where;
            }

            da = _aPI_BLL.returnDataTable(" select DistributorDealerId, DataType, replace(Name, '\"', 'U+0022') as Name, Address, Lat, Long, States, District, CityVillage, ContactPersonName, WhatsAppMobileNo, OtherMobileNo, EmailId, GSTNo, PANNo, BankName, BankACNo, BankIFSCCode, ProfilePhoto, CoverPhoto, ParentDistributorDealerId, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId from DistributorDealers " + where + " order by Name  ");

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
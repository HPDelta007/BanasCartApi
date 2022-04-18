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
using System.Web.Services;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web.Configuration;

public partial class API_ItmDetails : System.Web.UI.Page
{
    //string Code;
    //string APIKey;
    //string CustCode;
    //string PartNo;
    //string Top10;
    //string Top10KDH;

    string ItmId;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ItmId = (((Request.Form["ItmId"] != null && Request.Form["ItmId"] != "")) ? Request.Form["ItmId"].ToString() : null);

                //Code = (((Request.Form["Code"] != null && Request.Form["Code"] != "")) ? Request.Form["Code"].ToString() : null);
                //PartNo = (((Request.Form["PartNo"] != null && Request.Form["PartNo"] != "")) ? Request.Form["PartNo"].ToString() : null);

                //if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                //    APIKey = Request.Form["APIKey"].ToString();
                //else
                //    APIKey = null;

                //CustCode = (((Request.Form["CustCode"] != null && Request.Form["CustCode"] != "")) ? Request.Form["CustCode"].ToString() : null);

                //if (Request.Form["Top10"] != null && Request.Form["Top10"] != "")
                //    Top10 = Request.Form["Top10"].ToString();
                //else
                //    Top10 = null;

                //if (Request.Form["Top10KDH"] != null && Request.Form["Top10KDH"] != "")
                //    Top10KDH = Request.Form["Top10KDH"].ToString();
                //else
                //    Top10KDH = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                //if (ConfigAPIKey == APIKey)
                //{
                //    if (Code == null && PartNo == null && Top10 == null && Top10KDH == null)
                //    {
                //        string sw = "";
                //        StringBuilder s = new StringBuilder();
                //        s.Append("Pass Code or Part No.");
                //        sw = GetReturnValue("209", "Pass Code or Part No.", s);
                //        Response.ContentType = "application/json";
                //        Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                //    }
                //    //else if (Top10 != null && CustCode == null)
                //    //{
                //    //    string sw = "";
                //    //    StringBuilder s = new StringBuilder();
                //    //    s.Append("For TOP 10 Product Customer Code is compulsory.");
                //    //    sw = GetReturnValue("209", "For TOP 10 Product Customer Code is compulsory.", s);
                //    //    Response.ContentType = "application/json";
                //    //    Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                //    //}
                //    else
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

    private static string replaceSplChar(string Data)
    {
        Data = Data.Replace("%", "%25");
        Data = Data.Replace("<", "%3C");
        Data = Data.Replace(">", "%3E");
        Data = Data.Replace("&", "%26");
        Data = Data.Replace("+", "%2B");
        Data = Data.Replace("#", "%23");
        Data = Data.Replace("*", "%2A");
        Data = Data.Replace("!", "%21");
        // Data = Data.Replace(",", "%2C");
        Data = Data.Replace("'", "%27");
        Data = Data.Replace("=", "%3D");
        Data = Data.Replace("â‚¬", "%E2%82%AC");
        Data = Data.Replace("?", "%3F");
        //Data = Data.Replace(" ", "+");
        Data = Data.Replace("$", "%24");
        Data = Data.Replace("[", "");
        Data = Data.Replace("]", "");
        Data = Data.Replace("\"", "`");

        return Data;
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
                        string replaceText;

                        replaceText = replaceSplChar(ds.Tables[0].Rows[i][j].ToString());

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        string replaceText;

                        replaceText = replaceSplChar(ds.Tables[0].Rows[i][j].ToString());

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
            string Where = "";

            if (ItmId != null)
                Where = " Where ItmId = '" + ItmId.Replace("'", "") + "' ";

            da = _aPI_BLL.returnDataTable(" select * "+
                                          " , replace(ImageName, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') as ImageNameShow "+
                                          " from Itms " + Where + " order by Name ");


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

    [WebMethod]
    public void GetRateAndGST(string PartCode, string CustCode, out string PLRate, out string NetRate, out string GST)
    {
        ArrayList al = new ArrayList();

        DataTable dtPlant = new DataTable();

        var baseAddress = WebConfigurationManager.AppSettings["APIURL"].ToString();

        HttpWebRequest Http = (HttpWebRequest)WebRequest.Create(baseAddress);

        Http.ContentType = "application/json; charset=utf-8";

        Http.Method = "POST";
        byte[] lbPostBuffer = Encoding.Default.GetBytes("{PartCode:\"" + PartCode + "\",CustCode:\"" + CustCode + "\"}");

        Http.ContentLength = lbPostBuffer.Length;

        Stream PostStream = Http.GetRequestStream();
        PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
        PostStream.Close();

        HttpWebResponse WebResponse = (HttpWebResponse)Http.GetResponse();

        Stream responseStream = WebResponse.GetResponseStream();

        StreamReader Reader = new StreamReader(responseStream, Encoding.Default);

        string Html = Reader.ReadToEnd();

        JObject json = JObject.Parse(Html);

        var data = (JObject)JsonConvert.DeserializeObject(json.ToString());
        string actualData = data.ToString();

        JObject obj = new JObject();

        try
        {
            obj = JObject.Parse(actualData.ToString());
        }
        catch
        {
        }

        PLRate = obj.SelectToken("PLRate").ToString();
        NetRate = obj.SelectToken("NetRate").ToString();
        GST = obj.SelectToken("GST").ToString();

        WebResponse.Close();
        responseStream.Close();
    }
}
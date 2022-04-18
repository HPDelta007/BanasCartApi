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


public partial class API_GetLedgerAgingReport : System.Web.UI.Page
{
    string LgrName;
    string LgrGrpId;
    string Status;
    DateTime? FromDate;
    DateTime? ToDate;
    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["LgrCode"] != null && Request.Form["LgrCode"] != "")
                    LgrName = Request.Form["LgrCode"].ToString();
                else
                    LgrName = null;

                if (Request.Form["LgrGrpCode"] != null && Request.Form["LgrGrpCode"] != "")
                    LgrGrpId = Request.Form["LgrGrpCode"].ToString();
                else
                    LgrGrpId = null;

                if (Request.Form["FromDate"] != null && Request.Form["FromDate"] != "")
                    FromDate = Convert.ToDateTime(Request.Form["FromDate"]);
                else
                    FromDate = null;

                if (Request.Form["ToDate"] != null && Request.Form["ToDate"] != "")
                    ToDate = Convert.ToDateTime(Request.Form["ToDate"]);
                else
                    ToDate = null;

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

        DataTable dtFinal = new DataTable();
        //DataSet ds = new DataSet();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";
        try
        {
            da = _aPI_BLL.CustomerPaymentStatus(FromDate, ToDate, LgrName, LgrGrpId);

            if (da.Rows.Count > 0)
            {
                decimal OpeningAmt = Convert.ToDecimal(da.Rows[0]["LedgerOpeningBalance"].ToString());
                decimal PlusAmt = Convert.ToDecimal(da.Rows[0]["LedgerCrAmt"].ToString()) + Convert.ToDecimal(da.Rows[0]["LedgerCrAmtCRNT"].ToString());
                decimal MinusAmt = Convert.ToDecimal(da.Rows[0]["LedgerDrAmt"].ToString());
                decimal ClosingAmt = Convert.ToDecimal(da.Rows[0]["LedgerClosingAmt"].ToString());

                decimal CummOutStanding0015 = Convert.ToDecimal(da.Rows[0]["D30"].ToString());
                decimal CummOutStanding1630 = 0; // Convert.ToDecimal(da.Rows[0]["D45"].ToString());
                decimal CummOutStanding3145 = Convert.ToDecimal(da.Rows[0]["D60"].ToString());
                decimal CummOutStanding4560 = 0; // Convert.ToDecimal(da.Rows[0]["D90"].ToString());
                decimal CummOutStanding6190 = Convert.ToDecimal(da.Rows[0]["D90"].ToString());
                decimal CummOutStanding9199 = Convert.ToDecimal(da.Rows[0]["DA90"].ToString());

                dtFinal.Clear();
                dtFinal.Columns.Add("CummOutStanding0015");
                dtFinal.Columns.Add("CummOutStanding1630");
                dtFinal.Columns.Add("CummOutStanding3145");
                dtFinal.Columns.Add("CummOutStanding4560");
                dtFinal.Columns.Add("CummOutStanding6190");
                dtFinal.Columns.Add("CummOutStanding9199");

                dtFinal.Columns.Add("Opening");

                dtFinal.Columns.Add("Debit");

                dtFinal.Columns.Add("Credit");

                dtFinal.Columns.Add("Outstanding");
                dtFinal.Columns.Add("OpeningSufix");
                dtFinal.Columns.Add("OutstandingSufix");

                DataRow _OtSt = dtFinal.NewRow();

                _OtSt["CummOutStanding0015"] = CummOutStanding0015.ToString();
                _OtSt["CummOutStanding1630"] = CummOutStanding1630.ToString();
                _OtSt["CummOutStanding3145"] = CummOutStanding3145.ToString();
                _OtSt["CummOutStanding4560"] = CummOutStanding4560.ToString();
                _OtSt["CummOutStanding6190"] = CummOutStanding6190.ToString();
                _OtSt["CummOutStanding9199"] = CummOutStanding9199.ToString();

                _OtSt["Opening"] = (OpeningAmt < 0 ? OpeningAmt * (-1) : OpeningAmt).ToString();
                _OtSt["Debit"] = MinusAmt.ToString();
                _OtSt["Credit"] = PlusAmt.ToString();
                _OtSt["Outstanding"] = (ClosingAmt < 0 ? ClosingAmt * (-1) : ClosingAmt).ToString();

                if (OpeningAmt > 0)
                {
                    _OtSt["OpeningSufix"] = "Dr".ToString();
                }
                else
                {
                    _OtSt["OpeningSufix"] = "Cr".ToString();
                }

                if (ClosingAmt > 0)
                {
                    _OtSt["OutstandingSufix"] = "Dr".ToString();
                }
                else
                {
                    _OtSt["OutstandingSufix"] = "Cr".ToString();
                }

                dtFinal.Rows.Add(_OtSt);

                st.Append(DataTableToJsonObj(dtFinal));
            }
            else
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (da == null)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }
            else if (st.ToString() == "[]" || st.ToString() == "")
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }
            else if (da.Rows.Count > 0)
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
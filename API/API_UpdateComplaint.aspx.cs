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


public partial class API_UpdateComplaint : System.Web.UI.Page
{
    string ComplaintId;

    string ProductType;
    string PumpSerialNo;
    string MotorSerialNo;
    string ProblemTextListId;
    string CompaintDetail;
    string PumpModel;
    string MotorModel;
    decimal? HP;
    decimal? Stage;
    string SolutionTextListId;
    string SolutionRemarks;
    DateTime? SolvedOn;
    decimal? ServiceCharges;
    string Status;
    string GeneratedByUserId;

    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ComplaintId = (((Request.Form["ComplaintId"] != null && Request.Form["ComplaintId"] != "")) ? Request.Form["ComplaintId"].ToString() : null);
                ProductType = (((Request.Form["ProductType"] != null && Request.Form["ProductType"] != "")) ? Request.Form["ProductType"].ToString() : null);
                PumpSerialNo = (((Request.Form["PumpSerialNo"] != null && Request.Form["PumpSerialNo"] != "")) ? Request.Form["PumpSerialNo"].ToString() : null);
                MotorSerialNo = (((Request.Form["MotorSerialNo"] != null && Request.Form["MotorSerialNo"] != "")) ? Request.Form["MotorSerialNo"].ToString() : null);
                ProblemTextListId = (((Request.Form["ProblemTextListId"] != null && Request.Form["ProblemTextListId"] != "")) ? Request.Form["ProblemTextListId"].ToString() : null);
                CompaintDetail = (((Request.Form["CompaintDetail"] != null && Request.Form["CompaintDetail"] != "")) ? Request.Form["CompaintDetail"].ToString() : null);
                PumpModel = (((Request.Form["PumpModel"] != null && Request.Form["PumpModel"] != "")) ? Request.Form["PumpModel"].ToString() : null);
                MotorModel = (((Request.Form["MotorModel"] != null && Request.Form["MotorModel"] != "")) ? Request.Form["MotorModel"].ToString() : null);
                HP = (((Request.Form["HP"] != null && Request.Form["HP"] != "")) ? Convert.ToDecimal(Request.Form["HP"].ToString()) : Convert.ToDecimal(null));
                Stage = (((Request.Form["Stage"] != null && Request.Form["Stage"] != "")) ? Convert.ToDecimal(Request.Form["Stage"].ToString()) : Convert.ToDecimal(null));
                SolutionTextListId = (((Request.Form["SolutionTextListId"] != null && Request.Form["SolutionTextListId"] != "")) ? Request.Form["SolutionTextListId"].ToString() : null);
                SolutionRemarks = (((Request.Form["SolutionRemarks"] != null && Request.Form["SolutionRemarks"] != "")) ? Request.Form["SolutionRemarks"].ToString() : null);
                SolvedOn = (((Request.Form["SolvedOn"] != null && Request.Form["SolvedOn"] != "")) ? Convert.ToDateTime(Request.Form["SolvedOn"].ToString()) : Convert.ToDateTime(null));
                ServiceCharges = (((Request.Form["ServiceCharges"] != null && Request.Form["ServiceCharges"] != "")) ? Convert.ToDecimal(Request.Form["ServiceCharges"].ToString()) : Convert.ToDecimal(null));
                Status = (((Request.Form["Status"] != null && Request.Form["Status"] != "")) ? Request.Form["Status"].ToString() : null);
                GeneratedByUserId = (((Request.Form["GeneratedByUserId"] != null && Request.Form["GeneratedByUserId"] != "")) ? Request.Form["GeneratedByUserId"].ToString() : null);

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
            _aPI_BLL.InsertUpdateNonQuery(" update Complaints set " +
                                          " ProductType = " + ((ProductType == null) ? "NULL" : "'" + ProductType.ToString() + "'") + " " +
                                          " , PumpSerialNo =  " + ((PumpSerialNo == null) ? "NULL" : "'" + PumpSerialNo.ToString().Replace("'", "''") + "'") + " " +
                                          " , MotorSerialNo = " + ((MotorSerialNo == null) ? "NULL" : "'" + MotorSerialNo.ToString().Replace("'", "''") + "'") + " " +
                                          " , ProblemTextListId = " + ((ProblemTextListId == null) ? "NULL" : "'" + ProblemTextListId.ToString().Replace("'", "''") + "'") + " " +
                                          " , CompaintDetail = " + ((CompaintDetail == null) ? "NULL" : "N'" + CompaintDetail.ToString().Replace("'", "''") + "'") + " " +
                                          " , PumpModel = " + ((PumpModel == null) ? "NULL" : "'" + PumpModel.ToString().Replace("'", "''") + "'") + " " +
                                          " , MotorModel = " + ((MotorModel == null) ? "NULL" : "'" + MotorModel.ToString().Replace("'", "''") + "'") + " " +
                                          " , HP = " + ((HP == null) ? "NULL" : "'" + HP.ToString().Replace("'", "''") + "'") + " " +
                                          " , Stage = " + ((Stage == null) ? "NULL" : "'" + Stage.ToString().Replace("'", "''") + "'") + " " +
                                          " , SolutionTextListId = " + ((SolutionTextListId == null) ? "NULL" : "'" + SolutionTextListId.ToString().Replace("'", "''") + "'") + " " +
                                          " , SolutionRemarks = " + ((SolutionRemarks == null) ? "NULL" : "N'" + SolutionRemarks.ToString().Replace("'", "''") + "'") + " " +
                                          " , SolvedOn = " + ((SolvedOn == null) ? "NULL" : "'" + Convert.ToDateTime(SolvedOn).ToString("dd-MMM-yyyy").Replace("'", "''") + "'") + " " +
                                          " , ServiceCharges = " + ((ServiceCharges == null) ? "NULL" : "'" + ServiceCharges.ToString().Replace("'", "''") + "'") + " " +
                                          " , LastUpdatedOn = GETDATE()" +
                                          " , LastUpdatedByUserId = '" + GeneratedByUserId + "' " +
                                          " , Status = " + ((Status == null) ? "NULL" : "'" + Status.ToString().Replace("'", "''") + "'") + " " +
                                          " where ComplaintId = '" + ComplaintId.ToString() + "' ");

            if (ComplaintId != null)
                da = _aPI_BLL.returnDataTable("  select No from Complaints where ComplaintId = '" + ComplaintId.ToString() + "'  ");
            else
                da = null;

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
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


public partial class API_InsertComplaint : System.Web.UI.Page
{
    string ComplaintId;
    string No;
    DateTime? Dt;
    string ProductType;
    string PumpSerialNo;
    string MotorSerialNo;
    string FarmerName;
    string FarmerMobileNo;
    string ProblemTextListId;
    string CompaintDetail;
    string PumpModel;
    string MotorModel;
    decimal? HP;
    decimal? Stage;
    string GeneratedByUserType;
    string GeneratedByUserId;
    string Status;
    DateTime? DealerInvoiceDate;
    string DealerInvoiceNo;
    string DealerName;
    DateTime? DistributorInvoiceDate;
    string DistributorInvoiceNo;
    string DistributorName;
    HttpPostedFile file1;
    HttpPostedFile file2;
    HttpPostedFile file3;
    HttpPostedFile file4;

    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                No = (((Request.Form["No"] != null && Request.Form["No"] != "")) ? Request.Form["No"].ToString() : null);
                Dt = (((Request.Form["Dt"] != null && Request.Form["Dt"] != "")) ? Convert.ToDateTime(Request.Form["Dt"].ToString()) : Convert.ToDateTime(null));
                ProductType = (((Request.Form["ProductType"] != null && Request.Form["ProductType"] != "")) ? Request.Form["ProductType"].ToString() : null);
                PumpSerialNo = (((Request.Form["PumpSerialNo"] != null && Request.Form["PumpSerialNo"] != "")) ? Request.Form["PumpSerialNo"].ToString() : null);
                MotorSerialNo = (((Request.Form["MotorSerialNo"] != null && Request.Form["MotorSerialNo"] != "")) ? Request.Form["MotorSerialNo"].ToString() : null);
                FarmerName = (((Request.Form["FarmerName"] != null && Request.Form["FarmerName"] != "")) ? Request.Form["FarmerName"].ToString() : null);
                FarmerMobileNo = (((Request.Form["FarmerMobileNo"] != null && Request.Form["FarmerMobileNo"] != "")) ? Request.Form["FarmerMobileNo"].ToString() : null);
                ProblemTextListId = (((Request.Form["ProblemTextListId"] != null && Request.Form["ProblemTextListId"] != "")) ? Request.Form["ProblemTextListId"].ToString() : null);
                CompaintDetail = (((Request.Form["CompaintDetail"] != null && Request.Form["CompaintDetail"] != "")) ? Request.Form["CompaintDetail"].ToString() : null);
                PumpModel = (((Request.Form["PumpModel"] != null && Request.Form["PumpModel"] != "")) ? Request.Form["PumpModel"].ToString() : null);
                MotorModel = (((Request.Form["MotorModel"] != null && Request.Form["MotorModel"] != "")) ? Request.Form["MotorModel"].ToString() : null);
                HP = (((Request.Form["HP"] != null && Request.Form["HP"] != "")) ? Convert.ToDecimal(Request.Form["HP"].ToString()) : Convert.ToDecimal(null));
                Stage = (((Request.Form["Stage"] != null && Request.Form["Stage"] != "")) ? Convert.ToDecimal(Request.Form["Stage"].ToString()) : Convert.ToDecimal(null));
                GeneratedByUserType = (((Request.Form["GeneratedByUserType"] != null && Request.Form["GeneratedByUserType"] != "")) ? Request.Form["GeneratedByUserType"].ToString() : null);
                GeneratedByUserId = (((Request.Form["GeneratedByUserId"] != null && Request.Form["GeneratedByUserId"] != "")) ? Request.Form["GeneratedByUserId"].ToString() : null);
                Status = (((Request.Form["Status"] != null && Request.Form["Status"] != "")) ? Request.Form["Status"].ToString() : null);
                DealerInvoiceDate = (((Request.Form["DealerInvoiceDate"] != null && Request.Form["DealerInvoiceDate"] != "")) ? Convert.ToDateTime(Request.Form["DealerInvoiceDate"].ToString()) : Convert.ToDateTime(null));
                DealerInvoiceNo = (((Request.Form["DealerInvoiceNo"] != null && Request.Form["DealerInvoiceNo"] != "")) ? Request.Form["DealerInvoiceNo"].ToString() : null);
                DealerName = (((Request.Form["DealerName"] != null && Request.Form["DealerName"] != "")) ? Request.Form["DealerName"].ToString() : null);
                DistributorInvoiceDate = (((Request.Form["DistributorInvoiceDate"] != null && Request.Form["DistributorInvoiceDate"] != "")) ? Convert.ToDateTime(Request.Form["DistributorInvoiceDate"].ToString()) : Convert.ToDateTime(null));
                DistributorInvoiceNo = (((Request.Form["DistributorInvoiceNo"] != null && Request.Form["DistributorInvoiceNo"] != "")) ? Request.Form["DistributorInvoiceNo"].ToString() : null);
                DistributorName = (((Request.Form["DistributorName"] != null && Request.Form["DistributorName"] != "")) ? Request.Form["DistributorName"].ToString() : null);
                file1 = (((Request.Files["file1"] != null && Request.Files["file1"].FileName.ToString() != "")) ? Request.Files["file1"] : null);
                file2 = (((Request.Files["file2"] != null && Request.Files["file2"].FileName.ToString() != "")) ? Request.Files["file2"] : null);
                file3 = (((Request.Files["file3"] != null && Request.Files["file3"].FileName.ToString() != "")) ? Request.Files["file3"] : null);
                file4 = (((Request.Files["file4"] != null && Request.Files["file4"].FileName.ToString() != "")) ? Request.Files["file4"] : null);

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
            da = _aPI_BLL.returnDataTable(" declare @data varchar(10); " +
                                          " select @data = case when EXISTS(SELECT replace(convert(varchar, '" + Convert.ToDateTime(Dt).ToString("ddMMyy") + "', 101), '/','') + convert(varchar, a.Ct+1) AS No FROM ComplaintNos a WHERE  a.Dt = '" + Convert.ToDateTime(Dt).ToString("dd-MMM-yyyy") + "') then '1' else '2' end  " +
                                          " print @data " +
                                          " If @data = '1'   " +
                                          " SELECT replace(convert(varchar, '" + Convert.ToDateTime(Dt).ToString("ddMMyy") + "', 101), '/','') + '/' + convert(varchar, a.Ct+1) AS No FROM ComplaintNos a WHERE  a.Dt = '" + Convert.ToDateTime(Dt).ToString("dd-MMM-yyyy") + "' " +
                                          " else " +
                                          " SELECT replace(convert(varchar, '" + Convert.ToDateTime(Dt).ToString("ddMMyy") + "', 101), '/','') + '/' + convert(varchar, 1) AS No ");

            if (da.Rows.Count > 0)
            {
                ComplaintId = _aPI_BLL.InsertUpdateWithReturnIdentity(" insert into Complaints (No, Dt, ProductType, PumpSerialNo, MotorSerialNo, FarmerName, FarmerMobileNo, ProblemTextListId, " +
                                            " CompaintDetail, PumpModel, MotorModel, HP, Stage, GeneratedByUserType, GeneratedByUserId, Status, DealerInvoiceDate, " +
                                            "  DealerInvoiceNo, DealerName, DistributorInvoiceDate, DistributorInvoiceNo, DistributorName, InsertedOn, LastUpdatedOn, " +
                                            "  InsertedByUserId, LastUpdatedByUserId) values " +
                                            " (" +
                                            " '" + da.Rows[0][0].ToString() + "' " +
                                            " , " + ((Dt == null) ? "NULL" : "'" + Convert.ToDateTime(Dt).ToString("dd-MMM-yyyy") + "'") + " " +
                                            " , " + ((ProductType == null) ? "NULL" : "'" + ProductType.ToString() + "'") + " " +
                                            " , " + ((PumpSerialNo == null) ? "NULL" : "'" + PumpSerialNo.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((MotorSerialNo == null) ? "NULL" : "'" + MotorSerialNo.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((FarmerName == null) ? "NULL" : "'" + FarmerName.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((FarmerMobileNo == null) ? "NULL" : "'" + FarmerMobileNo.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((ProblemTextListId == null) ? "NULL" : "'" + ProblemTextListId.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((CompaintDetail == null) ? "NULL" : "N'" + CompaintDetail.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((PumpModel == null) ? "NULL" : "'" + PumpModel.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((MotorModel == null) ? "NULL" : "'" + MotorModel.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((HP == null) ? "NULL" : "'" + HP.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((Stage == null) ? "NULL" : "'" + Stage.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((GeneratedByUserType == null) ? "NULL" : "'" + GeneratedByUserType.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((GeneratedByUserId == null) ? "NULL" : "'" + GeneratedByUserId.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((Status == null) ? "NULL" : "'" + Status.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((DealerInvoiceDate == null) ? "NULL" : "'" + Convert.ToDateTime(DealerInvoiceDate).ToString("dd-MMM-yyyy").Replace("'", "''") + "'") + " " +
                                            " , " + ((DealerInvoiceNo == null) ? "NULL" : "'" + DealerInvoiceNo.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((DealerName == null) ? "NULL" : "'" + DealerName.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((DistributorInvoiceDate == null) ? "NULL" : "'" + Convert.ToDateTime(DistributorInvoiceDate).ToString("dd-MMM-yyyy").Replace("'", "''") + "'") + " " +
                                            " , " + ((DistributorInvoiceNo == null) ? "NULL" : "'" + DistributorInvoiceNo.ToString().Replace("'", "''") + "'") + " " +
                                            " , " + ((DistributorName == null) ? "NULL" : "'" + DistributorName.ToString().Replace("'", "''") + "'") + " " +
                                            " , GETDATE(), GETDATE(), '" + GeneratedByUserId + "', NULL " +
                                            ");select @@IDENTITY;");


                if (ComplaintId != null)
                {
                    _aPI_BLL.InsertUpdateNonQuery(" Declare @Count AS int;" +
                                     " SELECT @Count=COUNT(*)FROM ComplaintNos  WHERE Dt = '" + Convert.ToDateTime(Dt).ToString("dd-MMM-yyyy") + "'" +
                                     " IF(@Count=0)" +
                                     " BEGIN" +
                                     " INSERT INTO ComplaintNos (Dt,Ct) VALUES ('" + Convert.ToDateTime(Dt).ToString("dd-MMM-yyyy") + "',1)" +
                                     " END" +
                                     " ELSE" +
                                     " BEGIN" +
                                     " UPDATE ComplaintNos SET Ct=Ct+1 WHERE Dt = '" + Convert.ToDateTime(Dt).ToString("dd-MMM-yyyy") + "'" +
                                     " END");

                    if (file1 != null && file1.FileName != "")
                    {
                        string[] getExtenstion = file1.FileName.Split('.');
                        string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                        string FileNameForInsert = file1.FileName.Replace("." + oExtension, "");

                        var filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + FileNameForInsert + "-1." + oExtension;
                        file1.SaveAs(filePath);

                        _aPI_BLL.InsertUpdateNonQuery(" INSERT INTO FU (FUId, LnNo, Record1Id, FormName, FilePath, FileName, InsertedOn, " +
                                                  " LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values (" +
                                                  " NEWID(), 1, '" + ComplaintId.ToString() + "', 'ComplaintApp', '" + filePath.ToString() + "' " +
                                                  " , '" + FileNameForInsert + "-1." + oExtension + "', GETDATE(), GETDATE(), '" + GeneratedByUserId.ToString() + "' " +
                                                  " , '" + GeneratedByUserId.ToString() + "' " +
                                                  ") ");
                    }

                    if (file2 != null && file2.FileName != "")
                    {
                        string[] getExtenstion = file2.FileName.Split('.');
                        string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                        string FileNameForInsert = file2.FileName.Replace("." + oExtension, "");

                        var filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + FileNameForInsert + "-2." + oExtension;
                        file2.SaveAs(filePath);

                        _aPI_BLL.InsertUpdateNonQuery(" INSERT INTO FU (FUId, LnNo, Record1Id, FormName, FilePath, FileName, InsertedOn, " +
                                                  " LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values (" +
                                                  " NEWID(), 2, '" + ComplaintId.ToString() + "', 'ComplaintApp', '" + filePath.ToString() + "' " +
                                                  " , '" + FileNameForInsert + "-2." + oExtension + "', GETDATE(), GETDATE(), '" + GeneratedByUserId.ToString() + "' " +
                                                  " , '" + GeneratedByUserId.ToString() + "' " +
                                                  ") ");
                    }

                    if (file3 != null && file3.FileName != "")
                    {
                        string[] getExtenstion = file3.FileName.Split('.');
                        string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                        string FileNameForInsert = file3.FileName.Replace("." + oExtension, "");

                        var filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + FileNameForInsert + "-3." + oExtension;
                        file3.SaveAs(filePath);

                        _aPI_BLL.InsertUpdateNonQuery(" INSERT INTO FU (FUId, LnNo, Record1Id, FormName, FilePath, FileName, InsertedOn, " +
                                                  " LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values (" +
                                                  " NEWID(), 3, '" + ComplaintId.ToString() + "', 'ComplaintApp', '" + filePath.ToString() + "' " +
                                                  " , '" + FileNameForInsert + "-3." + oExtension + "', GETDATE(), GETDATE(), '" + GeneratedByUserId.ToString() + "' " +
                                                  " , '" + GeneratedByUserId.ToString() + "' " +
                                                  ") ");
                    }

                    if (file4 != null && file4.FileName != "")
                    {
                        string[] getExtenstion = file4.FileName.Split('.');
                        string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                        string FileNameForInsert = file4.FileName.Replace("." + oExtension, "");

                        var filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + FileNameForInsert + "-4." + oExtension;
                        file4.SaveAs(filePath);

                        _aPI_BLL.InsertUpdateNonQuery(" INSERT INTO FU (FUId, LnNo, Record1Id, FormName, FilePath, FileName, InsertedOn, " +
                                                  " LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values (" +
                                                  " NEWID(), 4, '" + ComplaintId.ToString() + "', 'ComplaintApp', '" + filePath.ToString() + "' " +
                                                  " , '" + FileNameForInsert + "-4." + oExtension + "', GETDATE(), GETDATE(), '" + GeneratedByUserId.ToString() + "' " +
                                                  " , '" + GeneratedByUserId.ToString() + "' " +
                                                  ") ");
                    }
                }
            }

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
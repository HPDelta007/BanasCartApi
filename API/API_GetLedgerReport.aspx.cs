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


public partial class API_GetLedgerReport : System.Web.UI.Page
{
    string LgrCode;
    string LgrGrpCode;
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
                    LgrCode = Request.Form["LgrCode"].ToString();
                else
                    LgrCode = null;

                if (Request.Form["LgrGrpCode"] != null && Request.Form["LgrGrpCode"] != "")
                    LgrGrpCode = Request.Form["LgrGrpCode"].ToString();
                else
                    LgrGrpCode = null;

                if (Request.Form["FromDate"] != null && Request.Form["FromDate"] != "")
                {
                    FromDate = Convert.ToDateTime(Request.Form["FromDate"]);
                }
                else
                {
                    FromDate = null;
                }

                if (Request.Form["ToDate"] != null && Request.Form["ToDate"] != "")
                {
                    ToDate = Convert.ToDateTime(Request.Form["ToDate"]);
                }
                else
                {
                    ToDate = null;
                }

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
        string where = "";
        string where1 = "";
        try
        {
            if (LgrCode != null)
            {
                where += " AND TRAN2.MASTERCODE1 = '" + LgrCode + "'";

                where1 += " AND f.MasterCode = '" + LgrCode + "'";
            }

            if (LgrGrpCode != null)
            {
                where += " and tran1.Mastercode2 = '" + LgrGrpCode + "'";
            }

            //da = _aPI_BLL.returnDataTable(" SELECT   TRAN2.TranType,TRAN2.RECONSTATUS,TRAN2.C1,(DATENAME(dd,TRAN2.DATE) + '-' + LEFT(DATENAME(MM,TRAN2.DATE),3) + '-' + DATENAME(YYYY,TRAN2.DATE)) as DATE " +
            //                 " ,TRAN2.B1 " +
            //                 " ,(Select top 1 replace(replace((Select Top 1 NameAlias from Help1 as H1 where H1.NameOrAlias = 1 and H1.Code = t21.mastercode1), ']', ')'), '[', '(') from Tran2 as t21 where t21.vchcode = tran2.vchcode and t21.rectype = 1 and  t21.value1 < 0 and t21.mastercode1 <> 19126 and t21.b1=tran2.b1 order by t21.srno) As DebitMast " +
            //                 " , (Select top 1 replace(replace((Select Top 1 NameAlias from Help1 as H1 where H1.NameOrAlias = 1 and H1.Code = t22.mastercode1), ']', ')'), '[', '(') from Tran2 as t22 where t22.vchcode = tran2.vchcode and t22.rectype = 1 and t22.value1 > 0 and t22.mastercode1 <> 19126 and t22.b1=tran2.b1 order by t22.srno) AS CreditMast " +
            //                 " , (Select top 1 shortnar from Tran2 as t2 where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 > 0 and t2.mastercode1 <> 19126 order by t2.srno) AS CrAccNar  " +
            //                 " , (Select top 1 shortnar from Tran2 as t2 where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 < 0 and t2.mastercode1 <> 19126 order by t2.srno) AS DrAccNar   " +
            //                 " ,TRAN2.VCHTYPE,TRAN2.VCHNO,TRAN2.VCHCODE,TRAN2.VALUE1,TRAN2.VCHSERIESCODE " +
            //                 " ,TRAN2.SHORTNAR,TRAN2.MASTERCODE1,TRAN2.SRNO " +
            //                 " ,(Select MasterCode1 from tran1 as t1 where t1.vchcode = tran2.vchcode ) AS PartyCode " +
            //                 " ,Vch.Narration1,Vch.Narration2,vch.purchasebillno,vch.purchasebilldate, MM.Name as 'Division'  " +
            //                 " FROM TRAN2  " +
            //                 " Left JOIN tran1 on Tran2.VchType = tran1.VchType and Tran2.VchCode = tran1.VchCode " +
            //                 " Left Join VchOtherInfo as Vch on Vch.Vchcode = Tran2.VchCode  " +
            //                 " Left join Master1 MM on tran1.Mastercode2 = MM.Code " +
            //                 " WHERE RECTYPE = 1 " +
            //    //" AND MASTERCODE1 ='" + LgrCode + "'" +
            //                 " AND TRAN2.MASTERCODE2 = 0   " +
            //                 " AND TRAN2.DATE >= '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "'" +
            //                 " AND TRAN2.DATE <= '" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "'" +
            //                 where +
            //                 " union all " +
            //                 " select 0 as TranType, 0 as RECONSTATUS, '' as C1, CONVERT(date, '01-Apr-2020') as DATE, 0 as B1, replace(replace(replace(m.Name, '\"', 'U+0022'), ']', ')'), '[', '(') as DebitMast, replace(replace(replace(m.Name, '\"', 'U+0022'), ']', ')'), '[', '(') as CreditMast, '' as CrAccNar " +
            //                 " , '' as DrAccNar, '' as VCHTYPE, 'OPENING BALANCE' as VCHNO, '' as VCHCODE, sum(d1), '', '', f.MasterCode, '0' as SRNO, f.MasterCode, '', '', '', '', '' from Folio1 f  " +
            //                 " inner join Debtors m on m.Code = f.MasterCode where 1=1 " + where1 + " " +
            //                 " group by replace(m.Name, '\"', 'U+0022'), f.MasterCode " +
            //                 " order by  DebitMast, SRNO, DATE " +
            //                 "  "); // ORDER BY TRAN2.DATE,TRAN2.VCHTYPE,TRAN2.VCHNO,TRAN2.VCHCODE,TRAN2.SRNO


            da = _aPI_BLL.returnDataTable(" SELECT Tran2.I2,Tran2.C2,TRAN2.TranType,TRAN2.RECONSTATUS,TRAN2.C1,TRAN2.DATE,TRAN2.B1, " +
                                          " (Select top 1 (Select Top 1 replace(NameAlias, '\"', 'U+0022') NameAlias from Help1 as H1 where H1.NameOrAlias = 1 and H1.Code = t21.mastercode1) from Tran2 as t21 where t21.vchcode = tran2.vchcode and t21.rectype = 1 and   " +
                                          " t21.value1 < 0 and t21.mastercode1 <> '" + LgrCode + "' and t21.b1=tran2.b1 order by t21.srno) As DebitMast,  " +
                                          " (Select top 1 (Select Top 1 replace(NameAlias, '\"', 'U+0022') NameAlias from Help1 as H1 where H1.NameOrAlias = 1 and H1.Code = t22.mastercode1)  " +
                                          " from Tran2 as t22 where t22.vchcode = tran2.vchcode and t22.rectype = 1 and t22.value1 > 0 and t22.mastercode1 <> '" + LgrCode + "' and t22.b1=tran2.b1 order by t22.srno) AS CreditMast,  " +
                                          " (Select top 1 shortnar " +
                                          " from Tran2 as t2 where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 > 0 and t2.mastercode1 <> '" + LgrCode + "' order by t2.srno) AS CrAccNar , (Select top 1 shortnar from Tran2 as t2  " +
                                          " where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 < 0 and t2.mastercode1 <> '" + LgrCode + "' order by t2.srno) AS DrAccNar, (Select top 1 I2 from Tran2 as t2  " +
                                          " where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 > 0 and t2.mastercode1 <> '" + LgrCode + "' order by t2.srno) AS CrInstType , (Select top 1 I2 from Tran2 as t2  " +
                                          " where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 < 0 and t2.mastercode1 <> '" + LgrCode + "' order by t2.srno) AS DrInstType, (Select top 1 C2 from Tran2 as t2  " +
                                          " where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 > 0 and t2.mastercode1 <> '" + LgrCode + "' order by t2.srno) AS CrInstNo , (Select top 1 C2 from Tran2 as t2  " +
                                          " where t2.vchcode = tran2.vchcode and t2.rectype = 1 and t2.value1 < 0 and t2.mastercode1 <> '" + LgrCode + "' order by t2.srno) AS DrInstNo,(Select top 1 t21.Srno from Tran2 as t21 where t21.vchcode = tran2.vchcode  " +
                                          " and t21.rectype = 1 and  t21.value1 < 0 and t21.mastercode1 <> '" + LgrCode + "' and t21.b1=tran2.b1 order by t21.srno) As DebitMastSrNo, (Select top 1 t22.SrNo from Tran2 as t22 where t22.vchcode = tran2.vchcode  " +
                                          " and t22.rectype = 1 and t22.value1 > 0 and t22.mastercode1 <> '" + LgrCode + "' and t22.b1=tran2.b1 order by t22.srno) AS CreditMastSrno  " +
                                          " ,TRAN2.VCHTYPE,TRAN2.VCHNO,TRAN2.VCHCODE,TRAN2.VALUE1,TRAN2.VCHSERIESCODE,TRAN2.SHORTNAR,TRAN2.MASTERCODE1,TRAN2.SRNO, " +
                                          " (Select MasterCode1 from tran1 as t1 where t1.vchcode = tran2.vchcode ) AS PartyCode,Vch.Narration1,Vch.Narration2,vch.purchasebillno,vch.purchasebilldate FROM TRAN2  " +
                                          " Left Join VchOtherInfo as Vch on Vch.Vchcode = Tran2.VchCode WHERE RECTYPE = 1  " +
                                          " AND MASTERCODE1 = '" + LgrCode + "'  " +
                                          " AND MASTERCODE2 = 0  " +
                                          "  AND DATE >= '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' AND DATE <= '" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "'  " +
                                          ((LgrGrpCode == null) ? "" : " AND Tran2.VchCode IN (SELECT VCHCODE FROM TRAN1 WHERE VCHSERIESCODE IN (SELECT MASTERCODE2 FROM TRAN10 WHERE MASTERCODE1 = '" + LgrGrpCode + "' AND RECTYPE =7)) ") +
                                          " union all " +
                                          " select NULL AS I2, NULL AS  C2, NULL AS  TranType, NULL AS  RECONSTATUS, NULL AS  C1, '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' AS  DATE, NULL AS  B1, " +
                                          " replace(m.Name, '\"', 'U+0022') AS  DebitMast, replace(m.Name, '\"', 'U+0022') AS  CreditMast, NULL AS  CrAccNar, NULL AS  DrAccNar, NULL AS  CrInstType, NULL AS  DrInstType, NULL AS  CrInstNo, NULL AS  DrInstNo,  " +
                                          " NULL AS  DebitMastSrNo, NULL AS  CreditMastSrno, NULL AS  VCHTYPE, 'OPENING' AS  VCHNO, NULL AS  VCHCODE,  Sum(o.D1) VALUE1, NULL AS  VCHSERIESCODE, NULL AS  SHORTNAR,  " +
                                          " MASTERCODE1 AS  MASTERCODE1, NULL AS  SRNO, NULL AS  PartyCode, NULL AS  Narration1, NULL AS  Narration2, 'OPENING'  AS  purchasebillno, NULL AS  purchasebilldate     from ( " +
                                          " SELECT isnull(D1,0) as D1, MASTERCODE1 FROM TRAN4 WHERE MASTERCODE1='" + LgrCode + "'   AND RECTYPE = 2  " +
                                          ((LgrGrpCode == null) ? "" : " AND BRANCHCODE in ( select Code from Master1 where CM2 = '" + LgrGrpCode + "' and MasterType=24)     ") +
                                          " union " +
                                          " SELECT isnull(sum(VALUE1),0)  AS BaseTransBal, MASTERCODE1  FROM TRAN2 WHERE MASTERCODE1 = '" + LgrCode + "'  " +
                                          ((LgrGrpCode == null) ? "" : " AND VCHSERIESCODE IN (SELECT MASTERCODE2 FROM TRAN10 WHERE MASTERCODE1 = '" + LgrGrpCode + "' AND RECTYPE =7) ") +
                                          " AND RECTYPE = 1 And Date<= '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' GROUP BY MASTERCODE1 " +
                                          " ) as o " +
                                          " inner join Master1 m on m.Code = o.MASTERCODE1 " +
                                          " GROUP BY o.MASTERCODE1, m.Name " +
                                          " ORDER BY TRAN2.DATE,TRAN2.VCHTYPE,TRAN2.VCHNO,TRAN2.VCHCODE,TRAN2.SRNO");

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
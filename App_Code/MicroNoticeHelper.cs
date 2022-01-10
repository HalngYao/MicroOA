using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;


namespace MicroNoticeHelper
{
    /// <summary>
    /// MicroNoticeHelper 的摘要说明
    /// </summary>
    public class MicroNotice
    {

        /// <summary>
        /// 获取通知内容，传入参数构造通知内容
        /// </summary>
        /// <returns></returns>
        public static string GetNoticeContent(string ContentType, string ShortTableName, string ModuleID, string FormID, string FormsID, string FormNumber, string FormName, string NodeNames, string NodeName, string DisplayName, string Applicant, string Note)
        {
            string flag = string.Empty, Href = string.Empty, Href2 = string.Empty, Href3 = string.Empty;
            string br = "&lt;br&gt;&lt;br&gt;";  //&lt;br&gt;=<br>

            Note = string.IsNullOrEmpty(Note) ? "" : "备注/Note：" + Note.toJsonTrim();

            Href = Note + "{0}" + br;
            Href2 = Note + "{1}" + br;
            Href3 = Note + "{2}";

            string FormName1 = "《" + FormName + "》";
            string FormName2 = "「" + FormName + "」";
            string FormName3 = "" + FormName + "";

            switch (ContentType)
            {
                case "ApplicantToApproval":  //申请者提交申请后通知审批者
                    flag = Applicant + "，提交了 " + FormName1 + "申请，编号：" + FormNumber + "，拜托您进行审批。" + Href;

                    flag += Applicant + ", submitted " + FormName3 + "application No.: " + FormNumber + ", please approve. " + Href3;
                    break;

                case "ApplicantReToApproval":  //申请者重新提交申请后通知审批者【重新提交】
                    flag = Applicant + "，重新提交了 " + FormName1 + "申请，编号：" + FormNumber + "，拜托您进行审批。" + Href;
                    //flag += Applicant + "は " + FormName2 + "，番号：" + FormNumber + " ，を改めて提出しました。承認をお願いします。" + Href2;
                    flag += Applicant + ", re submitted " + FormName3 + "application No.: " + FormNumber + ", please approve. " + Href3;
                    break;

                case "ApplicantAutoAccept":  //申请者提交申请且系统自动受理并通知受理者【非审批表单系统自动受理】
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，受理成功【系统自动受理】请您尽快对应（处理）。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "，番号：" + FormNumber + "，は受付されました【システム自動受付】。引き続きの対応をお願いします。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.: " + FormNumber + ", accepted successfully [system automatic acceptance] please deal with it as soon as possible. " + Href3;
                    break;

                case "ApplicantToAccept":  //申请成功到受理（受理者）注：不需要审批
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，拜托您尽快进行对应（处理）。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "，番号：" + FormNumber + "，引き続きの対応をお願いします。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.: " + FormNumber + ", please deal with it as soon as possible. " + Href3;
                    break;

                case "ApplicantToProcess":   //申请成功到处理（处理者）注：不需要审批 
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，拜托您尽快进行对应（处理）。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "，番号：" + FormNumber + "，引き続きの対応をお願いします。" + Href2;
                    flag += Applicant + " submitted  " + FormName3 + "application, No.: " + FormNumber + ", please deal with it as soon as possible. " + Href3;
                    break;

                case "ApprovalToApplicant":  //审批通过通知申请者
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已由 " + NodeNames + DisplayName + "审批通过。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "番号：" + FormNumber + "，は " + NodeNames + DisplayName + "に承認されました。" + Href2;
                    flag += "Your submitted " + FormName3 + "application No.: " + FormNumber + ", which has been approved by " + NodeNames + DisplayName + "." + Href3;
                    break;

                case "ApprovalToApproval":  //审批通过通知下一位审批者
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已由 " + NodeNames + DisplayName + " 审批通过，拜托您进行审批。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "番号：" + FormNumber + "，は " + NodeNames + DisplayName + " に承認されました。引き続きの承認をお願いします。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.: " + FormNumber + ", has been approved by " + NodeNames + DisplayName + ". Please approve. " + Href3;
                    break;

                case "ApprovalToProcess":  //所有节点审批完成通知受理者
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已由 " + NodeNames + DisplayName + "审批通过，拜托您进行对应（处理）。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "，番号：" + FormNumber + "，はアクセス先の " + NodeNames + DisplayName + "に承認されました。引き続きの承認をお願いします。" + Href2;
                    flag += Applicant + "submitted " + FormName3 + "application, No.:" + FormNumber + ", has been approved by " + NodeNames + DisplayName + ". Please deal with it. " + Href3;
                    break;

                case "AcceptAutoApplicant":  //系统自动受理成功通知申请者【系统自动受理】
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被受理【系统自动受理】正在为您安排对应（处理），请耐心等待。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "，番号：" + FormNumber + "，は受付されて処理中です【システム自動受付】。少々お待ちください。" + Href2;
                    flag += "Your submitted " + FormName3 + "application, No.: " + FormNumber + ", has been accepted[system automatic acceptance] is arranging corresponding (processing) for you, please wait patiently. " + Href3;
                    break;

                case "AcceptToApplicant":  //受理者手动受理成功通知申请者
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被受理正在为您安排对应（处理），请耐心等待。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "，番号：" + FormNumber + "，は受付されて処理中です。少々お待ちください。" + Href2;
                    flag += "Your submitted " + FormName3 + " application, No.: " + FormNumber + ", has been accepted, is arranging corresponding (processing) for you, please wait patiently. " + Href3;
                    break;

                case "AcceptAutoProcess":  //受理了通知受理者（处理者）【系统自动受理】
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，受理成功【系统自动受理】请您尽快对应（处理）。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "番号：" + FormNumber + "，は受付されました【システム自動受付】。引き続きの対応をお願いします。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.: " + FormNumber + ", accepted successfully [system automatic acceptance] please deal with it as soon as possible. " + Href3;
                    break;

                case "AcceptToProcess":  //受理者手动受理成功通知对应者（受理者）
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已受理成功，拜托您尽快进行对应（处理）。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "，番号：" + FormNumber + "，は受付されました。引き続きの対応をお願いします。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.: " + FormNumber + ", has been accepted successfully, please deal with it as soon as possible. " + Href3;
                    break;

                case "ClosedAutoApplicant":  //结案了通知申请者【系统自动受理自动结案】
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已受理和对应（处理）完成【系统自动结案】。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "申請，番号：" + FormNumber + "，は既に受付され且つ対応（処理）済です【システム自動閉じる】。" + Href2;
                    flag += "Your submitted " + FormName3 + "application, No.: " + FormNumber + ", accepted and corresponding(processing) completed[system automatic closing]. " + Href3;
                    break;

                case "ClosedAutoApplicant2":  //审批完成不需要受理也不需要对应的情况下结案了通知申请者【系统自动结案】
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已对应（处理）完成【系统自动结案】。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "申請，番号：" + FormNumber + "，既に対応（処理）済です【システム自動閉じる】。" + Href2;
                    flag += "Your submitted " + FormName3 + "application, No.:" + FormNumber + ", corresponding (processing) has been completed [system automatic closing]. " + Href3;
                    break;

                case "ClosedToApplicant":  //结案通知申请者
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，因为已经对应（处理）完成，所以进行结案。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "，番号：" + FormNumber + "，は処理済みなので、本案件を閉じます。" + Href2;
                    flag += "Your submitted " + FormName3 + "application, No.: " + FormNumber + ", because the corresponding (processing) has been completed, so the case is closed. " + Href3;
                    break;

                case "ClosedAutoAccept":  //审批完成不需要受理也不需要对应的情况下结案了通知受理者【系统自动结案】
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，审批已通过并且对应（处理）完成【系统自动结案】。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "申請，番号：" + FormNumber + "，は承認され且つ対応（处理）済です【システム自動閉じる】。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.: " + FormNumber + ", the approval has been passed and the corresponding(processing) has been completed[system automatic closing]. " + Href3;
                    break;

                case "ClosedAutoProcess":  //结案了通知对应者【系统自动结案】
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已受理并且对应（处理）完成【系统自动结案】。" + Href;
                    //flag += Applicant + "が提出した " + FormName2 + "申請，番号：" + FormNumber + "，は既に受付され且つ対応（処理）済です【システム自動閉じる】。" + Href2;
                    flag += Applicant + " submitted " + FormName3 + "application, No.:" + FormNumber + ", has been accepted and corresponding (processing) completed [system automatic closure]." + Href3;
                    break;

                case "RejectedToApplicant":  //结案通知申请者
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被 " + NodeName + DisplayName + " 驳回。" + Href;
                    //flag += "ご提出いただいた " + FormName2 + "，番号：" + FormNumber + "，は" + NodeName + DisplayName + "に却下されました。" + Href2;
                    flag += "Your submitted " + FormName3 + "application, No.:" + FormNumber + ", has been rejected by" + NodeName + DisplayName + " for the reason of " + Href3;
                    break;


                case "ApplicantWithdrawalToApplicant":  //申请者撤回通知自己
                    flag = "您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已撤回成功，如有已审批完毕的结果将失效，您可以修改表单内容或删除表单，如果需要继续请重新提交表单。" + Href;
                    //flag += "ご提出頂いた " + FormName1 + "の申請，番号：" + FormNumber + "は，取り消されました，承認済みの結果は無効になりますが、申請表での内容修正、削除することができます、続くの場合は改めて申請表を提出してください。" + Href2;
                    flag += "The " + FormName1 + " application No. " + FormNumber + " you submitted has been withdrawn successfully. If the approved result is invalid, you can modify the form content or delete the form. If you need to continue, please submit the form again." + Href3;
                    break;

                case "ApplicantWithdrawalToApproval":  //申请者撤回通知审批者(已审批的审批者)
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被本人撤回成功（在" + NodeName + "审批节点），已审批完毕的结果将失效。" + Href;
                    //flag += Applicant + "ご提出頂いた" + FormName1 + "の申請，番号：" + FormNumber + "は，本人に取り消されました（" + NodeName + "承認ノードで），承認済の結果は無効になります。" + Href2;
                    flag += Applicant + "submitted" + FormName1 + "application, No.:" + FormNumber + ", which has been successfully withdrawn by me(at the "+ NodeName + " approval node), and the approved result will be invalid." + Href3;
                    break;

                case "ApplicantWithdrawalToNextApproval":  //申请者撤回通知下一位审批者
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被本人撤回成功，该表单暂时不需要您审批或处理对应请知悉，谢谢！" + Href;
                    //flag += Applicant + "ご提出頂いた " + FormName1 + "の申請，番号：" + FormNumber + "，本人に取り消されました，当該申請は暫くあなたの承認或は対応処理の必要はありません。" + Href2;
                    flag+= Applicant + "submitted" + FormName1 + "application, No.:" + FormNumber + ", has been successfully withdrawn by me, this form does not need your approval or processing right now, please know, thank you! " + Href3;
                     break;

                case "ApprovalWithdrawalToApproval":  //审批者撤回通知审批者自己
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被您撤回成功，已审批完毕的结果将失效，您可以重新审批该表单或驳回表单。" + Href;
                    //flag += Applicant + "ご提出頂いた " + FormName1 + "の申請，番号：" + FormNumber + "は，取り消し成功です，承認済の結果は無効になります，申請を再承認或は却下することができます。" + Href2;
                    flag += Applicant + "submitted" + FormName1 + "application, No.:" + FormNumber + " has been successfully withdrawn by you. The approved result will be invalid. You can re approve the form or reject it. " + Href3;
                    break;

                case "ApprovalWithdrawalToNextApproval":  //审批者撤回通知下一位审批者
                    flag = Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被 " + NodeName + DisplayName + " 撤回成功，该表单暂时不需要您审批请知悉，谢谢！" + Href;
                    //flag += Applicant + "ご提出頂いた " + FormName1 + "の申請，番号：" + FormNumber + "，は" + NodeName + DisplayName + "に取り消されました ，当該申請表は暫く承認の必要はありません、ご了承ください。" + Href2;
                    flag += Applicant + "submitted" + FormName1 + "application, No.:" + FormNumber + ", has been successfully withdrawn by " + NodeName + DisplayName + ", this form does not need your approval for the time being, please know, thank you!" + Href3;
                    break;


            }

            return flag;

        }


        /// <summary>
        /// 在提交或审批表单里把记录插入通知列表
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="NoticeType">通知类型来源表单名称，如：IT依赖书</param>
        /// <param name="Title">通知主题</param>
        /// <param name="IsRead">已读</param>
        /// <param name="UIDs">接收通知的ID（可以是ID逗号分隔字符串）</param>
        /// <returns></returns>
        public static Boolean SetNotice(string ShortTableName, string ModuleID, string FormID, string FormsID, string FormNumber, string NoticeType, string Title, string Content, Boolean IsRead, string UIDs)
        {
            Boolean flag = false;

            if (!string.IsNullOrEmpty(UIDs))
            {
                string[] UIDArray = UIDs.Split(',');

                if (UIDArray.Length > 0 && UIDs != "0")
                {
                    string URL = "/Views/Forms/MicroFormList/View";

                    DataTable _dt = MicroDataTable.GetDataTable("use");
                    string SystemTitle = MicroPublic.GetMicroInfo("Title");

                    string ListParms = string.Empty;

                    DataTable _dtForms = MicroDataTable.GetDataTable("Forms");
                    if (_dtForms != null && _dtForms.Rows.Count > 0)
                    {
                        ListParms = _dtForms.Select("FormID=" + FormID)[0]["ListParms"].toStringTrim();
                        if (!string.IsNullOrEmpty(ListParms))
                            ListParms = "/" + ListParms;
                    }

                    string HttpDomain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                    string Href = " <a href=\"" + HttpDomain + URL + "/" + ShortTableName + "/" + ModuleID + "/" + FormID + "/" + FormsID + "/" + FormNumber + ListParms + "\">查看/View</a>";
                    string Href2 = " <a href=\"" + HttpDomain + URL + "/" + ShortTableName + "/" + ModuleID + "/" + FormID + "/" + FormsID + "/" + FormNumber + ListParms + "\">View（クリック）</a>";
                    string Href3 = " <a href=\"" + HttpDomain + URL + "/" + ShortTableName + "/" + ModuleID + "/" + FormID + "/" + FormsID + "/" + FormNumber + ListParms + "\">View</a>";

                    string Content2 = string.Empty, Content3 = string.Empty;

                    if (!string.IsNullOrEmpty(Content))
                    {
                        //加入了换行符（&lt;br&gt;）写入数据库，在LayuiTable时显示Tips的换行
                        Content2 = string.Format(Content, "", "", "");

                        //用于发送邮件，由于邮件内容接受到&lt;br&gt;时会转换为<br>直接输出起不到换行作用，所以在这里重新替换为<br/>后再输出
                        Content3 = string.Format(Content, Href, Href2, Href3).Replace("&lt;br&gt;", "<br/>");
                    }

                    //要插入数据的字段
                    DataTable _dtInsert = new DataTable();
                    _dtInsert.Columns.Add("ShortTableName", typeof(string));
                    _dtInsert.Columns.Add("ModuleID", typeof(int));
                    _dtInsert.Columns.Add("FormID", typeof(int));
                    _dtInsert.Columns.Add("FormsID", typeof(int));
                    _dtInsert.Columns.Add("FormNumber", typeof(string));
                    _dtInsert.Columns.Add("NoticeType", typeof(string));
                    _dtInsert.Columns.Add("Title", typeof(string));
                    _dtInsert.Columns.Add("Content", typeof(string));
                    _dtInsert.Columns.Add("IsRead", typeof(Boolean));
                    _dtInsert.Columns.Add("UID", typeof(int));

                    //HttpContext.Current.Response.Write(UIDs);
                    //HttpContext.Current.Response.End();

                    for (int i = 0; i < UIDArray.Length; i++)
                    {
                        int UID = UIDArray[i].toInt();

                        //得到接收通知的UID
                        DataRow[] _rows = _dt.Select("UID = " + UID);
                        if (_rows.Length > 0)
                        {
                            Boolean IsSiteTips = _rows[0]["IsSiteTips"].toBoolean();
                            Boolean IsMailTips = _rows[0]["IsMailTips"].toBoolean();
                            string EMail = _rows[0]["EMail"].toJsonTrim();

                            //IsGlobalTips  //全局提示
                            //IsSiteTips    //站内消息
                            //IsMailTips    //邮件通知

                            //启用站内消息
                            if (IsSiteTips)
                            {
                                DataRow _dr = _dtInsert.NewRow();

                                _dr["ShortTableName"] = ShortTableName;
                                _dr["ModuleID"] = ModuleID.toInt();
                                _dr["FormID"] = FormID.toInt();
                                _dr["FormsID"] = FormsID.toInt();
                                _dr["FormNumber"] = FormNumber;
                                _dr["NoticeType"] = NoticeType;
                                _dr["Title"] = Title;
                                _dr["Content"] = Content2;
                                _dr["IsRead"] = IsRead;
                                _dr["UID"] = UIDArray[i].toInt();
                                _dtInsert.Rows.Add(_dr);
                            }

                            //启用邮件通知
                            if (IsMailTips && !string.IsNullOrEmpty(EMail))
                                MicroPublic.SendMail(EMail, "", "【" + SystemTitle + "】" + Title, Content3);
                        }

                    }

                    //当有行数时发出站内通知
                    if (_dtInsert.Rows.Count > 0)
                        flag = MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "Notice");

                }
            }

            return flag;

        }
    }
}
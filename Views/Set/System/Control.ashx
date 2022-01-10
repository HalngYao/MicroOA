<%@ WebHandler Language="C#" Class="Control" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;

public class Control : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

        string flag = MicroPublic.GetMsg("SaveFailedTry"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (Action.toStringTrim().ToLower() == "modify")
            flag = CtrlModify(MID.Trim(), Fields.toStringTrim());

        context.Response.Write(flag);
    }

    protected string CtrlModify(string MID, string Fields)
    {
        string flag = MicroPublic.GetMsg("SaveFailedTry"), selTabColName = string.Empty, Invalid = string.Empty, ctlPrimaryKey = string.Empty, ctlFormStyle = string.Empty, ctlDisplayAsterisk = string.Empty, ctlAddDisplay = string.Empty, ctlAddDisplayButton = string.Empty, ctlSaveDraftButton = string.Empty, ctlModifyDisplay = string.Empty, ctlModifyDisplayButton = string.Empty, ctlViewDisplay = string.Empty, ctlViewDisplayLabel = string.Empty, ctlAfterDisplay = string.Empty, ctlTitle = string.Empty, ctlTitleStyle = string.Empty, ctlTypes = string.Empty, ctlPrefix = string.Empty, ctlStyle = string.Empty, ctlPlaceholder = string.Empty, ctlCheckboxSkin = string.Empty, ctlSwitchText = string.Empty, ctlDescription = string.Empty, ctlDescriptionDisplayPosition = string.Empty, ctlDescriptionDisplayMode = string.Empty, ctlDescriptionStyle = string.Empty, ctlValue = string.Empty, ctlDefaultValue = string.Empty, ctlCharLength = string.Empty, ctlReceiveName = string.Empty, ctlSourceTable = string.Empty, ctlTextName = string.Empty, ctlTextValue = string.Empty, ctlExtendJSCode = string.Empty,  ctlExtraFunction = string.Empty, ctlFilter = string.Empty, ctlVerify = string.Empty, ctlVerify2 = string.Empty, ctlVerifyCustomFunction = string.Empty, ctlGroup = string.Empty, ctlGroupDescription = string.Empty, ctlFormItemStyle = string.Empty, ctlInlineCss = string.Empty, ctlInlineStyle = string.Empty, ctlColSpace = string.Empty, ctlOffset = string.Empty, ctlXS = string.Empty, ctlSM = string.Empty, ctlMD = string.Empty, ctlLG = string.Empty, ctlInputInline = string.Empty, ctlInputInlineStyle = string.Empty, CustomHtmlCode = string.Empty;

        //用于生成Layui数据表时显示自定义字段，如用户所属部门保存的是用户ID，想要显示的是部门名称而该值正是这个要显示的字段
        //该值与“ctlSourceTable”“ctlTextName”“ctlTextValue”相配合使用，这三个值不为空时该字段默认赋值为“ctlTextName”
        string colCustomField = string.Empty;


        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;

            selTabColName = json["selTabColName"];
            Invalid = json["Invalid"];
            ctlPrimaryKey = json["ctlPrimaryKey"];
            ctlFormStyle = json["ctlFormStyle"];
            ctlDisplayAsterisk = json["ctlDisplayAsterisk"];
            ctlAddDisplay = json["ctlAddDisplay"];
            ctlAddDisplayButton = json["ctlAddDisplayButton"];
            ctlSaveDraftButton = json["ctlSaveDraftButton"];
            ctlModifyDisplay = json["ctlModifyDisplay"];
            ctlModifyDisplayButton = json["ctlModifyDisplayButton"];
            ctlViewDisplay = json["ctlViewDisplay"];
            ctlViewDisplayLabel = json["ctlViewDisplayLabel"];
            ctlAfterDisplay = json["ctlAfterDisplay"];
            ctlTitle = json["ctlTitle"];
            ctlTitleStyle = json["ctlTitleStyle"];
            ctlTypes = json["ctlTypes"];
            ctlPrefix = json["ctlPrefix"];
            ctlStyle = json["ctlStyle"];
            ctlPlaceholder = json["ctlPlaceholder"];
            ctlCheckboxSkin = json["ctlCheckboxSkin"];
            ctlSwitchText = json["ctlSwitchText"];
            ctlDescription = json["ctlDescription"];
            ctlDescriptionDisplayPosition = json["ctlDescriptionDisplayPosition"];
            ctlDescriptionDisplayMode = json["ctlDescriptionDisplayMode"];
            ctlDescriptionStyle = json["ctlDescriptionStyle"];
            ctlValue = json["ctlValue"];
            ctlDefaultValue = json["ctlDefaultValue"];
            ctlExtraFunction = json["ctlExtraFunction"]; //20201103新增
            ctlReceiveName = json["ctlReceiveName"];
            ctlSourceTable = json["ctlSourceTable"];
            ctlTextName = json["ctlTextName"];
            ctlTextValue = json["ctlTextValue"];
            ctlExtendJSCode = json["ctlExtendJSCode"];
            ctlCharLength = json["ctlCharLength"];
            ctlFilter = json["ctlFilter"];
            ctlVerify = json["ctlVerify"];
            ctlVerify2 = json["ctlVerify2"];
            ctlVerifyCustomFunction = json["ctlVerifyCustomFunction"];
            ctlGroup = json["ctlGroup"];
            ctlGroupDescription = json["ctlGroupDescription"];
            ctlFormItemStyle = json["ctlFormItemStyle"];
            ctlInlineCss = json["ctlInlineCss"];
            ctlInlineStyle = json["ctlInlineStyle"];
            ctlColSpace = json["ctlColSpace"];
            ctlOffset = json["ctlOffset"];
            ctlXS = json["ctlXS"];
            ctlSM = json["ctlSM"];
            ctlMD = json["ctlMD"];
            ctlLG = json["ctlLG"];
            ctlInputInline = json["ctlInputInline"];
            ctlInputInlineStyle = json["ctlInputInlineStyle"];
            CustomHtmlCode = json["CustomHtmlCode"];

            if (!string.IsNullOrEmpty(ctlSourceTable.toStringTrim()) && !string.IsNullOrEmpty(ctlTextName.toStringTrim()) && !string.IsNullOrEmpty(ctlTextValue.toStringTrim()))
            {
                //根据提交的TID查询当前提交的TabColName字段是不是ParentID，如果是的话把自定义字段“colCustomField”设置为“ParentID”，主要是用于LayuiDatatable表
                string _sql0 = "select TabColName from MicroTables where TID=@selTabColName";
                SqlParameter[] _sp0 = { new SqlParameter("@selTabColName", SqlDbType.Int) };
                _sp0[0].Value = selTabColName.toInt();
                string TabColName = MicroPublic.GetSingleField(0, _sql0, _sp0);

                colCustomField = TabColName.ToLower() == "parentid" ? "ParentID" : ctlTextName;
            }

            string _sql = "update MicroTables set Invalid = @Invalid, ctlPrimaryKey = @ctlPrimaryKey, ctlFormStyle = @ctlFormStyle, ctlDisplayAsterisk = @ctlDisplayAsterisk, ctlAddDisplay = @ctlAddDisplay, ctlAddDisplayButton = @ctlAddDisplayButton, ctlSaveDraftButton = @ctlSaveDraftButton, ctlModifyDisplay = @ctlModifyDisplay, ctlModifyDisplayButton = @ctlModifyDisplayButton, ctlViewDisplay = @ctlViewDisplay, ctlViewDisplayLabel = @ctlViewDisplayLabel, ctlAfterDisplay = @ctlAfterDisplay, ctlTitle = @ctlTitle, ctlTitleStyle = @ctlTitleStyle, ctlTypes = @ctlTypes, ctlPrefix = @ctlPrefix, ctlStyle = @ctlStyle, ctlPlaceholder = @ctlPlaceholder, ctlCheckboxSkin = @ctlCheckboxSkin, ctlSwitchText = @ctlSwitchText, ctlDescription = @ctlDescription, ctlDescriptionDisplayPosition = @ctlDescriptionDisplayPosition, ctlDescriptionDisplayMode = @ctlDescriptionDisplayMode, ctlDescriptionStyle = @ctlDescriptionStyle, ctlValue = @ctlValue, ctlDefaultValue = @ctlDefaultValue, ctlExtraFunction= @ctlExtraFunction, ctlReceiveName = @ctlReceiveName, ctlSourceTable = @ctlSourceTable, ctlTextName = @ctlTextName, ctlTextValue = @ctlTextValue, ctlExtendJSCode = @ctlExtendJSCode, ctlCharLength = @ctlCharLength, ctlFilter = @ctlFilter, ctlVerify = @ctlVerify, ctlVerify2 = @ctlVerify2, ctlVerifyCustomFunction = @ctlVerifyCustomFunction, ctlGroup = @ctlGroup, ctlGroupDescription = @ctlGroupDescription, ctlFormItemStyle = @ctlFormItemStyle, ctlInlineCss = @ctlInlineCss, ctlInlineStyle = @ctlInlineStyle, ctlColSpace = @ctlColSpace, ctlOffset = @ctlOffset, ctlXS = @ctlXS, ctlSM = @ctlSM, ctlMD = @ctlMD, ctlLG = @ctlLG, ctlInputInline = @ctlInputInline, ctlInputInlineStyle = @ctlInputInlineStyle, colCustomField = @colCustomField where TID = @selTabColName";

            SqlParameter[] _sp = {
                        new SqlParameter("@Invalid", SqlDbType.Bit),
                        new SqlParameter("@ctlPrimaryKey", SqlDbType.Bit),
                        new SqlParameter("@ctlFormStyle", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlDisplayAsterisk", SqlDbType.Bit),
                        new SqlParameter("@ctlAddDisplay", SqlDbType.Bit),
                        new SqlParameter("@ctlAddDisplayButton", SqlDbType.Bit),
                        new SqlParameter("@ctlSaveDraftButton", SqlDbType.Bit),
                        new SqlParameter("@ctlModifyDisplay", SqlDbType.Bit),
                        new SqlParameter("@ctlModifyDisplayButton", SqlDbType.Bit),
                        new SqlParameter("@ctlViewDisplay", SqlDbType.Bit),
                        new SqlParameter("@ctlViewDisplayLabel", SqlDbType.Bit),
                        new SqlParameter("@ctlAfterDisplay", SqlDbType.Bit),
                        new SqlParameter("@ctlTitle", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlTitleStyle", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlTypes", SqlDbType.VarChar,200),
                        new SqlParameter("@ctlPrefix", SqlDbType.VarChar,30),
                        new SqlParameter("@ctlStyle", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlPlaceholder", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlCheckboxSkin", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlSwitchText", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlDescription", SqlDbType.NVarChar,4000),
                        new SqlParameter("@ctlDescriptionDisplayPosition", SqlDbType.VarChar,10),
                        new SqlParameter("@ctlDescriptionDisplayMode", SqlDbType.Bit),
                        new SqlParameter("@ctlDescriptionStyle", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlValue", SqlDbType.VarChar,8000),
                        new SqlParameter("@ctlDefaultValue", SqlDbType.VarChar,8000),
                        new SqlParameter("@ctlExtraFunction", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlReceiveName", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlSourceTable", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlTextName", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlTextValue", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlExtendJSCode", SqlDbType.VarChar,8000),
                        new SqlParameter("@ctlCharLength", SqlDbType.VarChar,30),
                        new SqlParameter("@ctlFilter", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlVerify", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlVerify2", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlVerifyCustomFunction", SqlDbType.VarChar,2000),
                        new SqlParameter("@ctlGroup", SqlDbType.Int),
                        new SqlParameter("@ctlGroupDescription", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlFormItemStyle", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlInlineCss", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlInlineStyle", SqlDbType.VarChar,500),
                        new SqlParameter("@ctlColSpace", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlOffset", SqlDbType.VarChar,200),
                        new SqlParameter("@ctlXS", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlSM", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlMD", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlLG", SqlDbType.VarChar,50),
                        new SqlParameter("@ctlInputInline", SqlDbType.VarChar,100),
                        new SqlParameter("@ctlInputInlineStyle", SqlDbType.VarChar,500),
                        new SqlParameter("@colCustomField", SqlDbType.VarChar,100),
                        new SqlParameter("@selTabColName", SqlDbType.Int)
                        };

            _sp[0].Value = Invalid.toBoolean();
            _sp[1].Value = ctlPrimaryKey.toBoolean();
            _sp[2].Value = ctlFormStyle.toStringTrim();
            _sp[3].Value = ctlDisplayAsterisk.toBoolean();
            _sp[4].Value = ctlAddDisplay.toBoolean();
            _sp[5].Value = ctlAddDisplayButton.toBoolean();
            _sp[6].Value = ctlSaveDraftButton.toBoolean();
            _sp[7].Value = ctlModifyDisplay.toBoolean();
            _sp[8].Value = ctlModifyDisplayButton.toBoolean();
            _sp[9].Value = ctlViewDisplay.toBoolean();
            _sp[10].Value = ctlViewDisplayLabel.toBoolean();
            _sp[11].Value = ctlAfterDisplay.toBoolean();
            _sp[12].Value = ctlTitle.toStringTrim();
            _sp[13].Value = ctlTitleStyle.toStringTrim();
            _sp[14].Value = ctlTypes.toStringTrim();
            _sp[15].Value = ctlPrefix.toStringTrim();
            _sp[16].Value = ctlStyle.toStringTrim();
            _sp[17].Value = ctlPlaceholder.toStringTrim();
            _sp[18].Value = ctlCheckboxSkin.toStringTrim();
            _sp[19].Value = ctlSwitchText.toStringTrim();
            _sp[20].Value = ctlDescription.toStringTrim();
            _sp[21].Value = ctlDescriptionDisplayPosition.toStringTrim();
            _sp[22].Value = ctlDescriptionDisplayMode.toBoolean();
            _sp[23].Value = ctlDescriptionStyle.toStringTrim();
            _sp[24].Value = ctlValue.toStringTrim();
            _sp[25].Value = ctlDefaultValue.toStringTrim();
            _sp[26].Value = ctlExtraFunction.toStringTrim(); //
            _sp[27].Value = ctlReceiveName.toStringTrim();
            _sp[28].Value = ctlSourceTable.toStringTrim();
            _sp[29].Value = ctlTextName.toStringTrim();
            _sp[30].Value = ctlTextValue.toStringTrim();
            _sp[31].Value = ctlExtendJSCode.toStringTrim();
            _sp[32].Value = ctlCharLength.toStringTrim();
            _sp[33].Value = ctlFilter.toStringTrim();
            _sp[34].Value = ctlVerify.toStringTrim();
            _sp[35].Value = ctlVerify2.toStringTrim();
            _sp[36].Value = ctlVerifyCustomFunction.toStringTrim();
            _sp[37].Value = ctlGroup.toInt();
            _sp[38].Value = ctlGroupDescription.toStringTrim();
            _sp[39].Value = ctlFormItemStyle.toStringTrim();
            _sp[40].Value = ctlInlineCss.toStringTrim();
            _sp[41].Value = ctlInlineStyle.toStringTrim();
            _sp[42].Value = ctlColSpace.toStringTrim();
            _sp[43].Value = ctlOffset.toStringTrim();
            _sp[44].Value = ctlXS.toStringTrim();
            _sp[45].Value = ctlSM.toStringTrim();
            _sp[46].Value = ctlMD.toStringTrim();
            _sp[47].Value = ctlLG.toStringTrim();
            _sp[48].Value = ctlInputInline.toStringTrim();
            _sp[49].Value = ctlInputInlineStyle.toStringTrim();
            _sp[50].Value = colCustomField.toStringTrim();
            _sp[51].Value = selTabColName.toInt();

            Boolean flag1 = false;
            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                flag1 = true;

            //更新表全局需要用到的字段（属性）
            string _sql2 = "update MicroTables set ctlFormStyle=@ctlFormStyle, ctlColSpace=@ctlColSpace, ctlAddDisplayButton=@ctlAddDisplayButton, ctlSaveDraftButton=@ctlSaveDraftButton,ctlModifyDisplayButton=@ctlModifyDisplayButton, CustomHtmlCode=@CustomHtmlCode where TID=(select ParentID from MicroTables where TID=@selTabColName) ";
            SqlParameter[] _sp2 = {
                    new SqlParameter("@ctlFormStyle", SqlDbType.VarChar,50),  //Global                    
                    new SqlParameter("@ctlAddDisplayButton", SqlDbType.Bit),  //Global
                    new SqlParameter("@ctlSaveDraftButton", SqlDbType.Bit),  //Global
                    new SqlParameter("@ctlModifyDisplayButton", SqlDbType.Bit), //Global
                    new SqlParameter("@ctlColSpace", SqlDbType.VarChar,50),  //Global
                    new SqlParameter("@CustomHtmlCode", SqlDbType.NText),  //Global
                    new SqlParameter("@selTabColName", SqlDbType.Int) //where
            };

            _sp2[0].Value = ctlFormStyle.toStringTrim();
            _sp2[1].Value = ctlAddDisplayButton.toBoolean();
            _sp2[2].Value = ctlSaveDraftButton.toBoolean();
            _sp2[3].Value = ctlModifyDisplayButton.toBoolean();
            _sp2[4].Value = ctlColSpace.toStringTrim();
            _sp2[5].Value = CustomHtmlCode.toStringTrim();
            _sp2[6].Value = selTabColName.toInt();

            Boolean flag2 = false;
            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag2 = true;

            //更新同一组的组别描述
            string _sql3 = "update MicroTables set ctlGroupDescription=@ctlGroupDescription where ParentID in (select ParentID from MicroTables where TID=@selTabColName) and ctlGroup in (select ctlGroup from MicroTables where TID=@selTabColName)";
            SqlParameter[] _sp3 = {
                    new SqlParameter("@ctlGroupDescription", SqlDbType.VarChar,500),
                    new SqlParameter("@selTabColName", SqlDbType.Int)
                };

            _sp3[0].Value = ctlGroupDescription.toStringTrim();
            _sp3[1].Value = selTabColName.toInt();

            Boolean flag3 = false;
            if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
                flag3 = true;

            if (flag1 && flag2 && flag3)
                flag = MicroPublic.GetMsg("Save");

        }
        catch (Exception e) { flag = e.ToString(); }

        return flag;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}
<%@ WebHandler Language="C#" Class="GetTableAttributes" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;

public class GetTableAttributes : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();
        string flag = "False",
                ShortTableName = context.Server.UrlDecode(context.Request.QueryString["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.QueryString["moduleid"]);

        //测试值
        //ShortTableName = "ITGenForm";

        //用于判断是否有编辑表格权限
        Boolean EditTablePermit = MicroAuth.CheckPermit(ModuleID, "11");

        if (!string.IsNullOrEmpty(ShortTableName))
            flag = GetTableAttr(ShortTableName, EditTablePermit);

        context.Response.Write(flag);
    }

    /// <summary>
    /// 获取表格的全局属性，应用于生成Layui数据表
    /// </summary>
    /// <returns></returns>
    private string GetTableAttr(string ShortTableName, Boolean EditTablePermit)
    {
        string flag = "False", TableName = string.Empty, Cols = string.Empty;
        TableName = MicroPublic.GetTableName(ShortTableName);

        string tbElem = string.Empty, tbURL = string.Empty, tbData = string.Empty, tbToolbar = string.Empty, tbDefaultToolbar = string.Empty, tbTitle = string.Empty, tbText = string.Empty, tbInitSort = string.Empty, tbID = string.Empty, tbSkin = string.Empty, tbEven = string.Empty, tbSize = string.Empty, tbWidth = string.Empty, tbHeight = string.Empty, tbCellMinWidth = string.Empty, tbPage = string.Empty, tbLimit = string.Empty, tbLimits = string.Empty, tbTotalRow = string.Empty, tbLoading = string.Empty, tbAutoSort = string.Empty, tbDone = string.Empty, tbMainSub = string.Empty, colWidth = string.Empty, colMinWidth = string.Empty, colFixed = string.Empty, colType = string.Empty, colAlign = string.Empty, colEdit = string.Empty, colEvent = string.Empty, colStyle = string.Empty, colCheckedAll = string.Empty, colSort = string.Empty, colHide = string.Empty, colUnReSize = string.Empty, colTotalRow = string.Empty, colTotalRowText = string.Empty, colTemplet = string.Empty, colToolbar = string.Empty;

        int inttbWidth = 0, inttbHeight = 0, inttbCellMinWidth = 0, inttbLimit = 0;

        //cols的内容
        string _colType = string.Empty, _colCheckedAll = string.Empty, _colFixed = string.Empty, _colToolbar = string.Empty;

        string strTpl = "\"TID\":\"{0}\",\"ParentID\":\"{1}\",\"TabColName\":\"{2}\",\"DataType\":\"{3}\",\"FieldLength\":\"{4}\",\"DefaultValue\":\"{5}\",\"AllowNull\":\"{6}\",\"Description\":\"{7}\",\"Invalid\":\"{8}\",\"Del\":\"{9}\",\"MainSub\":\"{10}\",\"InJoinSql\":\"{11}\",\"Sort\":\"{12}\",\"PrimaryKey\":\"{13}\",\"Title\":\"{14}\",\"tbElem\":\"{15}\",\"tbURL\":\"{16}\",\"tbData\":\"{17}\",\"tbToolbar\":\"{18}\",\"tbDefaultToolbar\":\"{19}\",\"tbTitle\":\"{20}\",\"tbText\":\"{21}\",\"tbInitSort\":\"{22}\",\"tbID\":\"{23}\",\"tbSkin\":\"{24}\",\"tbEven\":\"{25}\",\"tbSize\":\"{26}\",\"tbWidth\":\"{27}\",\"tbHeight\":\"{28}\",\"tbCellMinWidth\":\"{29}\",\"tbPage\":\"{30}\",\"tbLimit\":\"{31}\",\"tbLimits\":\"{32}\",\"tbTotalRow\":\"{33}\",\"tbLoading\":\"{34}\",\"tbAutoSort\":\"{35}\",\"tbDone\":\"{36}\",\"Cols\":\"{37}\",\"Tips\":\"{38}\",\"CtrlTableContJS\":\"{39}\"";

        StringBuilder sb = new StringBuilder();
        try
        {
            string _sql = "select * from MicroTables where JoinTableColumn=1 and Invalid=0 and Del=0 and ( TabColName =@TableName or ParentID in (select TID from MicroTables where TabColName =@TableName)) order by ParentID,Sort ";
            SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
            _sp[0].Value = TableName;
            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {  //if Start
                DataRow[] _rows = _dt.Select("ParentID=0", "ParentID,Sort");
                foreach (DataRow _dr in _rows)
                {
                    tbElem = _dr["tbElem"].toJsonTrim();
                    tbURL = _dr["tbURL"].toJsonTrim();
                    tbData = _dr["tbData"].toJsonTrim();
                    tbToolbar = _dr["tbToolbar"].toJsonTrim();
                    tbDefaultToolbar = _dr["tbDefaultToolbar"].toJsonTrim();
                    tbTitle = _dr["tbTitle"].toJsonTrim();
                    tbText = _dr["tbText"].toJsonTrim();
                    tbInitSort = _dr["tbInitSort"].toJsonTrim();
                    tbID = _dr["tbID"].toJsonTrim();
                    tbSkin = _dr["tbSkin"].toJsonTrim();

                    tbEven = _dr["tbEven"].toJsonTrim();

                    tbSize = _dr["tbSize"].toJsonTrim();

                    tbWidth = _dr["tbWidth"].toJsonTrim();
                    if (!string.IsNullOrEmpty(tbWidth))
                        inttbWidth = int.Parse(tbWidth);

                    tbHeight = _dr["tbHeight"].toJsonTrim();
                    if (!string.IsNullOrEmpty(tbHeight))
                        inttbHeight = int.Parse(tbHeight);

                    tbCellMinWidth = _dr["tbCellMinWidth"].toJsonTrim();
                    if (!string.IsNullOrEmpty(tbCellMinWidth))
                        inttbCellMinWidth = int.Parse(tbCellMinWidth);

                    tbPage = _dr["tbPage"].toJsonTrim();

                    tbLimit = _dr["tbLimit"].toJsonTrim();
                    if (!string.IsNullOrEmpty(tbLimit))
                        inttbLimit = int.Parse(tbLimit);

                    tbLimits = _dr["tbLimits"].toJsonTrim();

                    tbTotalRow = _dr["tbTotalRow"].toJsonTrim();

                    tbLoading = _dr["tbLoading"].toJsonTrim();

                    tbAutoSort = _dr["tbAutoSort"].toJsonTrim();

                    tbDone = _dr["tbDone"].toJsonTrim();

                    tbMainSub = _dr["tbMainSub"].toJsonTrim();

                    //cols的内容
                    colType = _dr["colType"].toJsonTrim();
                    if (!string.IsNullOrEmpty(colType))
                        _colType = " type:" + "'" + colType + "'";

                    colCheckedAll = _dr["colCheckedAll"].toJsonTrim();
                    if (colType == "checkbox" && !string.IsNullOrEmpty(colCheckedAll))  //typ: checkbox才支持CheckedAll
                    {
                        colCheckedAll = colCheckedAll == "True" ? "true" : "false";
                        _colCheckedAll = ", LAY_CHECKED:" + colCheckedAll;
                    }

                    colFixed = _dr["colFixed"].toJsonTrim();
                    if (!string.IsNullOrEmpty(colFixed))
                        _colFixed = ", fixed:" + "'" + colFixed + "'";

                    colToolbar = _dr["colToolbar"].toJsonTrim();
                    if (!string.IsNullOrEmpty(colToolbar))
                        _colToolbar = ", toolbar:" + colToolbar;

                    //启用checkbox或radio等时，固定在左边
                    if (!string.IsNullOrEmpty(colType))
                    {
                        _colFixed = ", fixed:" + "'left'";
                        Cols += "{" + _colType + _colCheckedAll + _colFixed + "},";
                    }

                    //有父子节点关联时加入MainSub表头
                    if (tbMainSub == "True")
                        Cols += "{ field: 'MainSub', title: 'MainSub', align: 'center', width: 100, fixed: 'left' },";

                    Cols += GetCols(_dr["TID"].toJsonTrim(), _dt, EditTablePermit);
                    Cols = "[" + Cols + "]";

                    string str1 = string.Format(strTpl, _dr["TID"].toJsonTrim(), _dr["ParentID"].toJsonTrim(), _dr["TabColName"].toJsonTrim(), _dr["DataType"].toJsonTrim(), _dr["FieldLength"].toJsonTrim(), _dr["DefaultValue"].toJsonTrim(), _dr["AllowNull"].toJsonTrim(), _dr["Description"].toJsonTrim(), _dr["Invalid"].toJsonTrim(), _dr["Del"].toJsonTrim(), "Main", _dr["InJoinSql"].toJsonTrim(), _dr["Sort"].toJsonTrim(), _dr["PrimaryKey"].toJsonTrim(), _dr["Title"].toJsonTrim(), tbElem, tbURL, tbData, tbToolbar, tbDefaultToolbar, tbTitle, tbText, tbInitSort, tbID, tbSkin, tbEven, tbSize, inttbWidth, inttbHeight, inttbCellMinWidth, tbPage, inttbLimit, tbLimits, tbTotalRow, tbLoading, tbAutoSort, tbDone, Cols, GetTips(_dr["TID"].toJsonTrim(), _dt), GetCtrlTableContJS(ShortTableName, _dt));

                    sb.Append("{" + str1 + "},");
                }
                string json = sb.ToString();

                flag = "[" + json.Substring(0, json.Length - 1) + "]";
                flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + _dt.Rows.Count + ",\"data\":  " + flag + " }";
            }
        }
        catch { }

        return flag;

    }

    /// <summary>
    /// 获取表头属性，应用于生成Layui数据表
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="_dt"></param>
    /// <returns></returns>
    private string GetCols(string ID, DataTable _dt, Boolean EditTablePermit)
    {
        string Cols = string.Empty;
        DataRow[] _rows = _dt.Select("ParentID=" + ID, "ParentID,colCustomSort");

        foreach (DataRow _dr in _rows)
        {
            string Field = string.Empty, DataType = string.Empty, Title = string.Empty, TitleTipsIcon = string.Empty, colTitle = string.Empty, colCustomField = string.Empty, colWidth = string.Empty, colMinWidth = string.Empty, colFixed = string.Empty, colAlign = string.Empty, colEdit = string.Empty, colEvent = string.Empty, colStyle = string.Empty, colSort = string.Empty, colHide = string.Empty, colUnReSize = string.Empty, colTotalRow = string.Empty, colTotalRowText = string.Empty, colTemplet = string.Empty, colToolbar = string.Empty, ctlSourceTable = string.Empty, ctlTextName = string.Empty, ctlTextValue = string.Empty;

            //用于拼接字符串
            string _Field = string.Empty, _Title = string.Empty, _TitleTipsIcon = string.Empty, _colWidth = string.Empty, _colCustomField = string.Empty, _colMinWidth = string.Empty, _colFixed = string.Empty, _colAlign = string.Empty, _colEdit = string.Empty, _colEvent = string.Empty, _colStyle = string.Empty, _colSort = string.Empty, _colHide = string.Empty, _colUnReSize = string.Empty, _colTotalRow = string.Empty, _colTotalRowText = string.Empty, _colTemplet = string.Empty, _colToolbar = string.Empty;

            ctlSourceTable = _dr["ctlSourceTable"].toJsonTrim(); ctlTextName = _dr["ctlTextName"].toJsonTrim(); ctlTextValue = _dr["ctlTextValue"].toJsonTrim();

            Field = _dr["TabColName"].toJsonTrim();
            colCustomField = _dr["colCustomField"].toJsonTrim();  //自定义LayuiDataTable字段

            if (!string.IsNullOrEmpty(ctlSourceTable) && !string.IsNullOrEmpty(ctlTextName) && !string.IsNullOrEmpty(ctlTextValue) && !string.IsNullOrEmpty(colCustomField))
                Field = colCustomField;

            if (!string.IsNullOrEmpty(Field))
                _Field = "field:" + "'" + Field + "'";

            DataType = _dr["DataType"].toJsonTrim();

            TitleTipsIcon = _dr["TitleTipsIcon"].toJsonTrim();
            if (TitleTipsIcon == "True")
            {
                _TitleTipsIcon = "<i id=\\\\'tips" + Field + "\\\\' class=\\\\'layui-icon layui-icon-about\\\\'></i>";
            }

            Title = _dr["Title"].toJsonTrim();
            colTitle = _dr["colTitle"].toJsonTrim();
            Title = string.IsNullOrEmpty(colTitle) ? Title : colTitle;

            if (!string.IsNullOrEmpty(Title))
                _Title = ", title:" + "'" + Title + _TitleTipsIcon + "'";

            colWidth = _dr["colWidth"].toJsonTrim();
            if (!string.IsNullOrEmpty(colWidth))
            {
                if (colWidth.Contains("%"))
                    _colWidth = ", width:" + "'" + colWidth + "'";

                else if (colWidth.Contains(","))
                {
                    //宽度的值还可以设置为两个用逗号分隔“200,100”，第一个值代表管理员调用，第2个值非管理员调用
                    //应用场景如全社表单操作列的宽度设置（“变更”按钮非管理员隐藏，但多出了宽度）
                    if (MicroUserHelper.MicroUserInfo.CheckUserRole("Administrators"))
                        _colWidth = ", width:" + colWidth.Split(',')[0].toInt();
                    else
                        _colWidth = ", width:" + colWidth.Split(',')[1].toInt();
                }
                else
                    _colWidth = ", width:" + colWidth.toInt();
            }

            colMinWidth = _dr["colMinWidth"].toJsonTrim();
            if (!string.IsNullOrEmpty(colMinWidth))
                _colMinWidth = ", minWidth:" + colMinWidth.toInt();

            colFixed = _dr["colFixed"].toJsonTrim();
            if (!string.IsNullOrEmpty(colFixed))
                _colFixed = ", fixed:" + "'" + colFixed + "'";

            colAlign = _dr["colAlign"].toJsonTrim();
            if (!string.IsNullOrEmpty(colAlign))
                _colAlign = ", align:" + "'" + colAlign + "'";

            colEdit = _dr["colEdit"].toJsonTrim();
            //有编辑表格权限时才能继续进行
            if (EditTablePermit)
            {
                if (!string.IsNullOrEmpty(colEdit))
                    _colEdit = ", edit:" + "'" + colEdit + "'";
            }

            colEvent = _dr["colEvent"].toJsonTrim();
            if (!string.IsNullOrEmpty(colEvent))
                _colEvent = ", event:" + "'" + colEvent + "'";

            colStyle = _dr["colStyle"].toJsonTrim();
            if (!string.IsNullOrEmpty(colStyle))
                _colStyle = ", style:" + "'" + colStyle + "'";

            colSort = _dr["colSort"].toJsonTrim();
            if (!string.IsNullOrEmpty(colSort))
            {
                colSort = colSort == "True" ? "true" : "false";
                _colSort = ", sort:" + colSort;
            }

            colHide = _dr["colHide"].toJsonTrim();
            if (!string.IsNullOrEmpty(colHide))
            {
                colHide = colHide == "True" ? "true" : "false";
                _colHide = ", hide:" + colHide;
            }

            colUnReSize = _dr["colUnReSize"].toJsonTrim();
            if (!string.IsNullOrEmpty(colUnReSize))
            {
                colUnReSize = colUnReSize == "True" ? "true" : "false";
                _colUnReSize = ", unresize:" + colUnReSize;
            }

            colTotalRow = _dr["colTotalRow"].toJsonTrim();
            if (!string.IsNullOrEmpty(colTotalRow))
            {
                colTotalRow = colTotalRow == "True" ? "true" : "false";
                _colTotalRow = ", totalRow:" + colTotalRow;
            }

            colTotalRowText = _dr["colTotalRowText"].toJsonTrim();
            if (!string.IsNullOrEmpty(colTotalRowText))
                _colTotalRowText = ", totalRowText:" + "'" + colTotalRowText + "'";

            colTemplet = _dr["colTemplet"].toJsonTrim();
            if (!string.IsNullOrEmpty(colTemplet))
                _colTemplet = ", templet:" + "'" + colTemplet + "'";
            else
            {
                if (DataType == "bit")
                    _colTemplet = ", templet:" + "'#sw" + Field + "'";
            }

            colToolbar = _dr["colToolbar"].toJsonTrim();
            if (!string.IsNullOrEmpty(colToolbar))
                _colToolbar = ", toolbar:'" + colToolbar + "'";

            Cols += "{";
            Cols += _Field + _Title + _colWidth + _colMinWidth + _colFixed + _colAlign + _colEdit + _colEvent + _colStyle + _colSort + _colHide + _colUnReSize + _colTotalRow + _colTotalRowText + _colTemplet + _colToolbar;
            Cols += "},";

        }

        Cols = Cols.Substring(0, Cols.Length - 1);
        return Cols;
    }

    /// <summary>
    /// 调用js Tips方法显示Tips内容，GetTips在Layui数据表done时执行。
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="_dt"></param>
    /// <returns></returns>
    private string GetTips(string ID, DataTable _dt)
    {
        string Tips = string.Empty, _Tips = string.Empty;
        DataRow[] _rows = _dt.Select("ParentID=" + ID);

        foreach (DataRow _dr in _rows)
        {
            Tips = _dr["Description"].toJsonTrim();
            if (!string.IsNullOrEmpty(Tips))
                _Tips += "micro.Tips('" + Tips + "', '#tips" + _dr["TabColName"].toJsonTrim() + "', 1);";
        }

        //Tips = Cols.Substring(0, Cols.Length - 1);
        return _Tips;
    }

    /// <summary>
    /// 生成修改数据表单元格和Switch Change事件的JS代码，修改单元格的内容
    /// </summary>
    /// <returns></returns>
    private string GetCtrlTableContJS(string ShortTableName, DataTable _dt)
    {

        string flag = string.Empty, TableID = string.Empty, IDName = string.Empty, IDValue = string.Empty, FieldName = string.Empty;
        DataRow[] _rows = _dt.Select("ParentID=0", "ParentID,Sort");  //通过它提取TableID
        DataRow[] _rows2 = _dt.Select("DataType='int' and PrimaryKey=1", "ParentID,Sort");  //通过它提取表的主键
        DataRow[] _rows3 = _dt.Select("DataType='bit'", "ParentID,Sort");  //获取DateType类型为bit

        if (_rows.Length > 0 && _rows2.Length > 0)
        {
            TableID = _rows[0]["tbID"].toJsonTrim();
            if (string.IsNullOrEmpty(TableID))
                TableID = "tabTable";

            IDName = _rows2[0]["TabColName"].toJsonTrim();

            //生成监听单元格编辑代码
            flag += "table.on('edit(" + TableID + ")', function (o) {";
            flag += "var data = o.data,FieldName = o.field,FieldValue = o.value;";
            flag += "var Fields = { 'STN': '" + ShortTableName + "', 'IDName': '" + IDName + "', 'IDValue': data." + IDName + ", 'FieldName': FieldName, 'FieldValue': FieldValue };";
            flag += "Fields = encodeURI(JSON.stringify(Fields));";
            //flag += "console.log(Fields);";
            flag += "var Parameter = { 'action': 'modify', 'mid': micro.getUrlPara('mid'), 'fields': Fields };";
            flag += "micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicTableField.ashx', Parameter);";
            flag += "});";

            if (_rows3.Length > 0)
            {
                foreach (DataRow _dr3 in _rows3)
                {
                    //生成监听checkbox(switch)代码
                    flag += "form.on('switch(" + _dr3["TabColName"].toJsonTrim() + ")', function(o) {";
                    flag += "var Fields = { 'STN': '" + ShortTableName + "', 'IDName': '" + IDName + "', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': o.elem.checked };";
                    flag += "Fields = encodeURI(JSON.stringify(Fields));";
                    flag += "var Parameter = { 'action': 'modify', 'mid': micro.getUrlPara('mid'), 'fields': Fields };";
                    flag += "micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicTableField.ashx', Parameter);";
                    flag += " });";
                }
            }

        }

        return flag;

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
layui.use(['index', 'jquery', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');
    var EditTablePermit = $('#txtEditTablePermit').val();
    var Edit = EditTablePermit === '1' ? 'text' : '';

    var tabPara = { 'action': 'view', 'mid': MID};
    //数据表管理
    table.render({
        elem: '#tabTable'
        //, url: '/Views/Set/System/DataTableList.ashx'
        , toolbar: true
        //, defaultToolbar: 'default'
        , cols: [[
            { type: 'checkbox', fixed: 'left', rowspan: 2 }
            , { field: 'MainSub', width: 90, title: 'MainSub', align: 'center', fixed: 'left', rowspan: 2 }

            , { field: 'TID', title: 'TID<i class=\'layui-icon layui-icon-about\' id=\'tipsTID\'></i>', width: 80, align: 'center', rowspan: 2, fixed: 'left', hide: true }
            , { field: 'Sort', title: 'Sort<i class=\'layui-icon layui-icon-about\' id=\'tipsSort\'></i>', width: 80, align: 'center', edit: Edit, rowspan: 2, fixed: 'left' }
            , { field: 'ParentID', title: 'ParentID<i class=\'layui-icon layui-icon-about\' id=\'tipsParentID\'></i>', width: 90, align: 'center', rowspan: 2, fixed: 'left', hide: true }
            , { field: 'Title', title: 'Title<i class=\'layui-icon layui-icon-about\' id=\'tipsTitle\'></i>', width: 160, align: 'center', edit: Edit, rowspan: 2, fixed: 'left' }
            , { field: 'TabColName', title: 'TabColName<i class=\'layui-icon layui-icon-about\' id=\'tipsTabColName\'></i>', width: 200, align: 'center', rowspan: 2, fixed: 'left' }
            , { field: 'ShortTableName', title: 'ShortTableName<i class=\'layui-icon layui-icon-about\' id=\'tipsShortTableName\'></i>', width: 160, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'DataType', title: 'DataType<i class=\'layui-icon layui-icon-about\' id=\'tipsDataType\'></i>', width: 150, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'FieldLength', title: 'FieldLength<i class=\'layui-icon layui-icon-about\' id=\'tipsFieldLength\'></i>', width: 150, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'DefaultValue', title: 'DefaultValue<i class=\'layui-icon layui-icon-about\' id=\'tipsDefaultValue\'></i>', width: 150, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'IntSort', title: 'IntSort<i class=\'layui-icon layui-icon-about\' id=\'tipsIntSort\'></i>', width: 150, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'AscDesc', title: 'AscDesc<i class=\'layui-icon layui-icon-about\' id=\'tipsAscDesc\'></i>', width: 150, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'AllowNull', title: 'AllowNull<i class=\'layui-icon layui-icon-about\' id=\'tipsAllowNull\'></i>', width: 150, align: 'center', templet: '#swAllowNull', rowspan: 2 }
            , { field: 'InJoinSql', title: 'InJoinSql<i class=\'layui-icon layui-icon-about\' id=\'tipsInJoinSql\'></i>', width: 150, align: 'center', templet: '#swInJoinSql', rowspan: 2 }
            , { field: 'PrimaryKey', title: 'PrimaryKey<i class=\'layui-icon layui-icon-about\' id=\'tipsPrimaryKey\'></i>', width: 150, align: 'center', templet: '#swPrimaryKey', rowspan: 2 }
            , { field: 'Invalid', title: 'Invalid<i class=\'layui-icon layui-icon-about\' id=\'tipsInvalid\'></i>', width: 150, align: 'center', templet: '#swInvalid', rowspan: 2 }
            , { field: 'Del', title: 'Del<i class=\'layui-icon layui-icon-about\' id=\'tipsDel\'></i>', width: 150, align: 'center', templet: '#swDel', rowspan: 2 }
            , { align: 'left', title: '表格全局属性（只需在表名行设定一次即可，表名为加粗字体行）', colspan: 24 }
            , { align: 'left', title: '表头属性（需在字段行设定，即非加粗字体行）', colspan: 21 }
            , { align: 'left', title: '控件设置', colspan: 50 }
            , { align: 'left', title: '查询设置', colspan: 5 }
            , { field: 'Description', title: 'Description<i class=\'layui-icon layui-icon-about\' id=\'tipsDescription\'></i>', width: 500, align: 'left', edit: Edit, rowspan: 2 }
            //, { title: '操作', width: 150, align: 'center', fixed: 'right', toolbar: '#table-useradmin-webuser', rowspan: 2 }
        ],
        [
            //表格属性tb
            { field: 'Title', title: 'Title<i class=\'layui-icon layui-icon-about\' id=\'tipsTitle\'></i>', width: 160, align: 'center', edit: Edit, rowspan: 2 }
            , { field: 'tbElem', title: 'tbElem<i class=\'layui-icon layui-icon-about\' id=\'tipstbElem\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbURL', title: 'tbURL<i class=\'layui-icon layui-icon-about\' id=\'tipstbURL\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbData', title: 'tbData<i class=\'layui-icon layui-icon-about\' id=\'tipstbData\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbToolbar', title: 'tbToolbar<i class=\'layui-icon layui-icon-about\' id=\'tipstbToolbar\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbDefaultToolbar', title: 'tbDefaultToolbar<i class=\'layui-icon layui-icon-about\' id=\'tipstbDefaultToolbar\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbTitle', title: 'tbTitle<i class=\'layui-icon layui-icon-about\' id=\'tipstbTitle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbText', title: 'tbText<i class=\'layui-icon layui-icon-about\' id=\'tipstbText\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbInitSort', title: 'tbInitSort<i class=\'layui-icon layui-icon-about\' id=\'tipstbInitSort\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbID', title: 'tbID<i class=\'layui-icon layui-icon-about\' id=\'tipstbID\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbSkin', title: 'tbSkin<i class=\'layui-icon layui-icon-about\' id=\'tipstbSkin\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbEven', title: 'tbEven<i class=\'layui-icon layui-icon-about\' id=\'tipstbEven\'></i>', width: 150, align: 'center', templet: '#swtbEven' }
            , { field: 'tbSize', title: 'tbSize<i class=\'layui-icon layui-icon-about\' id=\'tipstbSize\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbWidth', title: 'tbWidth<i class=\'layui-icon layui-icon-about\' id=\'tipstbWidth\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbHeight', title: 'tbHeight<i class=\'layui-icon layui-icon-about\' id=\'tipstbHeight\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbCellMinWidth', title: 'tbCellMinWidth<i class=\'layui-icon layui-icon-about\' id=\'tipstbCellMinWidth\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbPage', title: 'tbPage<i class=\'layui-icon layui-icon-about\' id=\'tipstbPage\'></i>', width: 150, align: 'center', templet: '#swtbPage' }
            , { field: 'tbLimit', title: 'tbLimit<i class=\'layui-icon layui-icon-about\' id=\'tipstbLimit\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbLimits', title: 'tbLimits<i class=\'layui-icon layui-icon-about\' id=\'tipstbLimits\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbTotalRow', title: 'tbTotalRow<i class=\'layui-icon layui-icon-about\' id=\'tipstbTotalRow\'></i>', width: 150, align: 'center', templet: '#swtbTotalRow' }
            , { field: 'tbLoading', title: 'tbLoading<i class=\'layui-icon layui-icon-about\' id=\'tipstbLoading\'></i>', width: 150, align: 'center', templet: '#swtbLoading' }
            , { field: 'tbAutoSort', title: 'tbAutoSort<i class=\'layui-icon layui-icon-about\' id=\'tipstbAutoSort\'></i>', width: 150, align: 'center', templet: '#swtbAutoSort' }
            , { field: 'tbDone', title: 'tbDone<i class=\'layui-icon layui-icon-about\' id=\'tipstbDone\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'tbMainSub', title: 'tbMainSub<i class=\'layui-icon layui-icon-about\' id=\'tipstbMainSub\'></i>', width: 150, align: 'center', templet: '#swtbMainSub' }

            //表头属性
            , { field: 'colCustomSort', title: 'colCustomSort<i class=\'layui-icon layui-icon-tips\' id=\'tipscolCustomSort\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colTitle', title: 'colTitle<i class=\'layui-icon layui-icon-tips\' id=\'tipscolTitle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'TitleTipsIcon', title: 'TitleTipsIcon<i class=\'layui-icon layui-icon-about\' id=\'tipsTitleTipsIcon\'></i>', width: 150, align: 'center', templet: '#swTitleTipsIcon', rowspan: 2 }
            , { field: 'colCustomField', title: 'colCustomField<i class=\'layui-icon layui-icon-tips\' id=\'tipscolCustomField\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colWidth', title: 'colWidth<i class=\'layui-icon layui-icon-tips\' id=\'tipscolWidth\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colMinWidth', title: 'colMinWidth<i class=\'layui-icon layui-icon-tips\' id=\'tipscolMinWidth\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colFixed', title: 'colFixed<i class=\'layui-icon layui-icon-tips\' id=\'tipscolFixed\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colType', title: 'colType<i class=\'layui-icon layui-icon-tips\' id=\'tipscolType\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colAlign', title: 'colAlign<i class=\'layui-icon layui-icon-tips\' id=\'tipscolAlign\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colEdit', title: 'colEdit<i class=\'layui-icon layui-icon-tips\' id=\'tipscolEdit\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colEvent', title: 'colEvent<i class=\'layui-icon layui-icon-tips\' id=\'tipscolEvent\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colStyle', title: 'colStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipscolStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colCheckedAll', title: 'colCheckedAll<i class=\'layui-icon layui-icon-tips\' id=\'tipscolCheckedAll\'></i>', width: 150, align: 'center', templet: '#swcolCheckedAll' }
            , { field: 'colSort', title: 'colSort<i class=\'layui-icon layui-icon-tips\' id=\'tipscolSort\'></i>', width: 150, align: 'center', templet: '#swcolSort' }
            , { field: 'colHide', title: 'colHide<i class=\'layui-icon layui-icon-tips\' id=\'tipscolHide\'></i>', width: 150, align: 'center', templet: '#swcolHide' }
            , { field: 'colUnReSize', title: 'colUnReSize<i class=\'layui-icon layui-icon-tips\' id=\'tipscolUnReSize\'></i>', width: 150, align: 'center', templet: '#swcolUnReSize' }
            , { field: 'colTotalRow', title: 'colTotalRow<i class=\'layui-icon layui-icon-tips\' id=\'tipscolTotalRow\'></i>', width: 150, align: 'center', templet: '#swcolTotalRow' }
            , { field: 'colTotalRowText', title: 'colTotalRowText<i class=\'layui-icon layui-icon-tips\' id=\'tipscolTotalRowText\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colTemplet', title: 'colTemplet<i class=\'layui-icon layui-icon-tips\' id=\'tipscolTemplet\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'colToolbar', title: 'colToolbar<i class=\'layui-icon layui-icon-tips\' id=\'tipscolToolbar\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'JoinTableColumn', title: 'JoinTableColumn<i class=\'layui-icon layui-icon-tips\' id=\'tipsJoinTableColumn\'></i>', width: 200, align: 'center', templet: '#swJoinTableColumn' }

            //控件设置（在页面前端显示的HTML控件）
            , { field: 'ctlPrimaryKey', title: 'ctlPrimaryKey<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlPrimaryKey\'></i>', width: 150, align: 'center', templet: '#swctlPrimaryKey' }
            , { field: 'ctlFormStyle', title: 'ctlFormStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlFormStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlDisplayAsterisk', title: 'ctlDisplayAsterisk<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlDisplayAsterisk\'></i>', width: 150, align: 'center', templet: '#swctlDisplayAsterisk' }
            , { field: 'ctlAddDisplay', title: 'ctlAddDisplay<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlAddDisplay\'></i>', width: 150, align: 'center', templet: '#swctlAddDisplay' }
            , { field: 'ctlAddDisplayButton', title: 'ctlAddDisplayButton<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlAddDisplayButton\'></i>', width: 150, align: 'center', templet: '#swctlAddDisplayButton' }
            , { field: 'ctlSaveDraftButton', title: 'ctlSaveDraftButton<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlSaveDraftButton\'></i>', width: 150, align: 'center', templet: '#swctlSaveDraftButton' }
            , { field: 'ctlModifyDisplay', title: 'ctlModifyDisplay<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlModifyDisplay\'></i>', width: 150, align: 'center', templet: '#swctlModifyDisplay' }
            , { field: 'ctlModifyDisplayButton', title: 'ctlModifyDisplayButton<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlModifyDisplayButton\'></i>', width: 150, align: 'center', templet: '#swctlModifyDisplayButton' }
            , { field: 'ctlViewDisplay', title: 'ctlViewDisplay<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlViewDisplay\'></i>', width: 150, align: 'center', templet: '#swctlViewDisplay' }
            , { field: 'ctlViewDisplayLabel', title: 'ctlViewDisplayLabel<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlViewDisplayLabel\'></i>', width: 150, align: 'center', templet: '#swctlViewDisplayLabel' }
            , { field: 'ctlAfterDisplay', title: 'ctlAfterDisplay<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlAfterDisplay\'></i>', width: 150, align: 'center', templet: '#swctlAfterDisplay' }
            , { field: 'ctlTitle', title: 'ctlTitle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlTitle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlTitleStyle', title: 'ctlTitleStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlTitleStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlTypes', title: 'ctlTypes<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlTypes\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlPrefix', title: 'ctlPrefix<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlPrefix\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlStyle', title: 'ctlStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlPlaceholder', title: 'ctlPlaceholder<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlPlaceholder\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlCheckboxSkin', title: 'ctlCheckboxSkin<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlCheckboxSkin\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlSwitchText', title: 'ctlSwitchText<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlSwitchText\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlDescription', title: 'ctlDescription<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlDescription\'></i>', width: 500, align: 'left', edit: Edit }
            , { field: 'ctlDescriptionDisplayPosition', title: 'ctlDescriptionDisplayPosition<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlDescriptionDisplayPosition\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlDescriptionDisplayMode', title: 'ctlDescriptionDisplayMode<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlDescriptionDisplayMode\'></i>', width: 150, align: 'center', templet: '#swctlDescriptionDisplayMode' }
            , { field: 'ctlDescriptionStyle', title: 'ctlDescriptionStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlDescriptionStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlValue', title: 'ctlValue<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlValue\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlDefaultValue', title: 'ctlDefaultValue<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlDefaultValue\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlExtraFunction', title: 'ctlExtraFunction<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlExtraFunction\'></i>', width: 150, align: 'center', edit: Edit } //********
            , { field: 'ctlReceiveName', title: 'ctlReceiveName<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlReceiveName\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlSourceTable', title: 'ctlSourceTable<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlSourceTable\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlTextName', title: 'ctlTextName<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlTextName\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlTextValue', title: 'ctlTextValue<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlTextValue\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlExtendJSCode', title: 'ctlExtendJSCode<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlExtendJSCode\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlCharLength', title: 'ctlCharLength<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlCharLength\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlFilter', title: 'ctlFilter<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlFilter\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlVerify', title: 'ctlVerify<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlVerify\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlVerify2', title: 'ctlVerify2<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlVerify2\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlVerifyCustomFunction', title: 'ctlVerifyCustomFunction<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlVerifyCustomFunction\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlGroup', title: 'ctlGroup<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlGroup\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlGroupDescription', title: 'ctlGroupDescription<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlGroupDescription\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlFormItemStyle', title: 'ctlFormItemStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlFormItemStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlInlineCss', title: 'ctlInlineCss<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlInlineCss\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlInlineStyle', title: 'ctlInlineStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlInlineStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlColSpace', title: 'ctlColSpace<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlColSpace\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlOffset', title: 'ctlOffset<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlOffset\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlXS', title: 'ctlXS<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlXS\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlSM', title: 'ctlSM<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlSM\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlMD', title: 'ctlMD<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlMD\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlLG', title: 'ctlLG<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlLG\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlInputInline', title: 'ctlInputInline<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlInputInline\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'ctlInputInlineStyle', title: 'ctlInputInlineStyle<i class=\'layui-icon layui-icon-tips\' id=\'tipsctlInputInlineStyle\'></i>', width: 150, align: 'center', edit: Edit }
            , { field: 'CustomHtmlCode', title: 'CustomHtmlCode<i class=\'layui-icon layui-icon-tips\' id=\'tipsCustomHtmlCode\'></i>', width: 150, align: 'center', edit: Edit }

            //查询设置
            , { field: 'querySort', title: 'querySort<i class=\'layui-icon layui-icon-tips\' id=\'tipsquerySort\'></i>', width: 150, align: 'center', edit: 'text' }
            , { field: 'queryTitle', title: 'queryTitle<i class=\'layui-icon layui-icon-tips\' id=\'tipsqueryTitle\'></i>', width: 150, align: 'center', edit: 'text' }
            , { field: 'queryAsBaseField', title: 'queryAsBaseField<i class=\'layui-icon layui-icon-tips\' id=\'tipsqueryAsBaseField\'></i>', width: 200, align: 'center', templet: '#swqueryAsBaseField' }
            , { field: 'queryAsAdvancedField', title: 'queryAsAdvancedField<i class=\'layui-icon layui-icon-tips\' id=\'tipsqueryAsAdvancedField\'></i>', width: 200, align: 'center', templet: '#swqueryAsAdvancedField' }
            , { field: 'queryAsKeywordField', title: 'queryAsKeywordField<i class=\'layui-icon layui-icon-tips\' id=\'tipsqueryAsKeywordField\'></i>', width: 200, align: 'center', templet: '#swqueryAsKeywordField' }
            , { field: 'queryCtlTypes', title: 'queryCtlTypes<i class=\'layui-icon layui-icon-tips\' id=\'tipsqueryCtlTypes\'></i>', width: 150, align: 'center', edit: 'text' }


        ]]
        , page: true
        , limit: 200
        , limits: [10, 20, 30, 50, 100, 200, 300, 500, 1000]
        , data: micro.getAjaxData('json', micro.getRootPath() + '/Views/Set/System/MicroDataTableList.ashx', tabPara)["data"]
        , height: 'full-120'
        //, size: 'sm'
        , text: { none:'No Data.没有数据。' }
        , done: function () {
            $('table tr').each(function () {   //加深颜色
                var s = $(this).children().eq(1).text();
                if (s === "Main")
                    $(this).toggleClass("ws-datatable-tr");
            });
            //debugger;
            // 当前页面缓存数据
            var dataTemp = table.cache[this.id];
            // 控件渲染出来的table
            var tableElem = this.elem.next();
            layui.each(dataTemp, function (index, data) {
                if (data.ParentID === '0') {
                    // 关闭修改
                    tableElem.find('tr[data-index="' + index + '"]').find('td[data-field="ParentID"]').removeAttr('data-edit');
                }
            });
        }
    });

    //监听搜索
    form.on('submit(btnSearch)', function (data) {
        var Fields = data.field;
        alert(Fields);
        return false;
        //执行重载
        table.reload('tabTable', {
            where: Fields
        });
    });

    //监听单元格编辑
    var Path = micro.getRootPath() + '/Views/Set/System/MicroDataTable.ashx';

    table.on('edit(tabTable)', function (obj) {
        var data = obj.data, //所在行所有键值
            FieldName = obj.field, //字段名称
            FieldValue = obj.value; //修改后的值
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': data.TID, 'FieldName': FieldName, 'FieldValue': FieldValue };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);

    });

    form.on('switch(TitleTipsIcon)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(AllowNull)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(InJoinSql)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(PrimaryKey)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(Invalid)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(Del)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(tbEven)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(tbPage)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(tbTotalRow)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(tbLoading)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(tbAutoSort)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(tbMainSub)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(colCheckedAll)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(colSort)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(colHide)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(colUnReSize)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(colTotalRow)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(JoinTableColumn)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlPrimaryKey)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlDisplayAsterisk)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlAddDisplay)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlAddDisplayButton)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlSaveDraftButton)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlModifyDisplay)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlModifyDisplayButton)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlViewDisplay)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlViewDisplayLabel)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlAfterDisplay)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(ctlDescriptionDisplayMode)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(queryAsBaseField)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(queryAsAdvancedField)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });
    form.on('switch(queryAsKeywordField)', function (obj) {
        var Fields = { 'TableName': 'mTabs', 'IDName': 'TID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    //事件
    var active = {
        btnDel: function () {
            var checkStatus = table.checkStatus('tabTable')
                , checkData = checkStatus.data; //得到选中的数据

            if (checkData.length === 0) {
                return layer.msg('请选择数据');
            }

            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证口令'
            }, function (value, index) {
                layer.close(index);

                layer.confirm('确定删除吗？', function (index) {

                    //执行 Ajax 后重载
                    /*
                    admin.req({
                      url: 'xxx'
                      //,……
                    });
                    */
                    table.reload('tabTable');
                    layer.msg('已删除');
                });
            });
        }
        , btnAddTab: function () {         
            layer.open({
                type: 2
                , title: '创建数据表'
                , content: micro.getRootPath() +  '/Views/Set/System/CreateMicroDataTable/Add/MTabs/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即保存', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnAddTab'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
            });
        }
        , btnAddCol: function () {
            layer.open({
                type: 2
                , title: '添加表字段'
                , content: micro.getRootPath() + '/Views/Set/System/AddMicroDataTableColumn/Add/MTabs/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即保存', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnAddCol'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();

                }
            });
        }
    };

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });

    micro.Tips('主键，标识1，自动递增1', '#tipsTID', 1);
    micro.Tips('Int，排序【本表排序用】', '#tipsSort', 1);
    micro.Tips('Int，所属父ID,若为父元素时为0【本表排序用】', '#tipsParentID', 1);
    micro.Tips('String，表名or列名', '#tipsTabColName', 1);
    micro.Tips('String，短名称，表名or列名，用于对应真实表名避免真实表名暴露', '#tipsShortTableName', 1);
    micro.Tips('String，表的列名标题，用于动态生成表的列标题', '#tipsTitle', 1);
    micro.Tips('Boolean，显示列名的Tips Icon，Tips的内容是Description的值，默认值(D)：true', '#tipsTitleTipsIcon', 1);
    micro.Tips('String，数据库字段类型', '#tipsDataType', 1);
    micro.Tips('String，字段长度，通常针对DataType为string类型', '#tipsFieldLength', 1);
    micro.Tips('String，默认值，无默认值为系统null（在数据库字段默认值）', '#tipsDefaultValue', 1);
    micro.Tips('Int，以哪个字段作为表的Sql语句的排序，如：一个表有字段Field1,Field2…FieldN，若Field2=1，Field1=2，则Sql语句为 order by Field2,Field1【在创建的表读取该表的记录时使用】。注：在大多数的表中会默认有Sort这个排序字段，但不一定所有表都会通用，所以这个字段进行', '#tipsIntSort', 1);
    micro.Tips('String，Sql语句以升序还是降序进行排序，空值时为升序，可选值有：asc(升序)、desc(降序)。默认值(D): NULL', '#tipsAscDesc', 1);
    micro.Tips('Boolean，允许空值，默认值(D)：True', '#tipsAllowNull', 1);
    micro.Tips('Boolean，加入生成Sql命令，目前紧应用在代码工具页面上，默认值(D)：True', '#tipsInJoinSql', 1);
    micro.Tips('Boolean，是否为主键，默认值(D)：False', '#tipsPrimaryKey', 1);
    micro.Tips('String，描述', '#tipsDescription', 1);
    micro.Tips('Boolean，无效。无效时管理员还可见，其它角色不可见，默认值(D)：False', '#tipsInvalid', 1);
    micro.Tips('Boolean，删除。为了保护数据的安全性，这里的删除非真的删除只是在查询结果对所有角色不再显示，默认值(D)：False', '#tipsDel', 1);
    micro.Tips('String，"#demo"指定原始 table 容器的选择器或 DOM，方法渲染方式必填。默认值(M)：“#tabTable”', '#tipstbElem', 1);
    micro.Tips('String，异步数据接口相关参数。其中 url 参数为必填项，返回json格式的数据。例：[{"Name":"微办公","Value":"MicroOA"}]，默认值(M)：/App_Handler/GetPublicTableList.ashx', '#tipstbURL', 1);
    micro.Tips('String，[{}, {}, {}, {}, …]直接赋值数据。既适用于只展示一页数据，也非常适用于对一段已知数据进行多页展示。这里不直接填写数据，而是把数据赋值给变量，填写变量的名称（数据的格式是json格式）', '#tipstbData', 1);
    micro.Tips('String/DOM/Boolean <br/>开启表格头部工具栏区域，该参数支持四种类型值： <br/>toolbar: \'#toolbarDemo\' //指向自定义工具栏模板选择器 <br/>toolbar: \'<div>xxx</div>\' //直接传入工具栏模板字符 <br/>toolbar: true //仅开启工具栏，不显示左侧模板 <br/>toolbar: \'default\' //让工具栏左侧显示默认的内置模板，默认值：(M)default、(L)false', '#tipstbToolbar', 1);
    micro.Tips('String，defaultToolbar: [\'filter\', \'print\', \'exports\']，例： [\'print\']。默认值(M)：[\'filter\', \'print\', \'exports\']', '#tipstbDefaultToolbar', 1);
    micro.Tips('String，定义 table 的大标题（在文件导出等地方会用到）', '#tipstbTitle', 1);
    micro.Tips('String，自定义文本，如空数据时的异常提示等。默认值(M)： No Data.', '#tipstbText', 1);
    micro.Tips('String，初始排序状态。用于在数据表格渲染完毕时，默认按某个字段排序。', '#tipstbInitSort', 1);
    micro.Tips('String，设定容器唯一 id。id 是对表格的数据操作方法上是必要的传递条件，它是表格容器的索引，你在下文诸多地方都将会见识它的存在。 该参数也可以自动从 <table id="test"></table> 中的 id 参数中获取。', '#tipstbID', 1);
    micro.Tips('String，设定表格各种外观、尺寸等, 输入的值：line<br/>line（行边框风格） <br/>row （列边框风格） <br/>nob （无边框风格）', '#tipstbSkin', 1);
    micro.Tips('Boolean 是否开启隔行背景，默认值(D)：False', '#tipstbEven', 1);
    micro.Tips('String，用于设定表格尺寸，若使用默认尺寸不设置该属性即可sm （小尺寸） lg （大尺寸）', '#tipstbSize', 1);
    micro.Tips('Int，设定容器宽度。table容器的默认宽度是跟随它的父元素铺满，你也可以设定一个固定值，当容器中的内容超出了该宽度时，会自动出现横向滚动条。', '#tipstbWidth', 1);
    micro.Tips('Int，设定容器高度', '#tipstbHeight', 1);
    micro.Tips('Int，全局定义所有常规单元格的最小宽度（默认(L)：60），一般用于列宽自动分配的情况。其优先级低于表头参数中的 minWidth', '#tipstbCellMinWidth', 1);
    micro.Tips('Boolean，开启分页（默认(L/D)：false） 注：从 layui 2.2.0 开始，支持传入一个对象，里面可包含 laypage 组件所有支持的参数（jump、elem除外）', '#tipstbPage', 1);
    micro.Tips('Int，每页显示的条数（默认(L)：10）。值务必对应 limits 参数的选项。优先级低于 page 参数中的 limit 参数。', '#tipstbLimit', 1);
    micro.Tips('String，每页条数的选择项，默认(L/M)：[10,20,30,40,50,60,70,80,90]。优先级低于 page 参数中的 limits 参数。', '#tipstbLimits', 1);
    micro.Tips('Boolean，是否开启合计行区域，默认值(L/D)：False', '#tipstbTotalRow', 1);
    micro.Tips('Boolean，是否显示加载条（默认(L/D)：true）。如果设置 false，则在切换分页时，不会出现加载条。该参数只适用于 url 参数开启的方式', '#tipstbLoading', 1);
    micro.Tips('Boolean，默认(L) true、(D)false， 即直接由 table 组件在前端自动处理排序。 若为 false，则需自主排序，通常由服务端直接返回排序好的数据。', '#tipstbAutoSort', 1);
    micro.Tips('String，数据渲染完的回调。你可以借此做一些其它的操作', '#tipstbDone', 1);
    micro.Tips('Boolean，开启获取主项和子项(无限子项，主要是对有父项和子项的数据，如菜单、部门等)，默认值(D)：false', '#tipstbMainSub', 1);
    micro.Tips('Int，自定义排序，在显示数据表格时的列名显示的顺序', '#tipscolCustomSort', 1);
    micro.Tips('String，显示数据表格时的列名的名称，当该字段不为空时显示，为空时则显示的是前面的Title字段', '#tipscolTitle', 1);
    micro.Tips('String，自定义字段，在动态生成LayuiDataTable表头时用于代替的默认的显示字段，通常用于跨表查询。如：用户表的用户的“所属部门”其实是部门表的DetpID，但实际要显示的却是DeptName，DeptName作为跨表查询的字段，此时该字段值填入“DeptName”时则显示的是部门名称。该字段通常配合控件的这三个字段来使用 ”ctlSourceTable”（代入Department）、”ctlTextName”（代入DeptName）、”ctlTextValue”（代入DeptID）', '#tipscolCustomField', 1);
    micro.Tips('String 200 or 30%设定列宽，若不填写，则自动分配；若填写，则支持值为：数字、百分比 请结合实际情况，对不同列做不同设定。<br/>特殊情况：宽度的值还可以设置为两个用逗号分隔“200,100”，第一个值代表管理员调用，第2个值非管理员调用，应用场景如全社表单操作列的宽度设置（“变更”按钮非管理员隐藏，但多出了宽度）', '#tipscolWidth', 1);
    micro.Tips('Int，局部定义当前常规单元格的最小宽度（默认：60），一般用于列宽自动分配的情况。其优先级高于基础参数中的 cellMinWidth', '#tipscolMinWidth', 1);
    micro.Tips('固定列。可选值有：left（固定在左）、right（固定在右）。一旦设定，对应的列将会被固定在左或右，不随滚动条而滚动。<br/>注意：如果是固定在左，该列必须放在表头最前面；如果是固定在右，该列必须放在表头最后面。', '#tipscolFixed', 1);
    micro.Tips('String，设定列类型。可选值有： <br/>normal（常规列，无需设定） <br/>checkbox（复选框列） <br/>radio（单选框列，layui 2.4.0 新增） <br/>numbers（序号列） <br/>space（空列），默认值(M)：checkbox', '#tipscolType', 1);
    micro.Tips('String，单元格对齐方式。可选值有：left（居左）、center（居中）、right（居右），默认值：(L)left、(M)center', '#tipscolAlign', 1);
    micro.Tips('String，单元格编辑类型（默认(L)不开启）目前只支持：text（输入框）,默认(M)：text', '#tipscolEdit', 1);
    micro.Tips('String，自定义单元格点击事件名，以便在 tool 事件中完成对该单元格的业务处理，存放的是< script>代码块内的lay- event="test"的值，输入的值：test', '#tipscolEvent', 1);
    micro.Tips('String，自定义单元格样式。即传入 CSS 样式', '#tipscolStyle', 1);
    micro.Tips('Boolean，LAY_CHECKED <br/>是否全选状态（默认(L/D)：false）。必须复选框列开启后才有效，如果设置 true，则表示复选框默认全部选中。', '#tipscolCheckedAll', 1);
    micro.Tips('Boolean，是否允许排序（默认(L/D)：false）。如果设置 true，则在对应的表头显示排序icon，从而对列开启排序功能。', '#tipscolSort', 1);
    micro.Tips('Boolean，是否初始隐藏列，默认(L/D)：false', '#tipscolHide', 1);
    micro.Tips('Boolean，是否禁用拖拽列宽（默认(L/D)：false）。默认情况下会根据列类型（type）来决定是否禁用，如复选框列，会自动禁用。而其它普通列，默认允许拖拽列宽，当然你也可以设置 true 来禁用该功能。', '#tipscolUnReSize', 1);
    micro.Tips('Boolean，是否开启该列的自动合计功能，默认(L/D)：false', '#tipscolTotalRow', 1);
    micro.Tips('String，用于显示自定义的合计文本的名称。例：汇总', '#tipscolTotalRowText', 1);
    micro.Tips('String，自定义列模板，模板遵循 laytpl 语法。这是一个非常实用的功能，你可借助它实现逻辑处理，以及将原始数据转化成其它格式，如时间戳转化为日期字符等，对应的值是页面< script>代码块的ID，当该值为空值时且DataType为bit类型时，在生成Layui数据表时该值由“#sw”+TabColName组成', '#tipscolTemplet', 1);
    micro.Tips('String，行Toolbar,单独作为cols的一行，不需要携带field，放在行的最右侧如，编辑、审核、删除等（\,{fixed: \'right\', width:150, align:\'center\', toolbar: \'#barDemo\'}），页面需存在#barDemo对应的toolbar模块，对应的值是页面< script>代码块的ID', '#tipscolToolbar', 1);
    micro.Tips('Boolean，是否加入表格列，即创建Layui数据表格时是否显示该列，紧在创建表头时加入该值作为条件查询，实际返回的数据表的记录是不作为查询条件的。默认值(D)：True', '#tipsJoinTableColumn', 1);
    micro.Tips('Bit，是否为主键作为更新时的主键，字段名ctlPrimaryKey与PrimaryKey（每个表的默认ID）字段可二选一，其优先级高于PrimaryKey，且默认也调用PrimaryKey<br/>*特别注意：主键是具有唯一性的并且对应的值也是唯一性的，所以一个表不能够设定两个主键，一但设置错误会导致系统停止运行的所可能，非管理员操作建议使用默认值。默认值(D)：False。', '#tipsctlPrimaryKey', 1);
    micro.Tips('生成表单的风格。默认风格：像本页面的风格一样； 方框风格：在标题加上方框', '#tipsctlFormStyle', 1);
    micro.Tips('Bit，是否显示星号。必填项时星号为红色否则灰色。默认值(D)：True', '#tipsctlDisplayAsterisk', 1);
    micro.Tips('Bit，新增表单情况下显示该控件。默认值(D)：True', '#tipsctlAddDisplay', 1);
    micro.Tips('Bit，新增表单情况下显示提交按钮。默认值(D)：True', '#tipsctlAddDisplayButton', 1);
    micro.Tips('Bit，新增表单情况下显示临时保存按钮。默认值(D)：True', '#tipsctlSaveDraftButton', 1);
    micro.Tips('Bit，修改表单情况下显示该控件。默认值(D)：True', '#tipsctlModifyDisplay', 1);
    micro.Tips('Bit，修改表单情况下显示提交按钮。默认值(D)：True', '#tipsctlModifyDisplayButton', 1);
    micro.Tips('Bit，查看表单情况下显示该控件（即是否显示该条数据）。默认值(D)：True', '#tipsctlViewDisplay', 1);
    micro.Tips('Bit，查看表单情况下通过Label标签显示数据不用Input等控件。默认值(D)：True', '#tipsctlViewDisplayLabel', 1);
    micro.Tips('Bit，控件后显示，通常在申请者提交表单后，需要受理方受理时填写的控件。默认值(D)：True', '#tipsctlAfterDisplay', 1);
    micro.Tips('String，控件标题，即在label显示的titel，例：空值不显示label，=space 显示一个空的label（即有label但名称是空的）', '#tipsctlTitle', 1);
    micro.Tips('String，控件标题样式（即label样式），一个完整的 style', '#tipsctlTitleStyle', 1);
    micro.Tips('String，可选值有【Text，Date，Password，Hidden，Radio，CheckBox，Select，RadioTreeSelect，CheckBoxTreeSelect，RadioCascaderSelect，CheckBoxCascaderSelect，ImgUpload，FileUpload，Textarea，LayEdit】当控件类型为CheckBox，Radio，Upload时默认生成Text接收选中的值', '#tipsctlTypes', 1);
    micro.Tips('String，控件名称的前缀，如UserName->txtUserName。输入的值：txt', '#tipsctlPrefix', 1);
    micro.Tips('String，控件样式，一个完整的 style=&quot;width:300px;&quot;  它不紧紧只是一个样式你还可以这样写：  style=&quot;width:300px;&quot; readonly=&quot;readonly&quot;，即代表控件宽度为300px，并且是只读属性', '#tipsctlStyle', 1);
    micro.Tips('String，占位符，即text的水印符，目前只对text,和Textarea生效', '#tipsctlPlaceholder', 1);
    micro.Tips('String，复选框风格。当控件类型为CheckBox时，可进行风格的设定，此属性对其它控件无效。可选值：复选框风格、原始风格、开关风格', '#tipsctlCheckboxSkin', 1);
    micro.Tips('String，开关的文本。当Checkbox风格为开关风格时，开关显示的文本，此属性对其它控件无效。如：是|否', '#tipsctlSwitchText', 1);
    micro.Tips('String，描述。即对控件进行作用的描述，空值则不显示', '#tipsctlDescription', 1);
    micro.Tips('String，描述的显示位置，前面(before)：即紧跟在标题后面（像本标签一样）；后面(after)：即显示在HTML控件后面；标签上(label)：在标签上鼠标移入显示，当该值为空时则不显示描述。默认值(D)：after（后面）', '#tipsctlDescriptionDisplayPosition', 1);
    micro.Tips('Bit，描述的显示方式，鼠标移过提示形式或直接显示。默认是提示形式，默认值(D)：False', '#tipsctlDescriptionDisplayMode', 1);
    micro.Tips('String，当描述为直接显示时还需要一个div层存放描述的内容，如有需要可以对这个div层进行的样式的设定。一个完整的style代码', '#tipsctlDescriptionStyle', 1);
    micro.Tips('String，控件值，即在生成控件时为控件赋值，当控件类型为CheckBox，Radio或Select时，可根据该值生成该类控件，且该值可从“数据来源”进行取值，但该值不为空时其优先级要高于“数据来源”。该值可以设置多个，使用英文逗号进行分隔即可，如：男,女。<br/>且同时控件类型为CheckBox，Radio或Select还可以通过冒号来区分显示名称和值，其格式： Name:Value 如：男:True,女:False。<br/>除此外该值还可以填入系统内置函数：GetDateTimeNow【返回当前系统时间】、GetClientIP【返回客户端IP】<br/>***注：控件值和控件默认值的关系是，当控件类型为TextBox这一类的，值和默认值随便选填一个都可以，当控件类型为Select\CheckBox\Radio这一类的，则值是用来显示有多少个选项，默认值则是让这类控件默认选中的功能，所以填写时需要注意。控件值与控件默认值的区别是，控件值：在控件生成时生成值，控件默认值：是控件在提交表单时生成需要返回的值', '#tipsctlValue', 1);
    micro.Tips('String，控件默认值，在提交表单时根据默认值返回实际值。当控件类型为TextBox时默认值直接调用ctlValue【控件值】，且当控件类型为CheckBox，Radio或Select时，默认值即为该类控件默认选中。例：一个性别的Radio两个选项为 男:True,女:False，如果想让“女”默认选中则填入的默认值为False<br/>除此外该值还可以填入系统内置函数：GetDateTimeNow【返回当前系统时间】、GetClientIP【返回客户端IP】<br/>***注：控件值和控件默认值的关系是，当控件类型为TextBox这一类的，值和默认值随便选填一个都可以，当控件类型为Select\CheckBox\Radio这一类的，则值是用来显示有多少个选项，默认值则是让这类控件默认选中的功能，所以填写时需要注意。控件值与控件默认值的区别是，控件值：在控件生成时生成值，控件默认值：是控件在提交表单时生成需要返回的值', '#tipsctlDefaultValue', 1);
    micro.Tips('String，额外函数，这是一个无返回值的函数。通常在提交表单时需要执行的额外操作', '#tipsctlExtraFunction', 1);
    micro.Tips('String，接收名称【暂时没有启用20190903】，在提交表单时的变量名称，当为空值时默认由：ctlPrefix+TabColName（前缀+字段名称组成）', '#tipsctlReceiveName', 1);
    micro.Tips('String，当控件类型为CheckBox，Radio，Select时，它的数据来源表。当”控件值ctlValue“为空时数据来源才生效', '#tipsctlSourceTable', 1);
    micro.Tips('String，显示的字段名', '#tipsctlTextName', 1);
    micro.Tips('String，值的字段', '#tipsctlTextValue', 1);
    micro.Tips('String，控件扩展JS代码（紧对当前控件）。例如：当控件要输入日期时可以通过这个JS属性进行绑定日期插件等，通常对Text控件进行设定。代码示例：<br/> laydate.render({  <br/> elem: \'#test\' //或 elem: document.getElementById(\'test\')、elem: lay(\'#test\') 等<br/> });', '#tipsctlExtendJSCode', 1);
    micro.Tips('String，字符长度，通常针对text控件进行字符长度限制，该值不为空或0时才生效。例1单个限制：999->允许最大长度为999，例2区间范围限制（逗号分隔）：10,999->允许长度在10~999之间（同时表示该项为必填项）。默认值(D)：0或空即不限制长度', '#tipsctlCharLength', 1);
    micro.Tips('String，事件过滤器，主要用于事件的精确匹配，跟选择器是比较类似的。其实它并不私属于form模块，它在 layui 的很多基于事件的接口中都会应用到。例：lay-filter=“test”输入的值：test。空值时默认调前缀+用字段名（ctlPrefix+TabColName）', '#tipsctlFilter', 1);
    micro.Tips('String，调用CSS样式名称作为验证，多个验证分隔符”空格“。可选值有：<br/>限制只能输入数字：onlyNum，<br/>限制只能输入字母：onlyAlpha，<br/>限制只能输入数字和字母：onlyNumAlpha，<br/>限制只能输入数字和小数点：onlyNumPoint，<br/>限制只能输入数字和逗号：onlyNumCom', '#tipsctlVerify', 1);
    micro.Tips('String，内容验证，多个验证分隔符“|”。可选值有：必填项：required <br/> 手机号：phone<br/>邮箱：email<br/>网址：url<br/>数字：number <br/> 日期：date <br/> 身份证：identity <br/>自定义名称，为自定义时请在下方文本域填写自定义函数', '#tipsctlVerify2', 1);
    micro.Tips('String，这是运行在form.verify({ })代码块内的自定义代码，当验证2包含自定义时，请填写自定义函数。 例子： <br/> Example: function (value, item) {if (value.length <= 0 || value.length > 100) return \'只允许在100字以内。< br/> Only allowed 100 words.\'; }  Example是函数名称', '#tipsctlVerifyCustomFunction', 1);
    micro.Tips('Int，组别数字类型，相同的数字为同一组，把同一个组的控件放在同一个FormItem下。 默认值(D)：0  ，但在程度创建控件时默认是Sort+1的方式（即每个控件单独一组）', '#tipsctlGroup', 1);
    micro.Tips('String，组别描述，像当面一样用横线分割的文字，空值时不显示分割线。', '#tipsctlGroupDescription', 1);
    micro.Tips('String，FormItem的样式。一个完整的style代码。', '#tipsctlFormItemStyle', 1);
    micro.Tips('String，FormItem下的Inline的Css，多个Css空格隔开，配合xs/sm/md/lg实现不同屏幕下的排版显示状态。默认值：layui-inline （layui-inline是一个固定宽度的类可不填）', '#tipsctlInlineCss', 1);
    micro.Tips('String，FormItem下的Inline的样式。一个完整的style代码', '#tipsctlInlineStyle', 1);
    micro.Tips('String，列宽数字类型，同一行内的控件元素的间隔，同一行内所设定的数值应该是一样的。可选值【1，3，5，10，15，20，25，30】，默认值：(M)layui-col-space5', '#tipsctlColSpace', 1);
    micro.Tips('String，对列追加 类似 layui-col-md-offset* 的预设类，从而让列向右偏移。其中 * 号代表的是偏移占据的列数，md可选值为：lg/md/sm/xs，数字可选值为： 1 - 12。 <br/>如：layui-col-md-offset3，即代表在“中型桌面屏幕”下，让该列向右偏移3个列宽度，多种组合请用空格分开', '#tipsctlOffset', 1);
    micro.Tips('String，超小屏幕如手机，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。', '#tipsctlXS', 1);
    micro.Tips('String，中午屏幕如平板，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。', '#tipsctlSM', 1);
    micro.Tips('String，一般屏幕如桌面PC，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。', '#tipsctlMD', 1);
    micro.Tips('String，超大屏幕如电视机，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。', '#tipsctlLG', 1);
    micro.Tips('String，存放Input控件的div，可选值【inline、block】,默认值(M)：inline', '#tipsctlInputInline', 1);
    micro.Tips('String，InputInline样式，一个完整的style代码', '#tipsctlInputInlineStyle', 1);
    micro.Tips('Text，自定义HTML代码，在生成表单（新增或修改表单）时用，该代码放置layui-card-body内，在所有动态控件生成之后，弥补动态生成的不足之处。', '#tipsCustomHtmlCode', 1);

    //查询设置
    micro.Tips('Int，查询排序', '#tipsquerySort', 1);
    micro.Tips('String，查询标题', '#tipsqueryTitle', 1);
    micro.Tips('Bit，作为基础查询字段，默认值(M)：False', '#tipsqueryAsBaseField', 1);
    micro.Tips('Bit，作为高级查询字段，默认值(M)：False', '#tipsqueryAsAdvancedField', 1);
    micro.Tips('Bit，作为关键字查询字段，通常在模糊查询时构造成查询语句，默认值(D)：False', '#tipsqueryAsKeywordField', 1);
    micro.Tips('String，查询字段呈现的控件', '#tipsqueryCtlTypes', 1);



});
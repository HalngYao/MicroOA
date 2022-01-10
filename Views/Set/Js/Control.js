layui.use(['index', 'jquery', 'table', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');
    var mGet = {
        getTxtValGroup: function () {
            //selParentID bind
            var Fields = { "txt": "TabColName", "val": "TID", "stn": "mTabs", "lv": "False", "ob": "ParentID,Sort", "note": "Title" };
            micro.getTxtValGroup('get', 'selTabColName', micro.getRootPath() + '/App_Handler/GetParSubTextValue.ashx', Fields);
        },
        getTxtValSrcTab: function () {
            var Fields = { "txt": "TabColName", "val": "TabColName", "stn": "mTabs", "ob": "ParentID,Sort" };
            micro.getTxtVal('getalltn', 'ctlSourceTable', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields);
        },
        getTextName: function (TabColName, Selected) {
            var Fields = { "txt": "TabColName", "val": "TabColName", "stn": "mTabs", "tcn": "" + TabColName + "", "ob": "Sort" };
            micro.getTxtVal('get', 'ctlTextName', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields, Selected);
        },
        getTextValue: function (TabColName, Selected) {
            var Fields = { "txt": "TabColName", "val": "TabColName", "stn": "mTabs", "tcn": "" + TabColName + "", "ob": "Sort" };
            micro.getTxtVal('get', 'ctlTextValue', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields, Selected);
        },
        getSingleList: function (TID) {
            var Fields = { "TID": "" + TID + "" };
            Fields = encodeURI(Fields);
            var Parameter = { "action": "get", "mid": MID, "fields": Fields };
            micro.mAjax('json', micro.getRootPath() + '/Views/Set/System/ControlList.ashx', Parameter);
        }

    };


    //调用mGet函数selTabColName bind
    mGet.getTxtValGroup();
    //ctlSourceTable bind
    mGet.getTxtValSrcTab();

    form.on('select(ctlSourceTable)', function (data) {
        var v = data.value;
        mGet.getTextName(v);  //ctlTextName bind
        mGet.getTextValue(v);  //ctlTextValue bind
    });

    form.on('select(selTabColName)', function (data) {

        var Fields = { "TID": "" + data.value + "" };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { "action": "get", "mid": MID, "fields": Fields };
        var v = micro.getAjaxData('json', micro.getRootPath() + '/Views/Set/System/ControlList.ashx', Parameter);

        mGet.getTextName(v["data"][0].ctlSourceTable, v["data"][0].ctlTextName);   //ctlTextName bind
        mGet.getTextValue(v["data"][0].ctlSourceTable, v["data"][0].ctlTextValue);  //ctlTextValue bind

        form.val("micro-form", {

            "Invalid": v["data"][0].Invalid == "True" ? true : false,
            "ctlPrimaryKey": v["data"][0].ctlPrimaryKey == "True" ? true : false,
            "ctlFormStyle": v["data"][0].ctlFormStyle == "" ? "Default" : v["data"][0].ctlFormStyle,
            "ctlDisplayAsterisk": v["data"][0].ctlDisplayAsterisk == "True" ? true : false,
            "ctlAddDisplay": v["data"][0].ctlAddDisplay == "True" ? true : false,
            "ctlAddDisplayButton": v["data"][0].ctlAddDisplayButton == "True" ? true : false,
            "ctlModifyDisplay": v["data"][0].ctlModifyDisplay == "True" ? true : false,
            "ctlModifyDisplayButton": v["data"][0].ctlModifyDisplayButton == "True" ? true : false,
            "ctlViewDisplay": v["data"][0].ctlViewDisplay == "True" ? true : false,
            "ctlViewDisplayLabel": v["data"][0].ctlViewDisplayLabel == "True" ? true : false,
            "ctlAfterDisplay": v["data"][0].ctlAfterDisplay == "True" ? true : false,
            "ctlTitle": v["data"][0].ctlTitle,
            "ctlTitleStyle": v["data"][0].ctlTitleStyle,
            "ctlTypes": v["data"][0].ctlTypes,
            "ctlPrefix": v["data"][0].ctlPrefix,
            "ctlStyle": v["data"][0].ctlStyle,
            "ctlPlaceholder": v["data"][0].ctlPlaceholder,
            "ctlCheckboxSkin": v["data"][0].ctlCheckboxSkin,
            "ctlSwitchText": v["data"][0].ctlSwitchText,
            "ctlDescription": v["data"][0].ctlDescription,
            "ctlDescriptionDisplayPosition": v["data"][0].ctlDescriptionDisplayPosition,
            "ctlDescriptionDisplayMode": v["data"][0].ctlDescriptionDisplayMode == "True" ? true : false,
            "ctlDescriptionStyle": v["data"][0].ctlDescriptionStyle,
            "ctlValue": v["data"][0].ctlValue,
            "ctlDefaultValue": v["data"][0].ctlDefaultValue,
            "ctlExtraFunction": v["data"][0].ctlExtraFunction,
            "ctlReceiveName": v["data"][0].ctlReceiveName,
            "ctlSourceTable": v["data"][0].ctlSourceTable,
            //"ctlTextName": v["data"][0].ctlTextName,
            //"ctlTextValue": v["data"][0].ctlTextValue,
            "ctlExtendJSCode": v["data"][0].ctlExtendJSCode,
            "ctlCharLength": v["data"][0].ctlCharLength,
            "ctlFilter": v["data"][0].ctlFilter,
            "ctlVerify": v["data"][0].ctlVerify,
            "ctlVerify2": v["data"][0].ctlVerify2,
            "ctlVerifyCustomFunction": v["data"][0].ctlVerifyCustomFunction,
            "ctlGroup": v["data"][0].ctlGroup,
            "ctlGroupDescription": v["data"][0].ctlGroupDescription,
            "ctlFormItemStyle": v["data"][0].ctlFormItemStyle,
            "ctlInlineCss": v["data"][0].ctlInlineCss,
            "ctlInlineStyle": v["data"][0].ctlInlineStyle,
            "ctlColSpace": v["data"][0].ctlColSpace,
            "ctlOffset": v["data"][0].ctlOffset,
            "ctlXS": v["data"][0].ctlXS,
            "ctlSM": v["data"][0].ctlSM,
            "ctlMD": v["data"][0].ctlMD,
            "ctlLG": v["data"][0].ctlLG,
            "ctlInputInline": v["data"][0].ctlInputInline,
            "ctlInputInlineStyle": v["data"][0].ctlInputInlineStyle,
            "CustomHtmlCode": v["data"][0].CustomHtmlCode,

        });

        form.render();

    });


    form.on('submit(btnModify)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURIComponent(Fields);
        var Parameter = { "action": "modify", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/Set/System/Control.ashx', Parameter);
    });

});
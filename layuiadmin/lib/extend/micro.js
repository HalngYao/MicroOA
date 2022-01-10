layui.define(function (exports) {
    var $ = layui.jquery
        , form = layui.form
        , layer = layui.layer
        , table = layui.table
        , laydate = layui.laydate;

    var micro = {

        //获取根路径
        getRootPath: function getRootPath() {
            var curPageUrl = window.document.location.href;
            var rootPath = "";
            rootPath = curPageUrl.split("//")[1].split("/")[0];
            rootPath = rootPath.slice(0, 9);
            if (rootPath === "localhost")
                rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0];
            else
                rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0];

            return rootPath;
        },

        //获取URL参数值
        getUrlPara: function (name) { //name为URL参数值如“test.aspx?id=t1”中的id的值
            if (name === "mid") {
                return $("#txtMID").val();
            }
            else {
                var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"), flag = "";
                var r = window.location.search.substr(1).match(reg);
                if (r != null)
                    flag = unescape(r[2]);

                if (typeof flag !== "undefined")
                    return flag;  //返回参数值
                else
                    return "";
            }
        },

        //ajax同步post提交数据,return msg
        mAjax: function (DataType, URL, Parameter, isLoad, isRefresh, isCloseParent, isRepeatSubmit) {
            var flag = 'False';
            isLoad = isLoad || true;  //isLoad = typeof isLoad === 'undefined' ? true : isLoad;
            isRefresh = isRefresh || false;
            isCloseParent = isCloseParent || false;
            isRepeatSubmit = isRepeatSubmit || false;  //重复提交，默认禁止，默认值False

            $.ajax({
                async: true,
                type: 'post',
                dataType: DataType,
                url: URL,
                data: Parameter,
                cache: false,
                beforeSend: function () {
                    if (isLoad) //是否开启加载图标
                        layer.load(2);

                    //防止重新提交表单
                    if ($("#btnSave").length > 0 && !isRepeatSubmit) 
                        $("#btnSave").prop('disabled', true);

                },
                success: function (data) {
                    //加载层
                    if (isLoad)
                        layer.close(layer.load(2));

                    flag = data;

                    if (flag.substring(0, 4) === 'True') {

                        //母板页上的状态标记，默认值False，新增或修改数据成功时改变状态为True，用于关闭窗口是否刷新页面
                        $('#txtState').val('True');
                        $(window.parent.document).find('#txtState').val('True');

                        msg = layer.msg(flag.substring(4, flag.length), {
                            icon: 1
                            , anim: 5
                            , offset: 't'
                            , skin: 'ws-margin-top10'
                        }, function () {

                            if (isCloseParent) {
                                //当你在iframe页面关闭自身时
                                var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                                parent.layer.close(index); //再执行关闭 
                            }

                            if (isRefresh) //层销毁是否刷新页面
                                location.reload();
                        });

                    } else {

                        //如果保存失败解锁防止重新提交表单
                        if ($("#btnSave").length > 0)
                            $("#btnSave").prop('disabled', false);

                        msg = layer.msg(flag, {
                            icon: 5
                            , anim: 6
                            , offset: 't'
                        }, function () {
                            //return false;
                        });
                    }
                    return msg;
                },
                error: function () {
                    if (isLoad)
                        layer.close(layer.load(2));

                    //如果表单提交失败解锁防止重新提交表单
                    if ($("#btnSave").length > 0)
                        $("#btnSave").prop('disabled', false);

                    flag = "error,出错了";

                    msg = layer.msg(flag, {
                        icon: 5
                        , anim: 6
                        , offset: 't'
                    }, function () {
                        //return false;
                    });
                    return msg;
                }
            });
        },

        //ajax同步post提交数据,不返回任何消息
        mAjaxNoMsg: function (DataType, URL, Parameter, isRefresh) {
            isRefresh = isRefresh || false;
            $.ajax({
                async: true,
                type: 'post',
                dataType: DataType,
                url: micro.getRootPath() + URL,
                data: Parameter,
                cache: false,
                beforeSend: function () {

                },
                success: function (data) {
                    if (isRefresh) //是否刷新页面
                        location.reload();
                },
                error: function () {

                }
            });
        },

        //ajax异步（锁定）提交数据POST方式,return Data
        getAjaxData: function (DataType, URL, Parameter, isLoad) {
            var flag = 'False';
            isLoad = typeof isLoad === 'undefined' ? true : isLoad;
            $.ajax({
                async: false,
                type: 'post',
                dataType: DataType,
                url: URL,
                data: Parameter,
                cache: false,
                beforeSend: function () {
                    if (isLoad)
                        layer.load(2);
                },
                success: function (data) {
                    flag = data;
                    layer.close(layer.load(2));
                },
                error: function () {
                    flag = "error,出错了";
                    layer.close(layer.load(2));
                }
            });
            return flag;
        },


        //ajax同步（不锁定）提交数据POST方式,return Data
        mAjaxData: function (DataType, URL, Parameter, isLoad) {
            var flag = 'False';
            isLoad = typeof isLoad === 'undefined' ? true : isLoad;
            $.ajax({
                async: true,
                type: 'post',
                dataType: DataType,
                url: URL,
                data: Parameter,
                cache: false,
                beforeSend: function () {
                    if (isLoad)
                        layer.load(2);
                },
                success: function (data) {
                    flag = data;
                    layer.close(layer.load(2));
                },
                error: function () {
                    flag = "error,出错了";
                    layer.close(layer.load(2));
                }
            });
            return flag;
        },

        //ajax异步（锁定）提交数据Get方式,return Data
        getGetAjaxData: function (DataType, URL, Parameter, isLoad) {
            var flag = 'False';
            isLoad = typeof isLoad === 'undefined' ? true : isLoad;
            $.ajax({
                async: false,
                type: 'get',
                dataType: DataType,
                url: URL,
                data: Parameter,
                cache: false,
                beforeSend: function () {
                    if (isLoad)
                        layer.load(2);
                },
                success: function (data) {
                    flag = data;
                    layer.close(layer.load(2));
                },
                error: function () {
                    flag = "error,出错了";
                    layer.close(layer.load(2));
                }
            });
            return flag;
        },

        //ajax异步提交数据,return DataEval
        getAjaxDataEval: function (DataType, URL, Parameter) {
            $.ajax({
                async: true,
                type: 'post',
                dataType: DataType,
                url: micro.getRootPath() + URL,
                data: Parameter,
                cache: false,
                beforeSend: function () {
                },
                success: function (data) {
                    eval(data);
                },
                error: function () {
                }
            });
        },

        //Post获取Text，Value; 对select不分组
        getTxtVal: function (Action, CtrlID, URL, Fields, selected) {
            var MID = $("#txtMID").val();
            if (URL === "")
                URL = micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx';

            Fields = encodeURI(JSON.stringify(Fields));
            if ($("#" + CtrlID).length > 0) {  //判断元素是否存在
                var Parameter = { "action": Action, "mid": MID, "fields": Fields };
                $.post(URL, Parameter, function (data) {
                    if (data !== 'False' && data !== '') {
                        document.getElementById(CtrlID).options.length = 1;
                        $.each(JSON.parse(data), function (index, value) {
                            if (value.value == selected) //根据selected选中默认值
                                $("#" + CtrlID).append("<option value='" + value.value + "' level='" + value.numLevel + "' selected='selected'>" + value.text + "</option>");
                            else
                                $("#" + CtrlID).append("<option value='" + value.value + "' level='" + value.numLevel + "'>" + value.text + "</option>");

                        }, 'json');
                        form.render();
                    }
                });
            }
        },

        //Post获取Text，Value; 对select分组
        getTxtValGroup: function (Action, CtrlID, URL, Fields) {
            var MID = $("#txtMID").val();
            if (URL === "")
                URL = micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx';
            Fields = encodeURI(JSON.stringify(Fields));
            if ($("#" + CtrlID).length > 0) {  //判断元素是否存在
                var Parameter = { "action": Action, "mid": MID, "fields": Fields };
                $.post(URL, Parameter, function (data) {
                    if (data !== 'False' && data !== '') {
                        document.getElementById(CtrlID).options.length = 1;
                        $.each(JSON.parse(data), function (index, value) {
                            if (value.ParentID == "0") {
                                $("#" + CtrlID).append("<optgroup label='" + value.text + "'>");

                                $.each(JSON.parse(data), function (index2, value2) {
                                    if (value2.ParentID == value.value)
                                        $("#" + CtrlID).append("<option value='" + value2.value + "' level='" + value2.numLevel + "'>" + value2.text + "</option>");

                                }, 'json');

                                $("#" + CtrlID).append("</optgroup>");
                            }
                        }, 'json');
                        form.render();
                    }
                });
            }
        },

        //获取表格的属性，然后创建layui数据表
        getTableAttributes: function (action, stn, keyword, queryType, moduleID, formID, dataURL, dataType, startDate, endDate) {
            var parms = '',
                action = action || 'View',
                keyword = keyword || '',
                queryType = queryType || 'All', //All or Field or Field:DataType
                moduleID = moduleID || $("#txtMID").val(),
                dataURL = dataURL || '',  //暂时未用
                dataType = dataType || '',  //GetPersonalOvertime
                startDate = startDate || '',
                endDate = endDate || '';

            if (typeof (stn) !== "undefined" && stn !== "" && stn !== null)
                parms = '?Action=' + action + '&STN=' + stn + '&ModuleID=' + moduleID + '&FormID=' + formID + '&DataType=' + dataType + '&QueryType=' + queryType + '&Keyword=' + keyword + '&StartDate=' + startDate + '&EndDate=' + endDate + '&random=' + Math.random();

            URL = micro.getRootPath() + '/App_Handler/GetTableAttributes.ashx' + parms;

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: URL,
                data: '',
                cache: false,
                beforeSend: function () {
                    //return flag;
                },
                success: function (v) {

                    var tbURL = dataURL;
                    if (dataURL == '')
                        tbURL = v.data[0].tbURL

                    table.render({
                        elem: v.data[0].tbElem
                        , url: micro.getRootPath() + tbURL + parms //异步数据接口,get方式
                        , data: ''
                        , toolbar: v.data[0].tbToolbar == 'True' ? true : (v.data[0].tbToolbar == 'False' ? false : v.data[0].tbToolbar)  //值为bit和string类型
                        , defaultToolbar: v.data[0].tbDefaultToolbar !== "" ? eval("(" + v.data[0].tbDefaultToolbar + ")") : "" //v.data[0].tbDefaultToolbar
                        , title: v.data[0].tbTitle
                        , text: { none: v.data[0].tbText }
                        , initSort: v.data[0].tbInitSort !== "" ? eval("(" + v.data[0].tbInitSort + ")") : ""
                        , skin: v.data[0].tbSkin
                        , even: v.data[0].tbEven === 'True' ? true : false
                        , size: v.data[0].tbSize
                        , width: parseInt(v.data[0].tbWidth)
                        , height: v.data[0].tbHeight !== "0" ? 'full-' + v.data[0].tbHeight : ''
                        , cellMinWidth: parseInt(v.data[0].tbCellMinWidth)
                        , cols: [eval(v.data[0].Cols)]
                        , page: v.data[0].tbPage === 'True' ? true : false
                        , limit: parseInt(v.data[0].tbLimit)
                        , limits: v.data[0].tbLimits !== "" ? eval("(" + v.data[0].tbLimits + ")") : ""
                        , totalRow: v.data[0].tbTotalRow === 'True' ? true : false
                        , loading: v.data[0].tbLoading === 'True' ? true : false
                        , autoSort: v.data[0].tbAutoSort === 'True' ? true : false
                        , done: function () {
                            eval(v.data[0].tbDone);
                            eval(v.data[0].Tips);
                            eval(v.data[0].CtrlTableContJS);  //生成监听编辑单元格和监听switch change事件代码
                        }
                    });

                },
                error: function () {
                    flag = 'error,出错了';
                }
            });

        },

        //鼠标移入移出提示层
        Tips: function (Content, CtrlID, Index) {
            var tipsIndex = 0;
            $(CtrlID).on('mouseover', function () {
                tipsIndex = layer.tips(Content, CtrlID, {
                    time: 0
                    , tips: Index
                    //tipsMore: true
                });
            }).on('mouseout', function () {
                layer.close(tipsIndex);
            });
        },

        //字符串拼接
        //传入源字符串对象SourceData，通过for循环取得需要的字段，用指定符号进行分，isRepeat=false时取不重复值
        getStrSplice: function (SourceData, Field, Symbol, isRepeat) {
            var flag = '';
            Symbol = typeof Symbol === 'undefined' ? ',' : Symbol;
            isRepeat = typeof isRepeat === 'undefined' ? true : isRepeat;

            if (SourceData.length > 0) {

                for (var i = 0; i < SourceData.length; i++) {
                    var v = SourceData[i][Field].trim();
                    //取不重复的值
                    if (!isRepeat) {
                        var arrFlag = flag.split(Symbol),
                            num = 0;
                        //判断是否存在重复，如果重复num++
                        for (var j = 0; j < arrFlag.length; j++) {
                            if (arrFlag[j].trim() == v)
                                num = num + 1;
                        }
                        if (num == 0)
                            flag += v + Symbol;

                    } else
                        flag += v + Symbol;
                }
                flag = flag.substr(0, flag.length - 1);

            } else
                flag = 'False';

            return flag;
        },

        verifyPWD: function (PWD) {
            var flag = 'False';
            var Parameter = { "pwd": PWD };
            $.ajax({
                async: false,
                type: 'post',
                dataType: 'text',
                url: micro.getRootPath() + '/App_Handler/VerifyPWD.ashx',
                data: Parameter,
                beforeSend: function () {
                },
                success: function (data) {
                    flag = data;
                    if (flag.substring(0, 4) !== 'True') {
                        layer.msg(flag, {
                            icon: 5
                            , anim: 6
                            , offset: 't'
                            , skin: 'ws-margin-top10'
                        }, function () {
                            //return false;
                        });
                    }
                },
                error: function () {
                    flag = "出错了";
                }
            });
            return flag;
        },

        //获取两个时间的时间差，把值写进文本框
        getTimeDiff: function (elmID, startTime, endTime) {
            var t1 = new Date('2020-01-01 ' + startTime);
            var t2 = new Date('2020-01-01 ' + endTime);
            var s1 = t1.getTime();
            var s2 = t2.getTime();
            if (s2 <= s1) {
                layer.msg('结束时间不能小于开始时间<br/>The end time cannot be less than the start time', {
                    icon: 5
                    , anim: 6
                    , offset: 't'
                    , skin: 'ws-margin-top10'
                }, function () {
                    // $('#txtOvertimeTime').val('');
                });

                return false;
            } else {
                if (elmID !== '') {
                    var s = (s2 - s1) / 1000 / 60 / 60;
                    $('#' + elmID).val(s);
                }
            }
        },

        //返回今天日期yyyy-MM-dd
        getToDay: function () {
            var currDate = new Date;
            var year = currDate.getFullYear(); //获取当前年
            var mon = currDate.getMonth() + 1; //获取当前月
            mon = mon < 10 ? '0' + mon : mon;  //不足两位数补0
            var day = currDate.getDate(); //获取当前天
            day = day < 10 ? '0' + day : day;

            var Today = year + '-' + mon + '-' + day;  //今天

            return Today;
        },

        //动态获取下拉框的option
        getSelectOption: function (elemID, url, formID, date, typeID, val, defauleVal) {
            $.getJSON(getRootPath() + url, { "formid": formID, "date": date, "typeid": typeID, "val": val }, function (data) {  //参数没有固定的作用可参考{ "overtimeDate": overtimeDate, "shiftTypeID": shiftTypeID, "overtimeUID": overtimeUID }
                document.getElementById(elemID).options.length = 0; //清空值
                $.each(data, function (index, value) {
                    if (value.value == defauleVal)
                        $('#' + elemID).append('<option value=" ' + value.value + '" selected="selected">' + value.text + '</option>');
                    else
                        $('#' + elemID).append('<option value=" ' + value.value + '">' + value.text + '</option>');

                    form.render();
                });

            });

        },

        //检查浏览器版本
        checkBrowser: function (flag, tips) {
            if (flag == "True") {
                layer.open({
                    type: 1,
                    skin: 'ws-checkbrowser-tips',
                    closeBtn: 0,
                    area: ['500px', '300px'],
                    shade: 0.1,
                    title: false,
                    content: decodeURIComponent(decodeURI(tips)).replace(/\+/g, ' ')
                });
            }
        },

        //比较两个日期的大小，传入两个日期如果date1>date2则返回true,否则返回false
        checkBothDate: function (date1, date2) {
            var oDate1 = new Date(date1);
            var oDate2 = new Date(date2);
            if (oDate1.getTime() > oDate2.getTime())
                return true;
            else
                return false;

        },

        //判断是否为空值，如果为空返回true
        isEmpty: function (obj) {
            if (typeof obj == 'undefined' || obj == null || obj == '') {
                return true;
            } else {
                return false;
            }
        },

        //设置批量操作Key
        setBatchOperationKey: function (data, field, action, stn, mid) {

            var ids = micro.getStrSplice(data, field, ',', false),
                Parameter = { "action": action, "stn": stn, "mid": mid, "ids": ids },
                flag = micro.getAjaxData('text', '/App_Handler/CtrlPublicBatchOperationKey.ashx', Parameter, false);

            if (flag.substring(0, 4) === 'True')
                return true;
            else
                return false;

        },

        //设置水印
        setWaterMark: function () {

            if ($('#txtIsWaterMark').length > 0) {

                var waterMarkText = $('#txtWaterMarkText').val(),
                    xSpace = $('#txtXSpace').val(),
                    ySpace = $('#txtYSpace').val(),
                    waterMarkColor = $('#txtWaterMarkColor').val();

                var defaultSettings = {
                    watermark_txt: 'WaterMark',
                    watermark_x: 20,//水印起始位置x轴坐标
                    watermark_y: 20,//水印起始位置Y轴坐标
                    watermark_rows: 10,//水印行数
                    watermark_cols: 10,//水印列数
                    watermark_x_space: 100,//水印x轴间隔
                    watermark_y_space: 100,//水印y轴间隔
                    watermark_color: '#aaa',//水印字体颜色
                    watermark_alpha: 0.4,//水印透明度
                    //watermark_fontsize: '25px',//水印字体大小
                    //watermark_font: '微软雅黑',//水印字体
                    watermark_width: 210,//水印宽度
                    watermark_height: 50,//水印长度
                    watermark_angle: 15//水印倾斜度数
                };

                //获取页面最大宽度
                var page_width = Math.max(document.body.scrollWidth, document.body.clientWidth);
                var cutWidth = page_width * 0.0150;
                page_width = page_width - cutWidth;

                //获取页面最大高度
                var page_height = Math.max(document.body.scrollHeight, document.body.clientHeight) + 450;

                if (waterMarkText != '')
                    defaultSettings.watermark_txt = waterMarkText;

                if (page_width != '' && xSpace != '')
                    defaultSettings.watermark_cols = parseInt(page_width) / parseInt(xSpace);

                if (page_height != '' && ySpace != '')
                    defaultSettings.watermark_rows = parseInt(page_height) / parseInt(ySpace) - 3;

                if (xSpace != '')
                    defaultSettings.watermark_x_space = parseInt(xSpace);

                if (ySpace != '')
                    defaultSettings.watermark_y_space = parseInt(ySpace);

                if (waterMarkColor != '')
                    defaultSettings.watermark_color = waterMarkColor;

                //采用配置项替换默认值，作用类似jquery.extend
                if (arguments.length === 1 && typeof arguments[0] === "object") {
                    var src = arguments[0] || {};
                    for (key in src) {
                        if (src[key] && defaultSettings[key] && src[key] === defaultSettings[key])
                            continue;
                        else if (src[key])
                            defaultSettings[key] = src[key];
                    }
                }


                var oTemp = document.createDocumentFragment();

                //如果将水印列数设置为0，或水印列数设置过大，超过页面最大宽度，则重新计算水印列数和水印x轴间隔
                if (defaultSettings.watermark_cols == 0 || (parseInt(defaultSettings.watermark_x + defaultSettings.watermark_width * defaultSettings.watermark_cols + defaultSettings.watermark_x_space * (defaultSettings.watermark_cols - 1)) > page_width)) {
                    defaultSettings.watermark_cols = parseInt((page_width - defaultSettings.watermark_x + defaultSettings.watermark_x_space) / (defaultSettings.watermark_width + defaultSettings.watermark_x_space));
                    defaultSettings.watermark_x_space = parseInt((page_width - defaultSettings.watermark_x - defaultSettings.watermark_width * defaultSettings.watermark_cols) / (defaultSettings.watermark_cols - 1));
                }
                //如果将水印行数设置为0，或水印行数设置过大，超过页面最大长度，则重新计算水印行数和水印y轴间隔
                if (defaultSettings.watermark_rows == 0 || (parseInt(defaultSettings.watermark_y + defaultSettings.watermark_height * defaultSettings.watermark_rows + defaultSettings.watermark_y_space * (defaultSettings.watermark_rows - 1)) > page_height)) {
                    defaultSettings.watermark_rows = parseInt((defaultSettings.watermark_y_space + page_height - defaultSettings.watermark_y) / (defaultSettings.watermark_height + defaultSettings.watermark_y_space));
                    defaultSettings.watermark_y_space = parseInt(((page_height - defaultSettings.watermark_y) - defaultSettings.watermark_height * defaultSettings.watermark_rows) / (defaultSettings.watermark_rows - 1));
                }
                var x;
                var y;
                for (var i = 0; i < defaultSettings.watermark_rows; i++) {
                    y = defaultSettings.watermark_y + (defaultSettings.watermark_y_space + defaultSettings.watermark_height) * i;
                    for (var j = 0; j < defaultSettings.watermark_cols; j++) {
                        x = defaultSettings.watermark_x + (defaultSettings.watermark_width + defaultSettings.watermark_x_space) * j;

                        var mask_div = document.createElement('div');
                        mask_div.id = 'divWaterMark' + i + j;
                        mask_div.className = 'ws-watermark';
                        mask_div.appendChild(document.createTextNode(defaultSettings.watermark_txt));
                        //设置水印div倾斜显示
                        mask_div.style.webkitTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
                        mask_div.style.MozTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
                        mask_div.style.msTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
                        mask_div.style.OTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
                        mask_div.style.transform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
                        mask_div.style.visibility = "";
                        mask_div.style.position = "absolute";
                        mask_div.style.left = x + 'px';
                        mask_div.style.top = y + 'px';
                        mask_div.style.overflow = "hidden";
                        mask_div.style.zIndex = "9997";
                        mask_div.style.pointerEvents = 'none';//pointer-events:none  让水印不遮挡页面的点击事件
                        //mask_div.style.border="solid #eee 1px";
                        mask_div.style.opacity = defaultSettings.watermark_alpha;
                        //mask_div.style.fontSize = defaultSettings.watermark_fontsize;
                        //mask_div.style.fontFamily = defaultSettings.watermark_font;
                        mask_div.style.color = defaultSettings.watermark_color;
                        mask_div.style.textAlign = "center";
                        mask_div.style.width = defaultSettings.watermark_width + 'px';
                        mask_div.style.height = defaultSettings.watermark_height + 'px';
                        mask_div.style.display = "block";

                        if ($('#divWaterMark' + i + j).length == 0)
                            oTemp.appendChild(mask_div);
                    };
                };
                document.body.appendChild(oTemp);
            }
        }

    };

    // 限制只能输入数字
    $.fn.onlyNum = function () {
        $(this).keyup(function () {
            $(this).val($(this).val().replace(/[^0-9]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9]/g, ''));
        }).css("ime-mode", "disabled");
    };

    // 限制只能输入字母
    $.fn.onlyAlpha = function () {
        $(this).keyup(function () {
            $(this).val($(this).val().replace(/[^a-zA-Z]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^a-zA-Z]/g, ''));
        }).css("ime-mode", "disabled");
    };

    // 限制只能输入数字和字母
    $.fn.onlyNumAlpha = function () {
        $(this).keyup(function () {
            $(this).val($(this).val().replace(/[^0-9a-zA-Z]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9a-zA-Z]/g, ''));
        }).css("ime-mode", "disabled");
    };

    // 限制只能输入数字和小数点
    $.fn.onlyNumPoint = function () {
        $(this).keyup(function () {
            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
        }).css("ime-mode", "disabled");
    };

    // 限制只能输入数字和逗号
    $.fn.onlyNumCom = function () {
        $(this).keyup(function () {
            $(this).val($(this).val().replace(/[^0-9,]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9,]/g, ''));
        }).css("ime-mode", "disabled");
    };

    // 限制只能输入数字和冒号
    $.fn.onlyNumColon = function () {
        $(this).keyup(function () {
            $(this).val($(this).val().replace(/[^0-9:]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9:]/g, ''));
        }).css("ime-mode", "disabled");
    };

    $(function () {
        // 限制使用了onlyNum类样式的控件只能输入数字
        $(".onlyNum").onlyNum();
        //限制使用了onlyAlpha类样式的控件只能输入字母
        $(".onlyAlpha").onlyAlpha();
        // 限制使用了onlyNumAlpha类样式的控件只能输入数字和字母
        $(".onlyNumAlpha").onlyNumAlpha();
        // 限制使用了onlyNumPoint类样式的控件只能输入数字和小数点
        $(".onlyNumPoint").onlyNumPoint();
        // 限制使用了onlyNumCom类样式的控件只能输入数字和逗号
        $(".onlyNumCom").onlyNumCom();
        // 限制使用了onlyNumColon类样式的控件只能输入数字和冒号
        $(".onlyNumColon").onlyNumColon();

        //监听自定义菜单（如信息平台的导航）
        $('.ws-menu li').hover(function () {
            $(this).children("ul").show(300); //mouseover
        }, function () {
            $(this).children("ul").hide(300); //mouseout
        });
    });

    exports('micro', micro);

});
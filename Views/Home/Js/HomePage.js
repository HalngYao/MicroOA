layui.use(['index', 'laydate', 'carousel', 'element', 'layer', 'echarts', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , laydate = layui.laydate
        , carousel = layui.carousel
        , element = layui.element
        , layer = layui.layer
        , echarts = layui.echarts
        , micro = layui.micro
        , device = layui.device();

    var MID = $("#txtMID").val();

    //轮播切换
    $('.layadmin-carousel').each(function () {
        var othis = $(this);
        carousel.render({
            elem: this
            , width: '100%'
            , arrow: 'none'
            , interval: othis.data('interval')
            , autoplay: othis.data('autoplay') === true
            , trigger: (device.ios || device.android) ? 'click' : 'hover'
            , anim: othis.data('anim')
        });
    });

    element.render('progress');


    //自定义函数
    var mGet = {

        //按日周月年申请量统计
        GetApplyToQty: function (queryType, customDate) {
            var url = '/Views/Home/HomePage.ashx',
                parameter = { "action": "View","mid":MID, "datatype":"Qty", "querytype": queryType, "customDate": customDate };

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: url,
                data: parameter,
                cache: false,
                beforeSend: function () {

                },
                success: function (d) {
                    $('#p' + queryType + 'Qty').html(d.currQty);
                    $('#spanDiff' + queryType+'Qty').html(d.diffQty);
                },
                error: function () {
                    
                }
            });
        },

        GetApplyToChart: function (queryType, customDate) {
            var url = '/Views/Home/HomePage.ashx',
                parameter = { "action": "View", "mid": MID, "datatype": "Chart", "querytype": queryType, "customDate": customDate };

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: url,
                data: parameter,
                cache: false,
                beforeSend: function () {

                },
                success: function (d) {

                    var echartsApp = [], options = [
                        //各表单每月申请数量
                        {
                            tooltip: {
                                trigger: 'axis',
                                axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                                    type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
                                }
                            },
                            legend: {
                                //data: ['直接访问', '邮件营销', '联盟广告', '视频广告', '搜索引擎', '百度', '谷歌', '必应', '其他']
                                data: d.lData.split(',')
                            },
                            grid: {
                                left: '3%',
                                right: '4%',
                                bottom: '3%',
                                containLabel: true
                            },
                            xAxis: [
                                {
                                    type: 'category',
                                    //data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
                                    data: d.xData.split(',')
                                }
                            ],
                            yAxis: [
                                {
                                    type: 'value'
                                }
                            ],
                            series: eval(d.series)
                        }

                    ]
                        , elemDataView = $('#LAY-index-dataview1').children('div')
                        , renderDataView = function (index) {
                            echartsApp[index] = echarts.init(elemDataView[index], layui.echartsTheme);
                            echartsApp[index].setOption(options[index]);
                            //window.onresize = echartsApp[index].resize;
                            layui.admin.resize(function () {
                                echartsApp[index].resize();
                            });
                        };


                    //没找到DOM，终止执行
                    if (!elemDataView[0]) return;
                    renderDataView(0);

                    //监听数据概览轮播
                    var carouselIndex = 0;
                    carousel.on('change(LAY-index-dataview)', function (obj) {
                        renderDataView(carouselIndex = obj.index);
                    });

                    //监听侧边伸缩
                    layui.admin.on('side', function () {
                        setTimeout(function () {
                            renderDataView(carouselIndex);
                        }, 300);
                    });


                },
                error: function () {

                }
            });
        },

        //申请量饼图数据
        GetApplyToPieChart: function (queryType, customDate) {
            var url = '/Views/Home/HomePage.ashx',
                parameter = { "action": "View", "mid": MID, "datatype": "PieChart", "querytype": queryType, "customDate": customDate };

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: url,
                data: parameter,
                cache: false,
                beforeSend: function () {

                },
                success: function (d) {

                    var echartsApp = [], options = [
                        //各表单每月申请数量
                        {
                            tooltip: {
                                trigger: 'item',
                                formatter: '{a} <br/>{b}: {c} ({d}%)'
                            },
                            legend: {
                                data: d.lData.split(','),
                                textStyle: {
                                    fontSize: 10
                                },
                            },

                            series: [
                                {
                                    name: '按分类',
                                    type: 'pie',
                                    selectedMode: 'single',
                                    radius: [0, '30%'],
                                    center: ['50%', '60%'],
                                    label: {
                                        position: 'inner',
                                        fontSize: 14,
                                    },
                                    labelLine: {
                                        show: false
                                    },
                                    data: eval(d.sData)
                                },
                                {
                                    name: '按表单',
                                    type: 'pie',
                                    radius: ['45%', '60%'],
                                    center: ['50%', '60%'],
                                    labelLine: {
                                        length: 30,
                                    },
                                    label: {
                                        formatter: '{a|{a}}{abg|}\n{hr|}\n  {b|{b}：}{c}  {per|{d}%}  ',
                                        backgroundColor: '#F6F8FC',
                                        borderColor: '#8C8D8E',
                                        borderWidth: 1,
                                        borderRadius: 4,

                                        rich: {
                                            a: {
                                                color: '#6E7079',
                                                lineHeight: 22,
                                                align: 'center'
                                            },
                                            hr: {
                                                borderColor: '#8C8D8E',
                                                width: '100%',
                                                borderWidth: 1,
                                                height: 0
                                            },
                                            b: {
                                                color: '#4C5058',
                                                fontSize: 14,
                                                fontWeight: 'bold',
                                                lineHeight: 33
                                            },
                                            per: {
                                                color: '#fff',
                                                backgroundColor: '#4C5058',
                                                padding: [3, 4],
                                                borderRadius: 4
                                            }
                                        }
                                    },
                                    data: eval(d.sData2)
                                }
                            ]
                        }

                    ]
                        , elemDataView = $('#LAY-index-2dataview1').children('div')
                        , renderDataView = function (index) {
                            echartsApp[index] = echarts.init(elemDataView[index], layui.echartsTheme);
                            echartsApp[index].setOption(options[index]);
                            //window.onresize = echartsApp[index].resize;
                            layui.admin.resize(function () {
                                echartsApp[index].resize();
                            });
                        };


                    //没找到DOM，终止执行
                    if (!elemDataView[0]) return;
                    renderDataView(0);

                    //监听数据概览轮播
                    var carouselIndex = 0;
                    carousel.on('change(LAY-index-2dataview)', function (obj) {
                        renderDataView(carouselIndex = obj.index);
                    });

                    //监听侧边伸缩
                    layui.admin.on('side', function () {
                        setTimeout(function () {
                            renderDataView(carouselIndex);
                        }, 300);
                    });


                },
                error: function () {
                    console.log("PieChart Error.");
                }
            });
        }, 

        //浏览量
        GetPageView: function (queryType, customDate) {
            var url = '/Views/Home/HomePage.ashx',
                parameter = { "action": "View", "mid": MID, "datatype": "PageView", "querytype": queryType, "customDate": customDate };

            var name = '月';
            if (queryType == 'Day')
                name = '日';
            else if (queryType == 'Week')
                name = '周';
            else if (queryType == 'Year')
                name = '年';

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: url,
                data: parameter,
                cache: false,
                beforeSend: function () {
                    //$('#LAY-index-4dataview1').children(layer.load(2));
                },
                success: function (d) {
                    //layer.close(layer.load(2));

                    var echartsApp = [], options = [
                       
                        {
                            title: {
                                text: name+'流量趋势',
                                x: 'center',
                                textStyle: {
                                    fontSize: 14
                                }
                            },
                            tooltip: {
                                trigger: 'axis'
                            },
                            legend: {
                                data: ['PV', 'UV', 'IP'],
                                x: 'left',
                                //orient: 'vertical'
                            },
                            xAxis: [{
                                type: 'category',
                                boundaryGap: false,
                                //data: ['2021-01-13', '2021-01-12', '2021-01-11', '2021-01-10', '2021-01-09', '2021-01-08', '2021-01-07', '2021-01-06', '2021-01-05', '2021-01-04'] //示例数据格式
                                data: d.xData.split(',')
                            }],
                            yAxis: [{
                                type: 'value'
                            }],
                            series: [
                                {
                                    name: 'PV',
                                    type: 'line',
                                    smooth: true,
                                    itemStyle: { normal: { areaStyle: { type: 'default' } } },
                                    //data: [13000, 222, 333, 444, 555, 666, 3333, 33333, 55555, 66666, 33333, 3333, 6666, 11888, 26666, 38888, 56666, 42222, 39999, 28888, 17777, 9666, 6555, 5555, 3333, 2222, 3111, 6999, 5888, 2777, 1666, 999, 888, 777]  //示例数据格式
                                    data: d.pvData.split(',')
                                },
                                {
                                    name: 'UV',
                                    type: 'line',
                                    smooth: true,
                                    itemStyle: { normal: { areaStyle: { type: 'default' } } },
                                    //data: [12000, 22, 33, 44, 55, 66, 333, 3333, 5555, 12666, 3333, 333, 666, 1188, 2666, 3888, 6666, 4222, 3999, 2888, 1777, 966, 655, 555, 333, 222, 311, 699, 588, 277, 166, 99, 88, 77]  //示例数据格式
                                    data: d.uvData.split(',')
                                }, {
                                    name: 'IP',
                                    type: 'line',
                                    smooth: true,
                                    itemStyle: { normal: { areaStyle: { type: 'default' } } },
                                    //data: [11000, 220, 330, 440, 550, 660, 333, 3333, 5555, 12666, 3333, 333, 666, 1188, 2666, 3888, 6666, 4222, 3999, 2888, 1777, 966, 655, 555, 333, 222, 311, 699, 588, 277, 166, 99, 88, 77]  //示例数据格式
                                    data: d.ipData.split(',')
                                }
                            ]
                        }

                    ]
                        , elemDataView = $('#LAY-index-3dataview1').children('div')
                        , renderDataView = function (index) {
                            echartsApp[index] = echarts.init(elemDataView[index], layui.echartsTheme);
                            echartsApp[index].setOption(options[index]);
                            //window.onresize = echartsApp[index].resize;
                            layui.admin.resize(function () {
                                echartsApp[index].resize();
                            });
                        };


                    //没找到DOM，终止执行
                    if (!elemDataView[0]) return;
                    renderDataView(0);

                    //监听数据概览轮播
                    var carouselIndex = 0;
                    carousel.on('change(LAY-index-3dataview)', function (obj) {
                        renderDataView(carouselIndex = obj.index);
                    });

                    //监听侧边伸缩
                    layui.admin.on('side', function () {
                        setTimeout(function () {
                            renderDataView(carouselIndex);
                        }, 300);
                    });


                },
                error: function () {
                    console.log("PageView Error.");
                }
            });
        },

        //浏览器
        GetBrowser: function (queryType, customDate) {
            var url = '/Views/Home/HomePage.ashx',
                parameter = { "action": "View", "mid": MID, "datatype": "Browser", "querytype": queryType, "customDate": customDate };

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: url,
                data: parameter,
                cache: false,
                beforeSend: function () {

                },
                success: function (d) {

                    var echartsApp = [], options = [

                        {
                            title: {
                                text: '访客浏览器分布',
                                x: 'center',
                                textStyle: {
                                    fontSize: 14
                                }
                            },
                            tooltip: {
                                trigger: 'item',
                                formatter: "{a} <br/>{b} : {c} ({d}%)"
                            },
                            legend: {
                                orient: 'vertical',
                                x: 'left',
                                data: d.xData.split(',')
                            },
                            series: [{
                                name: '访问来源',
                                type: 'pie',
                                radius: '55%',
                                center: ['50%', '60%'],
                                data: eval(d.bData)
                            }]
                        }

                    ]
                        , elemDataView = $('#LAY-index-4dataview1').children('div')
                        , renderDataView = function (index) {
                            echartsApp[index] = echarts.init(elemDataView[index], layui.echartsTheme);
                            echartsApp[index].setOption(options[index]);
                            //window.onresize = echartsApp[index].resize;
                            layui.admin.resize(function () {
                                echartsApp[index].resize();
                            });
                        };


                    //没找到DOM，终止执行
                    if (!elemDataView[0]) return;
                    renderDataView(0);

                    //监听数据概览轮播
                    var carouselIndex = 0;
                    carousel.on('change(LAY-index-4dataview)', function (obj) {
                        renderDataView(carouselIndex = obj.index);
                    });

                    //监听侧边伸缩
                    layui.admin.on('side', function () {
                        setTimeout(function () {
                            renderDataView(carouselIndex);
                        }, 300);
                    });


                },
                error: function () {
                    console.log("GetBrowser Error.");
                }
            });
        },

        //获取系统信息
        GetSysInfo: function () {

            var url = '/Views/Home/HomePage.ashx',
                parameter = { "action": "View", "mid": MID, "datatype": "SysInfo", "querytype": "All", "customDate": "Now" };

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'text',
                url: url,
                data: parameter,
                cache: false,
                success: function (d) {
                    $('#divSysInfo').html(d);
                    element.render('progress');
                },
                error: function () {
                    console.log("GetSysInfo Error.");
                }
            });
        }
    }


    //Day
    laydate.render({
        elem: '#spanDay'
        , done: function (value, date, endDate) {
            mGet.GetApplyToQty('Day', value);
        }
    });

    laydate.render({
        elem: '#aDay'
        , done: function (value, date, endDate) {
            mGet.GetApplyToChart('Day', value);
            mGet.GetApplyToPieChart('Day', value);
        }
    });

    laydate.render({
        elem: '#aDay2'
        , done: function (value, date, endDate) {
            mGet.GetPageView('Day', value);
        }
    });


    //Week
    laydate.render({
        elem: '#spanWeek'
        , done: function (value, date, endDate) {
            mGet.GetApplyToQty('Week', value);
        }
    });

    laydate.render({
        elem: '#aWeek'
        , done: function (value, date, endDate) {
            mGet.GetApplyToChart('Week', value);
            mGet.GetApplyToPieChart('Week', value);
        }
    });

    laydate.render({
        elem: '#aWeek2'
        , done: function (value, date, endDate) {
            mGet.GetPageView('Week', value);
        }
    });


    //Month
    laydate.render({
        elem: '#spanMonth'
        , type: 'month'
        , done: function (value, date, endDate) {
            mGet.GetApplyToQty('Month', value);
        }
    });

    laydate.render({
        elem: '#aMonth'
        , type: 'month'
        , done: function (value, date, endDate) {
            mGet.GetApplyToChart('Month', value);
            mGet.GetApplyToPieChart('Month', value);
        }
    });

    laydate.render({
        elem: '#aMonth2'
        , type: 'month'
        , done: function (value, date, endDate) {
            mGet.GetPageView('Month', value);
        }
    });


    //Year
    laydate.render({
        elem: '#spanYear'
        , type: 'year'
        , done: function (value, date, endDate) {
            mGet.GetApplyToQty('Year', value + '-01-01');
        }
    });

    laydate.render({
        elem: '#aYear'
        , type: 'year'
        , done: function (value, date, endDate) {
            mGet.GetApplyToChart('Year', value + '-01-01');
            mGet.GetApplyToPieChart('Year', value + '-01-01');
        }
    });

    laydate.render({
        elem: '#aYear2'
        , type: 'year'
        , done: function (value, date, endDate) {
            mGet.GetPageView('Year', value + '-01-01');
        }
    });


    mGet.GetApplyToQty('Day');
    mGet.GetApplyToQty('Week');
    mGet.GetApplyToQty('Month');
    mGet.GetApplyToQty('Year');

    mGet.GetApplyToChart('Year');
    mGet.GetApplyToPieChart('Year');
    mGet.GetPageView('Month');
    mGet.GetBrowser('All');


    setInterval(function () {
        mGet.GetSysInfo();
    }, 5000);


});



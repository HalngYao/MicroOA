<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Login.master" AutoEventWireup="true" CodeFile="GetTime.aspx.cs" Inherits="Views_Tools_GetTime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
 
        .ws-time {
            background:#393D49;
            width: 960px;
            height: 120px;
            line-height: 120px;
            font-size: 120px;
            border-radius:80px;
            text-align:center;
            color:#0f0;
            padding-right:60px;
            position:relative;

        }
        
        .ws-time span {
        display: inline-block;
        width:120px;
        text-align:center;
        }

        #divCount span {
        display: block;
        font-size:20px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layadmin-user-login-main">
<%--        <div class="layadmin-user-login-box layadmin-user-login-header">
            <h2>计时器</h2>
            <p>Clock</p>
        </div>--%>
        <div class="layadmin-user-login-box layadmin-user-login-body">

             <div class="layui-form-item" style="left:-280px;">

                <div class="layui-inline ws-time">
                    <span id="H">00</span> :
                    <span id="M">00</span> :
                    <span id="S">00</span> :
                    <span id="MS">000</span>
                </div>
                
            </div>
            <div class="layui-form-item" style="left:60px;">
                <p id="p1"></p>
            </div>

            <div class="layui-form-item">
                <div class="layui-inline" style="width:500px;">
                    <input id="btnStart" type="button" class="layui-btn layui-btn-lg" value="开始 Start" data-type="GetStart" micro-data="Start" style="width:150px;">
                    <input type="button" class="layui-btn layui-btn-lg layui-btn-normal" value="计次 Count" data-type="GetCount" >
                    <input  type="button" class="layui-btn layui-btn-lg layui-btn-danger" value="复位 Reset" data-type="GetReset" >
                    <input type="text" name="txtCount" id="txtCount" placeholder="" class="layui-input layui-hide" value="0">
                </div>
            </div>
            <div class="layui-form-item" style="left:100px;">
                <div id="divCount" style="min-height:400px;"></div>
            </div>

        </div>
    </div>


    <script>

        layui.use(['jquery'], function () {
            var $ = layui.$;

            var hour, minute, second;//时 分 秒
            hour = minute = second = 0;//初始化
            var millisecond = 0;//毫秒
            var int;


            var mGet = {
                //Manager
                GetTimer: function () {
                    millisecond = millisecond + 50;
                    if (millisecond >= 1000) {
                        millisecond = 0;
                        second = second + 1;
                    }
                    if (second >= 60) {
                        second = 0;
                        minute = minute + 1;
                    }

                    if (minute >= 60) {
                        minute = 0;
                        hour = hour + 1;
                    }
                    $('#H').html(mGet.GetFormat(hour));
                    $('#M').html(mGet.GetFormat(minute));
                    $('#S').html(mGet.GetFormat(second));
                    $('#MS').html(mGet.GetFormat2(millisecond));
                },

                GetStart: function () {
                    
                    var d = $(this).attr('micro-data'); 
                    if (d === 'Start') {
                        int = setInterval(mGet.GetTimer, 50);
                        $(this).addClass('layui-btn-warm');
                        $(this).attr('micro-data', 'Pause');
                        $(this).prop('value', '暂停 Pause');
                    }
                    if (d === 'Pause') {
                        $(this).removeClass('layui-btn-warm');
                        $(this).attr('micro-data', 'Start');
                        $(this).prop('value','继续 Continue');
                        mGet.GetPause();
                    }
                },

                GetPause: function () {
                    window.clearInterval(int);
                },

                GetFormat: function (v) {
                    if (v < 10) {
                        v = '0' + v;
                    }
                    return v;
                },

                GetFormat2: function (v) {
                    if (v === 0) {
                        v = '000';
                    }
                    return v;
                },

                GetReset: function () {
                    window.clearInterval(int);
                    millisecond = hour = minute = second = 0;
                    var btnStart = $('#btnStart');
                    $(btnStart).prop('value', '开始 Start');
                    $(btnStart).removeClass('layui-btn-warm');
                    $(btnStart).attr('micro-data', 'Start');

                    $('#H').html('00');
                    $('#M').html('00');
                    $('#S').html('00');
                    $('#MS').html('000');
                    $('#divCount').html('');
                    $('#txtCount').val(0);
                },

                GetCount: function () {
                    var Num = parseInt($('#txtCount').val()) + 1;
                    $('#txtCount').val(Num);

                    $('#divCount').append('<span>' + Num + '次.     ' + mGet.GetFormat(hour) + ' : ' + mGet.GetFormat(minute) + ' : ' + mGet.GetFormat(second) + ' : ' + mGet.GetFormat2(millisecond) + '</span>');
                }


            }


            $('.layui-btn').on('click', function () {
                var type = $(this).data('type');
                mGet[type] ? mGet[type].call(this) : '';
            });

        });

        
        //*********GetDateNow*********
        function getCurDate() {
            var d = new Date();
            var week;
            switch (d.getDay()) {
                case 1: week = "星期一"; break;
                case 2: week = "星期二"; break;
                case 3: week = "星期三"; break;
                case 4: week = "星期四"; break;
                case 5: week = "星期五"; break;
                case 6: week = "星期六"; break;
                default: week = "星期天";
            }

            var years = (d.getYear() < 1900) ? (1900 + d.getYear()) : d.getYear();
            var month = add_zero(d.getMonth() + 1);
            var days = add_zero(d.getDate());
            var hours = add_zero(d.getHours());
            var minutes = add_zero(d.getMinutes());
            var seconds = add_zero(d.getSeconds());
            var ndate ="当前系统时间：" + years + "年" + month + "月" + days + "日 " + hours + ":" + minutes + ":" + seconds + " " + week;
            document.getElementById('p1').innerHTML = ndate;
        }
        function add_zero(temp) {
            if (temp < 10) return "0" + temp;
            else return temp;
        }
        setInterval("getCurDate()", 100);

       
    </script>

</asp:Content>


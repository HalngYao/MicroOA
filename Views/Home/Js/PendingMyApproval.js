layui.use(['index', 'carousel'], function () {
    var $ = layui.$
        , carousel = layui.carousel
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

});



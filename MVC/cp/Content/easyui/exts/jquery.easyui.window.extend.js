/**
 * Created with IntelliJ IDEA.
 * Licensed under the GPL licenses
 * http://www.gnu.org/licenses/gpl.txt
 * @author: ?看?不?字<zjh527@163.com>
 *
 *
 *
 * ?展?明：
 *      1、???展可以在含有iframe的?面中?建跨越iframe定位在最??的window。
 *
 *      2、此?展通???locate?指定窗体依附在哪?dom?象上。
 *          top:                表示定位在最??
 *          document:           ?含有iframe?，窗体定位在iframe的document中
 *          某?dom元素的id:     窗体被定位在指定的dom元素中。
 *
 *          注意: ?content和href中使用相?路??，此??的不同?置?影??面加?，?通??整content和href中的相?路??解?。
 *
 *      3、此方法不接受inline=true的?置。
 *
 *      4、其他????考easyui.window。
 *
 *      5、onLoad方法接收????win、body。
 *          win:    一?Object?象，包含以下??方法:
 *                      getData: ??name。用??取data中?置的?性
 *                      close: ???，???前窗体
 *
 *          body:   一?指向?出窗body的引用，分以下?种：
 *                  a) ?useiframe=false?，是一?指向window.top或window.self的引用。
 *                     要在onLoad中?easyui.window中的?容?行?置?，?使用?似如下形式操作:
 *                          body.$('#username').val('Tom')；
 *
 *                  b) ?useiframe=true?，是一?指向iframe.contentWindow的引用。
 *                     要在onLoad中?easyui.window中的?容?行?置?，操作前先判?body是否存在，如下形式操作:
 *                          if(body) body.doInit();
 *
 *
 *          注意：?useiframe=true?，不同??器?此方法的?行行?不同。
 *
 *
 *      6、toolbar和buttons中定?的每?元素的handler方法都接收一???win。??win?明??5
 *
 *
 *      7、窗体大小，默??算??：父?面大小*0.6 ，如用?指定大小，?不使用默???。
 *
 *      8、方法返回??前窗体的引用。
 *
 *      9、 ?useiframe=true 添加遮罩控制。
 *          9.1 ?性?明：
 *              showMask:   控制是否?示遮罩。其值：true|false
 *              loadMsg:    加?提示信息
 */
(function($){

    function getTop(w, options){
        var _doc;
        try{
            _doc = w['top'].document;
            _doc.getElementsByTagName;
        }catch(e){
            return w;
        }

        if(options.locate=='document' || _doc.getElementsByTagName('frameset').length >0){
            return w;
        }

        if(options.locate=='document.parent' || _doc.getElementsByTagName('frameset').length >0){
            return w.parent;
        }

        return w['top'];
    }

    function setWindowSize(w, options){
        var _top = getTop(w, options);
        var wHeight = $(_top).height(), wWidth = $(_top).width();
        if(!/^#/.test(options.locate)){
            if(options.height == 'auto'){
                options.height = wHeight * 0.6
            }

            if(options.width == 'auto'){
                options.width = wWidth * 0.6
            }
        }else{
            var locate = /^#/.test(options.locate)? options.locate:'#'+options.locate;
            if(options.height == 'auto'){
                options.height = $(locate).height() * 0.6
            }

            if(options.width == 'auto'){
                options.width = $(locate).width() * 0.6
            }
        }
    }

    $.extend({
        /**
         *
         * @param options
         * @return 返回?前窗体的引用
         *
         * 1、新增?性：
         *      useiframe: true|false，指定是否使用iframe加??面。
         *      locate:  top|document|id 默?:top
         *      data:  方法回???
         *
         * 2、增??性：
         *      content: 支持使用前?url指定要加?的?面。
         */
        showWindow: function(options){
            options = options || {};
            var target;
            var winOpts = $.extend({},{
                iconCls:'icon-form',
                useiframe: false,
                locate: 'top',
                data: undefined,
                width: 'auto',
                height: 'auto',
                cache: false,
                minimizable: true,
                maximizable: true,
                collapsible: true,
                resizable: true,
                loadMsg: $.fn.datagrid.defaults.loadMsg,
                showMask: false,
                onClose: function(){target.dialog('destroy');}
            }, options);


            var iframe = null;

            if(/^url:/.test(winOpts.content)){
                var url = winOpts.content.substr(4, winOpts.content.length);
                if(winOpts.useiframe){
                    iframe = $('<iframe>')
                        .attr('height', '100%')
                        .attr('width', '100%')
                        .attr('marginheight', 0)
                        .attr('marginwidth', 0)
                        .attr('frameborder', 0);

                    setTimeout(function(){
                        iframe.attr('src', url);
                    }, 10);

                }else{
                    winOpts.href = url;
                }

                delete winOpts.content;
            }

            var selfRefrence={
                getData: function(name){
                    return winOpts.data ? winOpts.data[name]:null;
                },
                close: function(){
                    target.panel('close');
                }
            };

            var _top = getTop(window, winOpts);
            var warpHandler = function(handler){
                if(typeof handler == 'function'){
                    return function(){
                        handler(selfRefrence);
                    };
                }

                if(typeof handler == 'string' && winOpts.useiframe){
                    return function(){
                        iframe[0].contentWindow[handler](selfRefrence);
                    }
                }

                if(typeof handler == 'string'){
                    return function(){
                        eval(_top[handler])(selfRefrence);
                    }
                }
            }

            setWindowSize(window, winOpts);


            //包?toolbar中各?象的handler
            if(winOpts.toolbar && $.isArray(winOpts.toolbar)){
                $.each(winOpts.toolbar, function(i, button){
                    button.handler = warpHandler(button.handler);
                });
            }

            //包?buttons中各?象的handler
            if(winOpts.buttons && $.isArray(winOpts.buttons)){
                $.each(winOpts.buttons, function(i, button){
                    button.handler = warpHandler(button.handler);
                });
            }


            var onLoadCallback = winOpts.onLoad;
            winOpts.onLoad = function(){
                onLoadCallback && onLoadCallback.call(this, selfRefrence, _top);
            }

            if(!/^#/.test(winOpts.locate)){
                if(winOpts.useiframe && iframe){
                    if(winOpts.showMask){
                        winOpts.onBeforeOpen = function(){
                            var panel = $(this).panel('panel');
                            var header = $(this).panel('header');
                            var body = $(this).panel('body');
                            body.css('position', 'relative');
                            var mask = $("<div class=\"datagrid-mask\" style=\"display:block;\"></div>").appendTo(body);
                            var msg = $("<div class=\"datagrid-mask-msg\" style=\"display:block; left: 50%;\"></div>").html(winOpts.loadMsg).appendTo(body);
                            setTimeout(function(){
                                msg.css("marginLeft", -msg.outerWidth() / 2);
                            }, 5);
                        }
                    }

                    iframe.bind('load', function(){
                        if(iframe[0].contentWindow){
                            onLoadCallback && onLoadCallback.call(this, selfRefrence, iframe[0].contentWindow);
                            target.panel('body').children("div.datagrid-mask-msg").remove();
                            target.panel('body').children("div.datagrid-mask").remove();
                        }
                    });

                    target = _top.$('<div>').css({'overflow':'hidden'}).append(iframe).dialog(winOpts);
                }else{
                    target = _top.$('<div>').dialog(winOpts);
                }
            }else{
                var locate = /^#/.test(winOpts.locate)? winOpts.locate:'#'+winOpts.locate;
                target = $('<div>').appendTo(locate).dialog($.extend({}, winOpts, {inline: true}));
            }

            return target;
        },
        showModalDialog: function(options){
            options = options || {};
            var opts = $.extend({}, options, {
                modal: true,
                minimizable: false,
                maximizable: false,
                resizable: false,
                collapsible: false
            });

            return $.showWindow(opts);
        }
    })
})(jQuery);
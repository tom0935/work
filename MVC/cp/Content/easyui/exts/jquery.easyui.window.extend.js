/**
 * Created with IntelliJ IDEA.
 * Licensed under the GPL licenses
 * http://www.gnu.org/licenses/gpl.txt
 * @author: ?��?��?�r<zjh527@163.com>
 *
 *
 *
 * ?�i?���G
 *      1�B???�i�i�H�b�t��iframe��?����?�ظ�Viframe�w��b��??��window�C
 *
 *      2�B��?�i�q???locate?���w���^�̪��b��?dom?�H�W�C
 *          top:                ��ܩw��b��??
 *          document:           ?�t��iframe?�A���^�w��biframe��document��
 *          �Y?dom������id:     ���^�Q�w��b���w��dom�������C
 *
 *          �`�N: ?content�Mhref���ϥά�?��??�A��??�����P?�m?�v??���[?�A?�q??��content�Mhref������?��??��?�C
 *
 *      3�B����k������inline=true��?�m�C
 *
 *      4�B��L????��easyui.window�C
 *
 *      5�BonLoad��k����????win�Bbody�C
 *          win:    �@?Object?�H�A�]�t�H�U??��k:
 *                      getData: ??name�C��??��data��?�m��?��
 *                      close: ???�A???�e���^
 *
 *          body:   �@?���V?�X��body���ޥΡA���H�U?���G
 *                  a) ?useiframe=false?�A�O�@?���Vwindow.top��window.self���ޥΡC
 *                     �n�bonLoad��?easyui.window����?�e?��?�m?�A?�ϥ�?���p�U�Φ��ާ@:
 *                          body.$('#username').val('Tom')�F
 *
 *                  b) ?useiframe=true?�A�O�@?���Viframe.contentWindow���ޥΡC
 *                     �n�bonLoad��?easyui.window����?�e?��?�m?�A�ާ@�e���P?body�O�_�s�b�A�p�U�Φ��ާ@:
 *                          if(body) body.doInit();
 *
 *
 *          �`�N�G?useiframe=true?�A���P??��?����k��?���?���P�C
 *
 *
 *      6�Btoolbar�Mbuttons���w?���C?������handler��k�������@???win�C??win?��??5
 *
 *
 *      7�B���^�j�p�A�q??��??�G��?���j�p*0.6 �A�p��?���w�j�p�A?���ϥ��q???�C
 *
 *      8�B��k��^??�e���^���ޥΡC
 *
 *      9�B ?useiframe=true �K�[�B�n����C
 *          9.1 ?��?���G
 *              showMask:   ����O�_?�ܾB�n�C��ȡGtrue|false
 *              loadMsg:    �[?���ܫH��
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
         * @return ��^?�e���^���ޥ�
         *
         * 1�B�s�W?�ʡG
         *      useiframe: true|false�A���w�O�_�ϥ�iframe�[??���C
         *      locate:  top|document|id �q?:top
         *      data:  ��k�^???
         *
         * 2�B�W??�ʡG
         *      content: ����ϥΫe?url���w�n�[?��?���C
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


            //�]?toolbar���U?�H��handler
            if(winOpts.toolbar && $.isArray(winOpts.toolbar)){
                $.each(winOpts.toolbar, function(i, button){
                    button.handler = warpHandler(button.handler);
                });
            }

            //�]?buttons���U?�H��handler
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
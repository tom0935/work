	$.extend($.fn.datetimebox.defaults,{
			formatter:function(date){
				var h = date.getHours();
				var M = date.getMinutes();
				var s = date.getSeconds();
				var ampm = h >= 12 ? 'pm' : 'am';
				h = h % 12;
				h = h ? h : 12;
				function formatNumber(value){
					return (value < 10 ? '0' : '') + value;
				}
				var separator = $(this).datetimebox('spinner').timespinner('options').separator;
				var r = $.fn.datebox.defaults.formatter(date) + ' ' + formatNumber(h)+separator+formatNumber(M);
				if ($(this).datetimebox('options').showSeconds){
					r += separator+formatNumber(s);
				}
				r += ' ' + ampm;
				return r;
			},
			parser:function(s){
				if ($.trim(s) == ''){
					return new Date();
				}
				var dt = s.split(' ');
				var d = $.fn.datebox.defaults.parser(dt[0]);
				if (dt.length < 2){
					return d;
				}
				var separator = $(this).datetimebox('spinner').timespinner('options').separator;
				var tt = dt[1].split(separator);
				var hour = parseInt(tt[0], 10) || 0;
				var minute = parseInt(tt[1], 10) || 0;
				var second = parseInt(tt[2], 10) || 0;
				var ampm = dt[2];
				if (ampm == 'pm'){
					hour += 12;
				}
				return new Date(d.getFullYear(), d.getMonth(), d.getDate(), hour, minute, second);
			}
		});
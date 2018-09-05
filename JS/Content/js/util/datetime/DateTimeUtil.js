/**
 * File Name: DateTimeUtil.js Function ID : Function Name: Version : V1.0.0
 * Description : Author : Histories : Version | Modified Date | Modifier |
 * Description
 * 
 */

var DateTimeUtil;

if (!DateTimeUtil) {
	DateTimeUtil = {};
}

(function() {
	if (typeof DateTimeUtil.transferIsoDate != 'function') {
		/**
		 * 將傳入的日期字串(yyyyMM or yyyyMMdd)轉成 ISO 格式日期字串(yyyy-MM or yyyy-MM-dd)
		 * 
		 * @param strDate 要轉換的日期字串, 格式為 yyyyMM or yyyyMMdd
		 * @returns strResult ISO 格式日期字串(yyyy-MM or yyyy-MM-dd), 若傳入無效的參數則回傳原參數值
		 */
		DateTimeUtil.transferIsoDate = function(strDate) {
			var strResult = strDate;

			if (strDate != undefined && strDate != null) {
				if (strDate.length == 6) {
					var strYear = strDate.substr(0, 4);
					var strMonth = strDate.substr(4, 2);

					strResult = strYear + '-' + strMonth;
				} else if (strDate.length == 8) {
					var strYear = strDate.substr(0, 4);
					var strMonth = strDate.substr(4, 2);
					var strDay = strDate.substr(6, 2);

					strResult = strYear + '-' + strMonth + '-' + strDay;
				}
			}

			return strResult;
		};
	}

	if (typeof DateTimeUtil.transferPlainDate != 'function') {
		/**
		 * 將傳入的 ISO 格式日期字串(yyyy-MM or yyyy-MM-dd)轉成日期字串(yyyyMM or yyyyMMdd)
		 * 
		 * @param strDate 要轉換的 ISO 格式日期字串, 格式為 yyyy-MM or yyyy-MM-dd
		 * @returns strResult 日期字串(yyyyMM or yyyyMMdd), 若傳入無效的參數則回傳原參數值
		 */
		DateTimeUtil.transferPlainDate = function(strDate) {
			var strResult = strDate;

			if (strDate != undefined && strDate != null) {
				var aryDate = strDate.split("-");

				if (aryDate.length >= 2) {
					var strYear = aryDate[0];
					var strMonth = aryDate[1];

					strResult = strYear + strMonth;
				} else if (aryDate.length >= 3) {
					var strYear = aryDate[0];
					var strMonth = aryDate[1];
					var strDay = aryDate[2];

					strResult = strYear + strMonth + strDay;
				}

			}

			return strResult;
		};
	}

	if (typeof DateTimeUtil.transferIsoTime != 'function') {
		/**
		 * 將傳入的時間字串(HHmm or HHmmss)轉成 ISO 格式時間字串(HH:mm or HH:mm:ss)
		 * 
		 * @param strTime 要轉換的日期字串, 格式為 HHmm or HHmmss
		 * @returns 時間字串(HH:mm or HH:mm:ss), 若傳入無效的參數則回傳原參數值
		 */
		DateTimeUtil.transferIsoTime = function(strTime) {
			var strResult = strTime;

			if (strTime.length == 4) {
				var strHour = strTime.substr(0, 2);
				var strMinute = strTime.substr(2, 2);

				strResult = strHour + ':' + strMinute;
			} else if (strTime.length == 6) {
				var strHour = strTime.substr(0, 2);
				var strMinute = strTime.substr(2, 2);
				var strSecond = strTime.substr(4, 2);

				strResult = strHour + ':' + strMinute + ':' + strSecond;;
			}

			return strResult;
		};
	}

	if (typeof DateTimeUtil.parseDate != 'function') {
		/**
		 * 將傳入的 ISO 格式日期字串(yyyy-MM-dd)轉成 Date 物件
		 * 
		 * @param strDate 要轉換的 ISO 格式日期字串, 格式為 yyyy-MM-dd
		 * @returns datResult Date 物件, 若傳入無效的參數或轉換失敗則回傳 null
		 */
		DateTimeUtil.parseDate = function(strDate) {
			var datResult = null;

			try {
				//parse a date in yyyy-mm-dd format 		
				var aryParts = strDate.match(/(\d+)/g);

				datResult = new Date(aryParts[0], aryParts[1] - 1, aryParts[2]);
			} catch (e) {
				datResult = null;
			}

			return datResult;
		};
	}

	if (typeof DateTimeUtil.convertDateToISOString != 'function') {
		/**
		 * 根據 Date 型態物件參數回傳 ISO 日期格式字串 (僅回傳日期部份, 不回傳時間)
		 * 
		 * @param date
		 * @returns ISO 日期格式字串
		 */
		DateTimeUtil.convertDateToISOString = function(date) {
			var y = date.getFullYear();
			var m = date.getMonth() + 1;
			var d = date.getDate();

			return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
		};
	}

	if (typeof DateTimeUtil.convertDateToISOStringYYYYMM != 'function') {
	    /**
		 * 根據 Date 型態物件參數回傳 ISO 日期格式字串 (僅回傳日期部份, 不回傳時間)
		 * 
		 * @param date
		 * @returns ISO 日期格式字串
		 */
	    DateTimeUtil.convertDateToISOStringYYYYMM = function (date) {
	        var y = date.getFullYear();
	        var m = date.getMonth() + 1;

	        return y + '-' + (m < 10 ? ('0' + m) : m) ;
	    };
	}
}());

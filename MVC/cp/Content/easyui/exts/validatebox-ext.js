	//yyyy-mm-dd format	
	//var regExpDate = /(\d+)/g;
	var regExpDate = /^(\d{4})-(\d{2})-(\d{2})$/;
	var regExpYear = /^(\d{4})$/;
	
	$.extend($.fn.validatebox.defaults.rules, 
	{   	     
		year:
		{
			validator: function(strYear)
	    	{								
				var objResponse = validatebox_validateYear(strYear);
				
				this.message = objResponse.message;
				
				return objResponse.isPassed;				
	    	},
	    	
	    	message: '資料錯誤'
		},
		
		/**
		 * 驗證輸入的日期字串是否為有效值以及其格式是否為 ISO Date 格式(yyyy-mm-dd).
		 */
		date:
		{
			validator: function(strDate)
			{
				var objResponse = validatebox_validateDate(strDate);
				
				this.message = objResponse.message;
				
				return objResponse.isPassed;
			},
	
			message: '資料錯誤'
		},
		
	     isAfter: 
	     {   	    	 
	         validator: function(strDateTo, objParam)
	         {     
	        	 var bIsPassed = true;
	        	 var strDateFrom = $(objParam[0]).datebox('getValue');
	        	 
	        	 if(strDateTo != '' && strDateFrom != '')
	        	 {
		        	 var aryDatePartTo = strDateTo.match(regExpDate);	        	 
		        	 
		        	 var strDateFrom = $(objParam[0]).datebox('getValue');
		        	 var aryDatePartFrom = strDateFrom.match(regExpDate);
		        	 
		        	 var datFrom = new Date(aryDatePartFrom[1], aryDatePartFrom[2] - 1, aryDatePartFrom[3]);
		        	 var datTo = new Date(aryDatePartTo[1], aryDatePartTo[2] - 1, aryDatePartTo[3]);
		        	 
		        	 bIsPassed = datFrom <= datTo;
	        	 }
	        		 
	        	 return bIsPassed;
	         },   

	         message: '選取日期不可小於起日'  
	     },  

	     isLaterToday:
	     {   
	         validator: function(strDate, objParam)
	         {   	        	 
	        	 straTheDatePart = strDate.match(regExpDate);
	             var datDate = new Date(straTheDatePart[1], straTheDatePart[2] - 1, straTheDatePart[3]);  
	             var datToday = new Date();
	             
	             return datDate < datToday;
	         },   

	         message: '選取日期不可大於今天'  
	     },
	     
	    /**
	     * 驗證輸入 combogrid 的值
	     */ 
		combogrid: 
		{
	        validator: function(value, objParam)
	        {
	            var bIsPassed = false;
	            var objOptions = $(objParam[0]).combogrid('options');
	            var strValue = $(objParam[0]).combogrid('getValue');
	            var objGrid = $(objParam[0]).combogrid('grid');
	            var objRows = objGrid.datagrid('getRows');
	            var strIdField = objOptions.idField;
	            
	            for(var i = 0; i < objRows.length; i++)
	            {
	            	var objRow = objRows[i];
	            	
	            	if(objRow[strIdField] == strValue)
	            	{
	            		bIsPassed = true;
	            		break;	            		
	            	}	            	
	            }
	            
	            return bIsPassed;
	        },
	        
	        message: '請輸入正確值'
	    },	     
	     		
	    /**
	     * 驗證輸入 combobox 的值
	     */
	    combobox:
	    {	    	
	    	validator: function(value, objParam)
	    	{
	    		var bIsPassed = false;
	    		var objOptions = $(objParam[0]).combobox('options');
	    		var strValue = $(objParam[0]).combobox('getValue');
	    		var straValue = $(objParam[0]).combobox('getData');	    		
	    		var strValueField = objOptions.valueField;
	    		
	    		for(var i = 0; i < straValue.length; i++)
	    		{
	    			var objItem = straValue[i];
	    			
	    			var strItemValue = objItem[strValueField];
	    			
	    			if(strValue == strItemValue)
	    			{
	    				bIsPassed = true;
	    				break;
	    			}
	    		}
	    		
	    		return bIsPassed;
	    	},
	    	
	    	message: '請輸入正確值'
	    },
	    
	    /**
	     * 驗證台灣身份證
	     */
	     twId: 
	     {   	    	 
	         validator: function(strId, objParam)
	         {     
	        	    //建立字母分數陣列(A~Z)   
	        	    var city = new Array(   
	        	         1,10,19,28,37,46,55,64,39,73,82, 2,11,   
	        	        20,48,29,38,47,56,65,74,83,21, 3,12,30   
	        	    );
	        	    
	        	    strId = strId.toUpperCase();   
	        	    // 使用「正規表達式」檢驗格式
	        	    
	        	    if (strId.search(/^[A-Z](1|2)\d{8}$/i) == -1) 
	        	    {             
	        	        return false;   
	        	    } 
	        	    else 
	        	    {   
	        	        //將字串分割為陣列(IE必需這麼做才不會出錯)   
	        	        strId = strId.split('');   
	        	        
	        	        //計算總分   
	        	        var total = city[strId[0].charCodeAt(0)-65];
	        	        
	        	        for(var i=1; i<=8; i++)
	        	        {   
	        	            total += eval(strId[i]) * (9 - i);   
	        	        }
	        	        
	        	        //補上檢查碼(最後一碼)   
	        	        total += eval(strId[9]);
	        	        
	        	        //檢查比對碼(餘數應為0);   
	        	        return ((total%10 == 0 ));   
	        	    } 
	         },   
	
	         message: '輸入的身份證號碼錯誤'  
	     }
	});	
	
	function validatebox_validateYear(strYear)
	{
		var bIsPassed = true;	
		var strMessage = '';
		var objResponse = {isPassed: bIsPassed, message: strMessage};
		
		try
		{						
			bIsPassed = regExpYear.test(strYear);
			
			if(bIsPassed == false)
			{
				strMessage = '年格式錯誤,應為 yyyy';
			}
		}
		catch(e)
		{
			bIsPassed = false;
			strMessage = 'validatebox_validateYear(' + strYear + '): ' + e.message;			
		}
		
		objResponse.isPassed = bIsPassed;
		objResponse.message = strMessage;		
		
		return objResponse;
	}
	
	/**
	 * 驗證輸入的日期字串是否為有效值以及其格式是否為 ISO Date 格式(yyyy-mm-dd).
	 * 
	 * @param strDate:
	 * 傳入欲驗證的日期字串.
	 * 
	 * @returns objResponse
	 * 日期字串驗證後的結果. 
	 * 
	 * objResponse.isPassed:
	 * 取得驗證結果的 boolean 值, true 為通過驗證, false 為沒通過驗證.
	 * 
	 * objResponse.message:
	 * 取得驗證結果的訊息, 若 objResponse.isPassed 的值為 true 則回傳空字串; 若 objResponse.isPassed 的值為 false 則回傳驗證失敗的訊息字串.
	 * 
	 */
	function validatebox_validateDate(strDate)
	{
		var bIsPassed = true;	
		var strMessage = '';
		var objResponse = {isPassed: bIsPassed, message: strMessage};
		
		try
		{			
			var bIsDateFormat = regExpDate.test(strDate);
			var aryDate = strDate.split('-');
											
			if(bIsDateFormat == false)
			{
				bIsPassed = false;
				
				if(bIsPassed == false)
				{
					strMessage = '日期格式錯誤,應為 yyyy-mm-dd';
				}
			}
			else
			{					 
				var strYear = aryDate[0];
				var strMonth = aryDate[1];
				var strDay = aryDate[2];
				
				var iYear = 0;
				var iMonth = 0;
				var iDay = 0;
				
				for(var i = 0; i < strYear.length; i++)
				{						
					var strDigit = strYear.substring(i, i + 1);
					var iDigit = parseInt(strDigit);
					
					iYear *= 10;
					iYear += iDigit;
				}
				
				for(var i = 0; i < strMonth.length; i++)
				{						
					var strDigit = strMonth.substring(i, i + 1);
					var iDigit = parseInt(strDigit);
					
					iMonth *= 10;
					iMonth += iDigit;
				}					
				
				for(var i = 0; i < strDay.length; i++)
				{						
					var strDigit = strDay.substring(i, i + 1);
					var iDigit = parseInt(strDigit);
					
					iDay *= 10;
					iDay += iDigit;
				}					

				if(iYear <= 0 || iYear > 9999)
				{
					bIsPassed = false;
				}
				
				if((iMonth <= 0) || (iMonth > 12))
				{
					bIsPassed = false;
				}
				
				if(iDay <= 0 || iDay > 31)
				{
					bIsPassed = false;
				}
				else
				{						
					if(iMonth == 2)
					{
						if(iDay > 29)
						{
							//February can't be greater than 29
							bIsPassed = false;
						}
						else if(iDay == 29)
						{
							//check for leap year if the month and day is February 29
					        var iDiv4 = iYear % 4;
					        var iDiv100 = iYear % 100;
					        var iDiv400 = iYear % 400;
					        // if not divisible by 4, then not a leap year so February 29 is invalid
					        if(iDiv4 != 0) 
					        { 
					        	bIsPassed = false; 
					        }
					        
					        //at this point, year is divisible by 4
					        //So if year is divisible by 100 and not 400, then it's not a leap year so Feb 29 is invalid
					        if((iDiv100 == 0) && (iDiv400 != 0))
					        {
					        	bIsPassed = false; 
					        }							    
						}
					}
					
					//check for months with only 30 days
					if((iMonth == 4) || (iMonth == 6) || (iMonth == 9) || (iMonth == 11))
					{
						if(iDay > 30)
						{
							bIsPassed = false;
						}	
					}
				}
				
				if(bIsPassed == false)
				{
					strMessage = '無效的日期值';
				}
			}
		}
		catch(e)
		{
			bIsPassed = false;
			strMessage = 'validatebox_validateDate(' + strDate + '): ' + e.message;			
		}
		
		objResponse.isPassed = bIsPassed;
		objResponse.message = strMessage;		
		
		return objResponse;	
	}	
	
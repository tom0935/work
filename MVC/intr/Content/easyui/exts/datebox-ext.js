	$.fn.datebox.defaults.formatter = function(date)
	{
		var y = date.getFullYear();
		var m = date.getMonth()+1;
		var d = date.getDate();
		
		if(m < 10)
		{
			m='0'+m;
		}
		
		if(d < 10)
		{
			d='0'+d;
		}
		
		return  y + '-'+ m + '-' + d;
	};	
			
	$.fn.datebox.defaults.parser = function(strDate)
	{		
        if(!strDate)
        {
        	return new Date();
        }
        
        var aryValue = strDate.split('-');  
        var strYear = parseInt(aryValue[0],10);  
        var strMonth = parseInt(aryValue[1],10);  
        var strDay = parseInt(aryValue[2],10);  
        
        if (!isNaN(strYear) && !isNaN(strMonth) && !isNaN(strDay))
        {  
            return new Date(strYear, strMonth - 1, strDay);  
        } 
        else 
        {  
            return new Date();  
        }		
	};
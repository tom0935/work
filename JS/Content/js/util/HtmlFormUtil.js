/**
 * File     Name: HtmlFormUtil.js
 * Function ID  : 
 * Function Name: 
 * Version      : V1.0.0
 * Description  :
 * Author		: 
 * Histories    : 
 * Version  |  Modified Date  |  Modifier  |  Description 
 *   
 */	

var HtmlFormUtil;

if(!HtmlFormUtil)
{
	HtmlFormUtil = {};
}

(function()
{
	if(typeof HtmlFormUtil.clearFieldData != 'function')
	{
		/**
		 * 根據輸入的元件 ID 將其內含的子元件的值全部清空, 清空的項目為包含 password, select-multiple,
		 * select-one, text, textarea, checkbox, radio 等 html 基本元件的值.
		 * 例如一個 html form 或 div 包含 text, checkbox, radio 等元件, 若要清空這些元件的值
		 * 現在不需要再一個元件一個元件的將值清空, 只要透過這方法傳入該 html form 或 div 的 id
		 * 就可以將其所包含的 text, checkbox, radio 等元件的值清空.
		 * 
		 * @param strId 該元件的 ID 
		 */
		HtmlFormUtil.clearFieldData = function(strId)
		{
		    $('#' + strId).find(':input').each(function() 
		    {  
		        switch(this.type)
		        {
		            case 'password':  
		            case 'select-multiple':  
		            case 'select-one':  
		            case 'text':  
		            case 'textarea':
		                $(this).val('');
		                break;  
		            case 'checkbox':
		            case 'radio':
		                this.checked = false;  
		        }  
		    });				
		};
	}
		
	if(typeof HtmlFormUtil.isNoInput != 'function')
	{		
		/**
		 * 根據輸入的元件 ID 確認其內含的子元件是否至少有一個元件有輸入值,
		 * 例如一個 html form 或 div 包含 text, textarea 等元件, 該方法
		 * 用以確認該 form 或 div 其內含的 text 或 textarea 至少有一個元件
		 * 有輸入值, 較常是應用於查詢條件至少要有一個條件中.
		 * 
		 * @param strId 該元件的 ID
		 * @returns objResponse 回傳確認結果物件
		 * 			objResponse.isPassed 取得驗證結果的 boolean 值, true 為表示至少有一個元件有輸入值, 反之則為 false.
		 * 			objResponse.message  取得驗證結果的訊息, 若 objResponse.isPassed 的值為 true 則回傳空字串; 反之則回傳驗證失敗的訊息字串.
		 */
		HtmlFormUtil.isNoInput = function(strId)
		{
			var bIsPassed = true;	
			var strMessage = '';
			var objResponse = {isPassed: bIsPassed, message: strMessage};
												
			var objFields =$('#' + strId).serializeArray();
			
			$.each(objFields, function(i, bojField)
			{
				var strTheValue = $.trim(bojField.value);

				if(strTheValue != '')
				{
					bIsPassed = true;
					
					//To exit the each loop
					return false;
				}			
			}); 

			if(bIsPassed == false)
			{
				strMessage = '請至少輸入一項查詢條件';				
			}
			
			objResponse.isPassed = bIsPassed;
			objResponse.message = strMessage;
			
			return objResponse;
		};
	}
		
}());

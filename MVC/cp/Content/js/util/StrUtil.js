/**
 * File     Name: StrUtil.js
 * Function ID  : 
 * Function Name: 
 * Version      : V1.0.0
 * Description  :
 * Author		: 
 * Histories    : 
 * Version  |  Modified Date  |  Modifier  |  Description 
 *   
 */	

var StrUtil;

if(!StrUtil)
{
	StrUtil = {};
}

(function()
{
	if(typeof StrUtil.bindValueItem != 'function')
	{		
		/**
		 * 組成 value_item 格式的字串
		 * 
		 * @param strValue 字串值
		 * @param strItem 字串值
		 * @returns strResult 回傳 value_item 格式的字串
		 */
		StrUtil.bindValueItem = function(strValue, strItem)
		{	
			var strResult = '';
			
			strValue = $.trim(strValue);
			strItem = $.trim(strItem);						
			
			if(strValue.length == 0 && strItem.length == 0)
			{
				strResult = '';
			}
			else
			{
				if(strValue == '')
				{
					strResult = strItem;
				}
				else if(strItem == '')
				{
					strResult = strValue;
				}
				else
				{
					strResult = strValue + '_' + strItem;
				}						
			}				
			
			return strResult;
		};
		
	}	
	
	if(typeof StrUtil.unbindValueItem != 'function')
	{
		/**
		 * 移除 value_item 格式字串為個別值 value 及 item
		 * 
		 * @param strValueItem 傳入 value_item 格式的字串
		 * @returns objResponse 回傳字串拆解後的值
		 * 			objResponse.value 取得 value_item 格式字串拆解後的 value 值
		 * 			objResponse.item  取得 value_item 格式字串拆解後的 item 值 
		 */
		StrUtil.unbindValueItem = function(strValueItem)
		{
			strValueItem = $.trim(strValueItem);
			
			var objResponse = {};
			var iPosition = strValueItem.indexOf('_');			
			
			if(iPosition != -1)
			{
				var strValue = '';
				var strItem = '';
				
				strValue = strValueItem.substring(0, iPosition);
				strItem = strValueItem.substring(iPosition + 1, strValueItem.length);						
				
				objResponse.value = strValue;
				objResponse.item = strItem;			
			}
			else
			{			
				objResponse.value = strValueItem;
				objResponse.item = strValueItem;						
			}			
			
			return objResponse;
		};
		
	}

	if(typeof StrUtil.trimNull != 'function')
	{
		/**
		 * 去掉字串的前後空白, 若傳入的字串為 null 時則回傳空字串
		 * 
		 * @param strValue 要去除空白的字串
		 * @returns strValue 去除空白後的字串
		 */
		StrUtil.trimNull = function(strValue)
		{
			if(strValue == null)
			{
				strValue = '';
			}
			else
			{
				strValue = $.trim(strValue);
			}
			
			return strValue;			
		};		
	}		
	
	if(typeof StrUtil.replaceEmptyValue != 'function')
	{
		/**
		 * 若傳入的參數 strValue 為空值則以傳入的參數值 strNewValue 替換, 若參數值 strNewValue 未帶入則預設以 -- 字串做為替換字串,
		 * 該方法通常用在 datagrid 的資料值展現上.
		 * 
		 * @param strValue 要處理的字串
		 * @param strNewValue 要替換的字串
		 * @returns strValue 處理完後的字串
		 */
		StrUtil.replaceEmptyValue = function(strValue, strNewValue)
		{						
			if(strValue == 'easyui-footer')
			{
				//若是 easyui-footer 字串資料則不進行替換動作, 用於 easyui datagrid 的 footer 資料展現
				strValue = '';
			}
			else
			{														
				if(strValue == null || strValue == undefined)
				{									
					strValue = $.trim(strValue);
					
					if(strValue == '')
					{
						if(strNewValue != null || strNewValue != undefined)
						{
							strNewValue = $.trim(strNewValue);
							strValue = strNewValue;
						}
						else
						{
							strValue = '--';
						}						
					}										
				}
			}			
			
			return strValue;			
		};
	}
					
}());	

/**
 * 組成 value_item 格式的字串
 * 
 * @deprecated
 * @param strValue 字串值
 * @param strItem 字串值
 * @returns strResult 回傳 value_item 格式的字串
 */
function StrUtil_bindValueItem(strValue, strItem)
{
	return StrUtil.bindValueItem(strValue, strItem);
}

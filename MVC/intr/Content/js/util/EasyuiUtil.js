/**
 * File     Name: EasyuiUtil.js
 * Function ID  : 
 * Function Name: 
 * Version      : V1.0.0
 * Description  :
 * Author		: 
 * Histories    : 
 * Version  |  Modified Date  |  Modifier  |  Description 
 *   
 */	


var EasyuiUtil;

if(!EasyuiUtil)
{
	EasyuiUtil = {};
	
	if(!EasyuiUtil.datagrid)
	{
		EasyuiUtil.datagrid = {};
	}
	
	if(!EasyuiUtil.combogrid)
	{
		EasyuiUtil.combogrid = {};
	}	
	
	if(!EasyuiUtil.datebox)
	{
		EasyuiUtil.datebox = {};
	}		
}	
	
(function()
{
	if(typeof EasyuiUtil.datagrid.refreshHeight != 'function')
	{
		/**
		 * 根據 datagrid 的筆數刷新 datagrid 的高度, 若調整的高度大於 500 則固定高度不動以維持一致的 UI 展現格式
		 * 
		 * @param strId 要調整高度的 datagrid 的 id
		 * @param iRecordNum datagrid 的目前筆數
		 * @param iBasicHeight datagrid 的基本高度, 高度的調整會依 datagrid 的目前筆數及基本高度來計算要調整的高度
		 */
		EasyuiUtil.datagrid.refreshHeight = function(strId, iRecordNum, iBasicHeight)
		{
			var iHeight = 130;
			
			if(iBasicHeight != null && iBasicHeight != undefined)
			{
				iHeight = iBasicHeight;
			}
			
			for(var iIndex = 0; iIndex < iRecordNum; iIndex++)
			{
				iHeight += 30;
			}				
			
			if(iHeight > 500)
			{
				iHeight = 500;
			}		
			
			var objParameter = {height: iHeight};
			
			$('#' + strId).datagrid('resize', objParameter);			
		};
	}
	
	if(typeof EasyuiUtil.datagrid.deleteRows != 'function')
	{		
		/**
		 * 清空 datagrid 中的所有資料列
		 * 
		 * @param strId 要清空資料的 datagrid 的 id
		 */
		EasyuiUtil.datagrid.deleteRows = function(strId)
		{
			var objRows = $('#' + strId).datagrid('getRows');
			
			for(var iIndex = objRows.length - 1; iIndex >= 0; iIndex--)
			{
				$('#' + strId).datagrid('deleteRow', iIndex);
			}			
		};
	}
	
	if(typeof EasyuiUtil.datagrid.deleteRowsByObj != 'function')
	{
		/**
		 * 清空 datagrid 中的所有資料列
		 * 
		 * @param objDataGrid 要清空資料的 datagrid 的實體物件
		 */
		EasyuiUtil.datagrid.deleteRowsByObj = function(objDataGrid)
		{
			var objRows = objDataGrid.datagrid('getRows');		
			
			for(var iIndex = objRows.length - 1; iIndex >= 0; iIndex--)
			{
				objDataGrid.datagrid('deleteRow', iIndex);
			}				
		};
	}
		
	if(typeof EasyuiUtil.combogrid.loadData != 'function')
	{
		/**
		 * 載入 combogrid 的資料
		 * 
		 * @param strId 要載入資料的 combogrid 的 id
		 * @param objData 要載入的資料
		 */
		EasyuiUtil.combogrid.loadData = function(strId, objData)
		{
			$('#' + strId).combogrid('clear');
			var objDataGrid = $('#' + strId).combogrid('grid');
			
			EasyuiUtil.datagrid.deleteRowsByObj(objDataGrid);
			objDataGrid.datagrid('loadData', objData);
		};
	}
	
	if(typeof EasyuiUtil.combogrid.setValue != 'function')
	{
		/**
		 * 設定 combogrid 的資料值,透過這個方法設定值則 combogrid 的 text 值將自動設成 VALUE + 連結分隔符號 + ITEM 格式
		 * 
		 * @param strId 要設值的 combogrid 的 id
		 * @param strValue 要設定的資料值
		 */
		EasyuiUtil.combogrid.setValue = function(strId, strValue)
		{			
			$('#' + strId).combogrid('clear');
			$('#' + strId).combogrid('setValue', strValue);
			
			var objDataGrid = $('#' + strId).combogrid('grid');
			var objSelectedItem = objDataGrid.datagrid('getSelected');					
			console.info(objSelectedItem);
			if(objSelectedItem != null)
			{
				var strText = StrUtil_bindValueItem(objSelectedItem.VALUE, objSelectedItem.ITEM);
				
				$('#' + strId).combogrid('setText', strText);
				
			}									
		};
	}
	
	if(typeof EasyuiUtil.combogrid.getSelectedValueByField != 'function')
	{
		/**
		 * 取得在 combogrid 上選取的資料列的指定欄位資料值
		 * 
		 * @param strId 要取值的 combogrid 的 id
		 * @param strField 指定要取值的欄位
		 */		
		EasyuiUtil.combogrid.getSelectedValueByField = function(strId, strField)
		{
			var strValue = '';
			
			var objDataGrid = $('#' + strId).combogrid('grid');
			var objSelectedItem = objDataGrid.datagrid('getSelected');
			
			if(objSelectedItem != null)
			{
				strValue = objSelectedItem[strField];
			}
			
			return strValue;			
		};
	}
	
	if(typeof EasyuiUtil.combogrid.getSelectedValueByItemField != 'function')
	{
		/**
		 * 取得在 combogrid 上選取的資料列的 Item 欄位資料值
		 * 
		 * @param strId 要取值的 combogrid 的 id
		 */
		EasyuiUtil.combogrid.getSelectedValueByItemField = function(strId)
		{
			var strItem = '';
			
			var objDataGrid = $('#' + strId).combogrid('grid');
			var objSelectedItem = objDataGrid.datagrid('getSelected');		
			
			if(objSelectedItem != null)
			{
				strItem = objSelectedItem.ITEM;
			}
			
			return strItem;			
		};
	}	

	if(typeof EasyuiUtil.datebox.setDefaultValueWhenFocus != 'function')
	{
		/**
		 * 設定 datebox 的預設值,當 batebox 的值為空字串時而使用者點選到該 batebox 則以設定的值取代空字串
		 * 
		 * @param strId 要設定預設值的 datebox 的 id
		 * @param strDate 要設定的預設日期字串,格式為 yyyy-mm-dd 
		 */		
		EasyuiUtil.datebox.setDefaultValueWhenFocus = function(strId, strDate)
		{
			var objDatebox = $('#' + strId).datebox('textbox');
			
			objDatebox.focus
			(
				function()
				{
					if(objDatebox.val() == '')
					{
						$('#' + strId).datebox('setValue', strDate);
					}	
				}
			);
		};
	}

}());


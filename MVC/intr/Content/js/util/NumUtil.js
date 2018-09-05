/**
 * File     Name: NumUtil.js
 * Function ID  : 
 * Function Name: 
 * Version      : V1.0.0
 * Description  :
 * Author		: 
 * Histories    : 
 * Version  |  Modified Date  |  Modifier  |  Description 
 *   
 */	

var NumUtil;

if(!NumUtil)
{
	NumUtil = {};
}

(function()
{
	if(typeof NumUtil.isFloat != 'function')
	{
		/**
		 * 驗證輸入的值是否為浮點數
		 * 
		 * @param value 要驗證的值
		 * @returns bResult 回傳 boolean 值, true 為表示傳入的值為浮點數, 反之則否
		 */
		NumUtil.isFloat = function(value)
		{
			var reg = /^[0-9]*\.[0-9]*$/;
			var bResult = reg.test(value);
			
			return bResult;			
		};
	}
		
}());

/**
 * 驗證輸入的值是否為浮點數
 * 
 * @deprecated
 * @param value 要驗證的值
 * @returns bResult 回傳 boolean 值, true 為表示傳入的值為浮點數, 反之則否
 */
function NumUtil_isFloat(value)
{
	return NumUtil.isFloat(value);
}
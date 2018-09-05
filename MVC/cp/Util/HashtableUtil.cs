
using System.Collections;
using System;
public class HashtableUtil{

    public static String getValue(Hashtable ht, String keyStr)
    {
        String value = "";
        foreach (DictionaryEntry de in ht)
        {
            if (de.Key.Equals(keyStr))
            {
                if (de.Value != null)
                {
                    value = de.Value.ToString();
                }
                else
                {
                    value = "";
                }
                break;
            }
        }
        return value;
    }
}
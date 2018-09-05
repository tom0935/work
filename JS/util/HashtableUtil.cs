
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
                value = de.Value.ToString();
                break;
            }
        }
        return value;
    }
}
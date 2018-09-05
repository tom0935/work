using System.Data.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
public class EntityHelper
{
    /// <summary>
    /// Entities the property names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<string> EntityPropertyNames<T>()
    {
        string entityName = typeof(T).Name;
 
        var members = new Dictionary<string, IEnumerable<string>>();
 
        var mw = new MetadataWorkspace(
            new[] { "res://*/" },
            new[] { Assembly.GetExecutingAssembly() });
 
        var tables = mw.GetItems(DataSpace.CSpace);
 
        foreach (var e in tables.OfType<EntityType>())
        {
            members.Add
                (
                    e.Name,
                    e.Members
                        .Where(m => m.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
                        .Select(m => m.Name)
                );
        }
 
        return members.Where(x => x.Key == entityName).Select(x => x.Value).FirstOrDefault();
    }
 
}
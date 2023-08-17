using System.Data;

namespace ComicApi.Model.Repositories;

public static class DbExtensions
{
    public static T GetValue<T>(this DataRow row, string column)
    {
        var value = row[column];
        if (value is DBNull) return default(T);
        return (T)value;
    }
}
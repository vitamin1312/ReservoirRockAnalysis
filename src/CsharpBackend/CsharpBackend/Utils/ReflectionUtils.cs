namespace CsharpBackend.Utils
{
    public class ReflectionUtils
    {
        public static IEnumerable<string> GetPublicProperties<T>()
        {
            return typeof(T)
                .GetProperties()
                .Where(x => x.GetMethod?.IsPublic == true)
                .Select(x => x.Name);
        }

        public static object? GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            var propInfo = obj.GetType().GetProperty(propertyName);

            return propInfo?.GetValue(obj);
        }
    }
}

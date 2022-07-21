using SimpleCut.Common.Consts;

namespace SimpleCut.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static string DateTimeToString(this DateTime target)
        {
            return target.ToString(DateTimeSettings.DateTimeFormat); 
        }
    }
}

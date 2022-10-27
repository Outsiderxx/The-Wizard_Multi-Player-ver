using System;

/// <summary>
/// 時間單位轉換器
/// </summary>
public static class DateTimeConverter
{
    /// <summary>
    /// Unix timestamp 時間基準點
    /// </summary>
    private static readonly DateTime epochTime = new DateTime(1970, 1, 1);

    /// <summary>
    /// 將DateTime物件轉換為自JS可直接使用的毫秒時間值
    /// </summary>
    /// <param name="dateTime">要被轉換的DateTime物件</param>
    /// <returns>1970/1/1到被轉換DateTime物件所經過的時間(in milliseconds)</returns>
    /// <example>JS: Date(returnValue)</example>
    public static double ToTimestamp(DateTime dateTime)
    {
        return dateTime.Subtract(epochTime).TotalMilliseconds;
    }

    /// <summary>
    /// 將UNIX timestamp轉換為DateTime物件
    /// </summary>
    /// <param name="milliseconds">UNIX timestamp</param>
    /// <returns>DateTime物件</returns>
    public static DateTime ToDateTime(long timestamp)
    {
        return epochTime.AddMilliseconds(timestamp);
    }

    /// <summary>
    /// 將ISO time format表示的時間轉換為DateTime物件
    /// </summary>
    /// <param name="ISOTime">ISO time format表示的時間</param>
    /// <returns>DateTime物件</returns>
    public static DateTime ToDateTime(string ISOTime)
    {
        return DateTime.Parse(ISOTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
}

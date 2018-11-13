using System;
using System.Linq;

/// <summary>
/// 時間系の拡張クラス.
/// </summary>
public static class UnixAndDateTimeExtention
{
	/// <summary>UNIX Time</summary>
    public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

	/// <summary>
	/// Uint型からDateTime型へ変換する.
	/// </summary>
	/// <param name="value">Uint型の値</param>
	/// <returns>DateTime型の値</returns>
	public static DateTime UnixToDateTime(this uint value)
	{
        return UNIX_EPOCH.AddSeconds(value).ToLocalTime();
	}

	/// <summary>
	/// サーバーとのやり取りで使われるyyyyMMdd形式の文字列からDateTime型へ変換する.
	/// DateIDに日付が無かった場合,強制的に1日として返す.
	/// </summary>
	public static DateTime StrIdToDateTime(this string value)
    {
        var strDate = value.Substring(0, 4)+"/";    // 年
        strDate += value.Substring(4, 2)+"/";       // 月
		strDate += value.Length > 6 ? value.Substring(6, 2) : "01";
        return DateTime.Parse(strDate);
    }

    /// <summary>
    /// 引数のDateTimeと引き算を行い差分の日数を求める.
    /// </summary>
    public static int GetDays(this DateTime value, DateTime prev)
    {
        // 端末で日付不正している可能性あり.
        if(prev > value){
            return 0;
        }

        var day = value.Day;
        var prevDay = prev.Day;

        // 年月に差分があるならその日数も考慮する.
        var nowMonth = value.Month+((value.Year - prev.Year)*12);
        var prevMonth = prev.Month;
        var year = value.Year;
        var month = value.Month;
        for(var i = 0; i < nowMonth-prevMonth; ++i){
            month--;
            if(month <= 0) {
                month = 12;
                year--;
            }
            var days = DateTime.DaysInMonth(year,month);
            day += days;
        }
        return day-prevDay;
    }

    /// <summary>
    /// 日本語表記で文字列化する.
    /// </summary>
    public static string ToJapanString(this DateTime value, string format)
    {
        var culture = System.Globalization.CultureInfo.CreateSpecificCulture("ja-JP");
        return value.ToString(format, culture);
    }

    /// <summary>
    /// DateTime から Unix 時間 [s] を返す.
    /// </summary>
    /// <param name="value">DateTime型の値</param>
    /// <returns>UnixTime.</returns>
    public static uint ToUnixTime(this DateTime value)
    {
        return (uint)(value.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
    }

    /// <summary>
    /// DateTime から yyyyMMdd形式の文字列(サーバーで使用するdate_id) を返す.
    /// </summary>
    /// <param name="value">DateTime型の値</param>
    /// /// <param name="bWithoutDay">日付抜かしで作成したい場合はtrue.</param>
    /// <param name="value">yyyyMMdd形式の文字列(サーバーで使用するdate_id).</param>
    public static string ToStrId(this DateTime value, bool bWithoutDay = false)
    {
        var strYear = value.Year.ToString("D4");
        var strMonth = value.Month.ToString("D2");
        var strDay = bWithoutDay ? "": value.Day.ToString("D2");
        return strYear + strMonth + strDay;
    }
}

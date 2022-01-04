using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MecWise.Blazor.Common
{

    public static class StringExtensions {
        public static T ToEnum<T>(this string value) {
            return (T)Enum.Parse(typeof(T), value, true);
        }


        public static Uri Append(this Uri uri, params string[] paths) {
            string tempUri = uri.ToString();
            foreach (var path in paths) {
                tempUri = string.Format("{0}/{1}", tempUri.TrimEnd('/'), path.TrimStart('/'));
            }
            return new Uri(tempUri);
        }

        public static bool In(this string item, params string[] items) {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }

        public static string SplitAndGetItem(this string item, char separator, int index) {
            if (string.IsNullOrEmpty(item)) {
                return "";
            }

            string[] items = item.Split(separator);
            if (index <= items.Length - 1) {
                return items[index];
            }
            else {
                return "";
            }
        }

        public static string SplitAndGetItem(this string item, int index) {
            if (string.IsNullOrEmpty(item)) {
                return "";
            }

            char[] items = item.ToArray();
            if (index <= items.Length - 1) {
                return items[index].ToString();
            }
            else {
                return "";
            }
        }
    }

    public static class JTokenExtensions
    {
        public static bool IsEqual(this JToken JTObj, object obj) {
            try
            {
                if (JTObj.ToString() == obj.ToString())
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime ToLocTime(this DateTime datetime)
        {
            try
            {
                return datetime.AddHours(8);
            }
            catch
            {
                return datetime;
            }
        }
    }

    public static class ObjectExtensions
    {

        public static bool IsDate(this object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime result = DateTime.Now;
                return DateTime.TryParse(strDate,out result);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDate(this object obj, string format) {
            string strDate = obj.ToString();
            try {
                DateTime.ParseExact(strDate, format, CultureInfo.InvariantCulture);
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool IsNumeric(this object obj)
        {
            string strDate = obj.ToString();
            try
            {
                Double result;
                return Double.TryParse(strDate, out result);
            }
            catch
            {
                return false;
            }
        }

        public static decimal ToDec(this object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static double ToDbl(this object obj)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }


        public static int ToInt(this object obj)
        {
            try
            {
                if (obj == null) {
                    return 0; 
                }
                if (string.IsNullOrEmpty(obj.ToStr())) {
                    return 0;
                }
                if (obj.ToString().ToUpper().In("FALSE"))
                {
                    return 0;
                }
                if (obj.ToString().ToUpper().In("TRUE"))
                {
                    return 1;
                }
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static string ToStr(this object obj)
        {
            try
            {
                if (obj == null){
                    return "";
                }
                else{
                    return obj.ToString();
                }
                
            }
            catch
            {
                return "";
            }
        }

        public static bool ToBool(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    if (obj.ToString().ToUpper().In("0","FALSE","N")) {
                        return false;
                    }

                    if (obj.ToString().ToUpper().In("1", "TRUE", "Y"))
                    {
                        return true;
                    }

                    return Convert.ToBoolean(obj.ToString());
                }

            }
            catch
            {
                return false;
            }
        }

        public static DateTime ToDate(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return DateTime.MinValue;
                }
                else
                {

                    // Some IOS version use "AM/PM" as "A.M/P.M"
                    string tempDateStr = obj.ToStr().ToUpper();
                    tempDateStr = tempDateStr.Replace("A.M", "AM");
                    tempDateStr = tempDateStr.Replace("P.M", "PM");

                    string[] formats = {"yyyyMMdd", "yyyyMMdd HH:mm:ss", "yyyy-MM-dd",
                        "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:ss",
                        "yyyy-MM-dd HH:mm:ss tt"
                    };

                    foreach (string format in formats) {
                        if (tempDateStr.IsDate(format)) {
                            return DateTime.ParseExact(tempDateStr.ToStr(), format, CultureInfo.InvariantCulture);
                        }
                    }

                    return DateTime.Parse(tempDateStr.ToString(), System.Globalization.CultureInfo.CurrentCulture);
                }

            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}

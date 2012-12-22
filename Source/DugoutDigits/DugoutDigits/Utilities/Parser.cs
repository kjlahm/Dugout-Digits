using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Utilities {
    public class Parser {
        public static DateTime ParseDateAndTime(string date, string time, int year) {
            string[] dateSplit = date.Split('/');
            string[] timeSplit = time.Split(new char[] { ':', ' ' });

            int month = Convert.ToInt32(dateSplit[0]);
            int day = Convert.ToInt32(dateSplit[1]);
            int hour = Convert.ToInt32(timeSplit[0]);
            int minute = Convert.ToInt32(timeSplit[1]);
            if (timeSplit[2].Equals("PM")) hour += 12;

            return new DateTime(year, month, day, hour, minute, 0);
        }
    }
}
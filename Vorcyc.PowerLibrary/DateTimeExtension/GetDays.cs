using System;
using System.Collections.Generic;
using System.Text;

namespace Vorcyc.PowerLibrary.DateTimeExtension
{
    public static class GetDays
    {

        public static int GetDaysOfYear(int year)
        {
            int sum = 0;
            for (int i = 1; i < 13; i++) {
                DateTime date = new DateTime(year, i, 1);
                //下个月一号减上个月1号的得到的日子数
                int daysOfMonth = (date.AddMonths(1) - date).Days;
                sum += daysOfMonth;
            }
            return sum;
        }

        public static int GetDaysOfMonth(int year, int month)
        {
            DateTime date = new DateTime(year, month, 1);
            return (date.AddMonths(1) - date).Days;
        }

    }
}

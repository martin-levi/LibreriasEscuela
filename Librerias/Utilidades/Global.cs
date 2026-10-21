using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Librerias.Utilidades {
    public class Global {

        public static IEnumerable<Tuple< int, int>> MonthsBetween(
           DateTime startDate,
           DateTime endDate) {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate) {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            } else {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }
             
            while (iterator <= limit) {
                yield return Tuple.Create(
                    iterator.Month,
                    iterator.Year);
                iterator = iterator.AddMonths(1);
            }
        }

    }
}

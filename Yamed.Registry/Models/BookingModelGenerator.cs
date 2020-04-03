using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yamed.Core;


namespace Yamed.Registry.Models
{
    public static class BookingModelGenerator
    {
        public static IEnumerable<BookingModel> GetBookingModels(IEnumerable<dynamic> dataList)
        {
            return (from object data in dataList
                    let fam = (string) ObjHelper.GetAnonymousValue(data, "FAM")
                let im = (string) ObjHelper.GetAnonymousValue(data, "IM")
                let ot = (string) ObjHelper.GetAnonymousValue(data, "OT")
                select new BookingModel
                {
                    StartTime = (DateTime) ObjHelper.GetAnonymousValue(data, "BeginTime"), Name = $"{fam} {im} {ot}"
                }).ToList();
        }
    }
}

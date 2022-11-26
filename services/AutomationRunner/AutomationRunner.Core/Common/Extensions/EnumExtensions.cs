using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutomationRunner.Core.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEntityId(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo.Length <= 0) 
                return en.ToString();
            
            var attrs = memInfo[0].GetCustomAttributes(typeof(EntityIdAttribute), false);

            return attrs.Length > 0 ? ((EntityIdAttribute)attrs[0]).Text : en.ToString();
        }
    }
}

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

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(EntityIdAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((EntityIdAttribute)attrs[0]).Text;
            }

            return en.ToString();
        }
    }
}

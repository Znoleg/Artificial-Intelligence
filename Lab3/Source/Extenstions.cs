using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameKnowledgeModel
{
    public static class Extenstions
    {
        public static T RequestEnumValue<T>(string printMessage, string warningMessage, 
            IEnumerable<T> toExclude = null) where T : Enum
        {
            Type enumType = typeof(T);
            T[] enumFitsValues = (T[])Enum.GetValues(typeof(T));
            
            if (toExclude != null && toExclude.Any())
            {
                enumFitsValues = enumFitsValues.Where(type => !toExclude.Contains(type)).ToArray();
            }

            IEnumerable<string> enumStrings = enumFitsValues.Select(enumValue => enumValue.ToString());

            string enumTypesStr = $"[{string.Join(", ", enumStrings)}]";

            Console.WriteLine($"{printMessage} {enumTypesStr}: ");
            string enumTypeReply = Console.ReadLine();
            while (!enumStrings.Contains(enumTypeReply))
            {
                Console.WriteLine(warningMessage);
                enumTypeReply = Console.ReadLine();
            }

            return (T)Enum.Parse(enumType, enumTypeReply);
        }

        public static string ToSingleStr(this IEnumerable<string> stringCollection, string separator = ", ") =>
            string.Join(separator, stringCollection);
    }
}

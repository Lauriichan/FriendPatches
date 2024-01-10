using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendPatches.Tools
{
    static internal class DebugHelper
    {

        public static string ToDebugString(this Exception exp)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(exp.GetType().Name + ": " + exp.Message);
            builder.Append(exp.StackTrace);
            if (exp.InnerException != null)
            {
                AppendCause(builder, exp.InnerException);
            }
            return builder.ToString();
        }
        
        private static void AppendCause(StringBuilder builder, Exception exp)
        {
            builder.Append("Caused by " + exp.GetType().Name + ": " + exp.Message);
            builder.Append(exp.StackTrace);
            if (exp.InnerException != null)
            {
                AppendCause(builder, exp.InnerException);
            }
        }

    }
}

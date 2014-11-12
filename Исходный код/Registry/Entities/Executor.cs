using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Executor
    {
        public int? IdExecutor { get; set; }
        public string ExecutorName { get; set; }
        public string ExecutorLogin { get; set; }
        public string Phone { get; set; }
        public bool? IsInactive { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Executor));
        }

        public bool Equals(Executor other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Executor first, Executor second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdExecutor == second.IdExecutor &&
                first.ExecutorName == second.ExecutorName &&
                first.ExecutorLogin == second.ExecutorLogin &&
                first.Phone == second.Phone &&
                first.IsInactive == second.IsInactive;
        }

        public static bool operator !=(Executor first, Executor second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

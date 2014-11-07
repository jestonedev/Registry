using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Executor
    {
        public int? id_executor { get; set; }
        public string executor_name { get; set; }
        public string executor_login { get; set; }
        public string phone { get; set; }
        public bool? is_inactive { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Executor))
                return false;
            Executor obj_executor = (Executor)obj;
            if (this == obj_executor)
                return true;
            else
                return false;
        }

        public bool Equals(Executor other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Executor first, Executor second)
        {
            return first.id_executor == second.id_executor &&
                first.executor_name == second.executor_name &&
                first.executor_login == second.executor_login &&
                first.phone == second.phone &&
                first.is_inactive == second.is_inactive;
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

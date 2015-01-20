﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ResettleObject
    {
        public int? IdAssoc { get; set; }
        public int? IdObject { get; set; }
        public int? IdProcess { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ResettleObject));
        }

        public bool Equals(ResettleObject other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ResettleObject first, ResettleObject second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdObject == second.IdObject &&
                first.IdProcess == second.IdProcess;
        }

        public static bool operator !=(ResettleObject first, ResettleObject second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

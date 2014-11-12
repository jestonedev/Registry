﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Warrant
    {
        public int? IdWarrant { get; set; }
        public int? IdWarrantDocType { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string OnBehalfOf { get; set; }
        public string Notary { get; set; }
        public string NotaryDistrict { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Warrant));
        }

        public bool Equals(Warrant other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Warrant first, Warrant second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdWarrant == second.IdWarrant &&
                first.IdWarrantDocType == second.IdWarrantDocType &&
                first.Notary == second.Notary &&
                first.NotaryDistrict == second.NotaryDistrict &&
                first.OnBehalfOf == second.OnBehalfOf &&
                first.RegistrationDate == second.RegistrationDate &&
                first.RegistrationNum == second.RegistrationNum &&
                first.Description == second.Description;
        }

        public static bool operator !=(Warrant first, Warrant second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

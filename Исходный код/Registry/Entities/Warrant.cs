using System;

namespace Registry.Entities
{
    public sealed class Warrant : Entity
    {
        public int? IdWarrant { get; set; }
        public int? IdWarrantDocType { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string OnBehalfOf { get; set; }
        public string Notary { get; set; }
        public string NotaryDistrict { get; set; }
        public string Description { get; set; }
    }
}

using System;

namespace Registry.Entities
{
    public sealed class ClaimState : Entity
    {
        public int? IdState { get; set; }
        public int? IdClaim { get; set; }
        public int? IdStateType { get; set; }
        public DateTime? DateStartState { get; set; }
        public DateTime? DateEndState { get; set; }
        public string DocumentNum { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ClaimState));
        }

        public bool Equals(ClaimState other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ClaimState first, ClaimState second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdState == second.IdState &&
                   first.IdClaim == second.IdClaim &&
                   first.IdStateType == second.IdStateType &&
                   first.DateStartState == second.DateStartState &&
                   first.DateEndState == second.DateEndState &&
                   first.DocumentNum == second.DocumentNum &&
                   first.DocumentDate == second.DocumentDate &&
                   first.Description == second.Description;
        }

        public static bool operator !=(ClaimState first, ClaimState second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

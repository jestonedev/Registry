using System;

namespace Registry.Entities
{
    public class OwnershipRight : Entity
    {
        public int? IdOwnershipRight { get; set; }
        public int? IdOwnershipRightType { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as OwnershipRight));
        }

        public bool Equals(OwnershipRight other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(OwnershipRight first, OwnershipRight second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdOwnershipRight == second.IdOwnershipRight &&
                   first.IdOwnershipRightType == second.IdOwnershipRightType &&
                   first.Number == second.Number &&
                   first.Date == second.Date &&
                   first.Description == second.Description;
        }

        public static bool operator !=(OwnershipRight first, OwnershipRight second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

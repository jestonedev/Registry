namespace Registry.Entities
{
    public class Entity
    {
        public static bool operator ==(Entity first, Entity second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            foreach (var propertyInfo in first.GetType().GetProperties())
            {
                var firstValue = propertyInfo.GetValue(first, null);
                var secondValue = propertyInfo.GetValue(second, null);
                if (firstValue == null && secondValue == null)
                    continue;
                if (firstValue == null || secondValue == null)
                    return false;
                if (!firstValue.Equals(secondValue))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Entity first, Entity second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Entity;
        }

        public bool Equals(Entity other)
        {
            return Equals((object)other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

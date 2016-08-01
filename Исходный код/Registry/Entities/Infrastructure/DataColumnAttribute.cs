using System;

namespace Registry.Entities.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public object DefaultValue { get; set; }

        public bool IncludeIntoUpdate
        {
            get
            {
                if (_includeIntoUpdateSetted)
                {
                    return _includeIntoUpdate;
                }
                return !IsPrimaryKey;
            }
            set
            {
                _includeIntoUpdateSetted = true;
                _includeIntoUpdate = value;
            }
        }

        public bool IncludeIntoInsert
        {
            get
            {
                if (_includeIntoInsertSetted)
                {
                    return _includeIntoInsert;
                }
                return !IsPrimaryKey;
            }
            set
            {
                _includeIntoInsertSetted = true;
                _includeIntoInsert = value;
            }
        }

        private bool _includeIntoUpdate;
        private bool _includeIntoUpdateSetted;
        private bool _includeIntoInsert;
        private bool _includeIntoInsertSetted;
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Entities.Infrastructure;

namespace Registry.DataModels
{
    public static class DataModelHelper
    {
        public static IEnumerable<int> Intersect(IEnumerable<int> list1, IEnumerable<int> list2)
        {
            if (list1 != null && list2 != null)
                return list1.Intersect(list2).ToList();
            return list1 ?? list2;
        }

        public enum ConditionType { BuildingCondition, PremisesCondition };

        public static IEnumerable<int> MunicipalObjectStates()
        {
            return new List<int> { 4, 5, 9, 11, 12 };
        }

        public static IEnumerable<int> NonMunicipalObjectStates()
        {
            return new List<int> { 3, 6, 7, 8, 10 };
        }

        public static IEnumerable<int> MunicipalAndUnknownObjectStates()
        {
            return new List<int> { 1 }.Concat(MunicipalObjectStates());
        }

        public static IEnumerable<int> NonMunicipalAndUnknownObjectStates()
        {
            return new List<int> { 1 }.Concat(NonMunicipalObjectStates());
        }
    }
}

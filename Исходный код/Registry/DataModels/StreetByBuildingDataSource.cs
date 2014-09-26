using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace Registry.DataModels
{
    //Связующий источник данных
    public class StreetByBuildingDataSource : IList
    {
        private BuildingsDataModel buildings = null;
        private KladrDataModel kladr = null;

        public StreetByBuildingDataSource(BuildingsDataModel buildings, KladrDataModel kladr)
        {
            this.buildings = buildings;
            this.kladr = kladr;
        }

        public int IndexOf(object item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object item)
        {
            throw new DataModelException("Коллекция только для чтения");
        }

        public void RemoveAt(int index)
        {
            throw new DataModelException("Коллекция только для чтения");
        }

        public object this[int index]
        {
            get
            {
                DataRow building_row = buildings.Select().Rows[index];
                DataRow kladr_row = kladr.Select().Rows.Find(building_row["id_street"]);
                string street_name = null;
                if (kladr_row != null)
                    street_name = kladr_row["street_name"].ToString();
                return new BuildingKladrAssoc(Convert.ToInt32(building_row["id_building"]), street_name);
            }
            set
            {
                throw new DataModelException("Коллекция только для чтения");
            }
        }

        public int Add(object value)
        {
            throw new DataModelException("Коллекция только для чтения");
        }

        public void Clear()
        {
            throw new DataModelException("Коллекция только для чтения");
        }

        public bool Contains(object item)
        {
            DataRow building_row = buildings.Select().Rows.Find(((BuildingKladrAssoc)item).id_building);
            if (building_row == null)
                return false;
            DataRow kladr_row = kladr.Select().Rows.Find(building_row["id_street"]);
            if (kladr_row == null)
                return false;
            if (kladr_row["steet_name"].ToString() == ((BuildingKladrAssoc)item).street_name)
                return true;
            else
                return false;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return buildings.Select().Rows.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new DataModelException("Коллекция только для чтения");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new IdStreetByBuildingDataSourceEnumerator(buildings, kladr);
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return null; }
        }
    }

    public class IdStreetByBuildingDataSourceEnumerator : IEnumerator
    {
        private int position = -1;
        private BindingSource v_building = new BindingSource();
        private BindingSource v_kladr = new BindingSource();

        public IdStreetByBuildingDataSourceEnumerator(BuildingsDataModel buildings, KladrDataModel kladr)
        {
            this.v_building.DataSource = buildings.Select();
            this.v_kladr.DataSource = kladr.Select();
        }

        public object Current
        {
            get
            {
                try
                {
                    DataRowView building_row = (DataRowView)v_building[position];
                    int kladr_index = v_kladr.Find("id_street", building_row["id_street"]);
                    return new BuildingKladrAssoc(Convert.ToInt32(building_row["id_building"]),
                        ((DataRowView)v_kladr[kladr_index])["street_name"].ToString());
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < v_building.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }

    public class BuildingKladrAssoc
    {
        public int id_building { get; set; }
        public string street_name { get; set; }

        public BuildingKladrAssoc(int id_building, string street_name)
        {
            this.id_building = id_building;
            this.street_name = street_name;
        }
    }
}

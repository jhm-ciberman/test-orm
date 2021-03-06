using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    class DB
    {
        class Table
        {
            public bool isAutoIncrement = true;
            private int maxId = 0;
            public string PrimaryKeyName { get; private set; }
            private List<IDictionary<string, object>> data = new List<IDictionary<string, object>>();

            public Table(string primaryKeyName)
            {
                this.PrimaryKeyName = primaryKeyName;
            }

            public int Insert(IDictionary<string, object> row)
            {
                var data = new Dictionary<string, object>(row);
                if (this.isAutoIncrement)
                    data[this.PrimaryKeyName] = ++this.maxId;

                this.data.Add(data);
                return this.maxId;
            }

            public void Update(object primaryKey, IDictionary<string, object> row)
            {
                var data = this.Find(primaryKey);
                foreach (var col in row)
                    data[col.Key] = col.Value;
            }

            public IDictionary<string, object> Find(object primaryKey)
            {
                return this.data.FirstOrDefault(row => object.Equals(row[this.PrimaryKeyName], primaryKey));
            }

            public IDictionary<string, object> Find(string columnName, object value)
            {
                return this.data.FirstOrDefault(row => object.Equals(row[columnName], value));
            }
        }

        private static Dictionary<string, Table> tables = new Dictionary<string, Table>();

        public static void CreateTable(string tableName, string primaryKeyName)
        {
            tables.Add(tableName, new Table(primaryKeyName));
        }

        public static void Insert(string tableName, IDictionary<string, object> row)
        {
            InsertGetId(tableName, row);
        }

        public static int InsertGetId(string tableName, IDictionary<string, object> row)
        {
            int pk = tables[tableName].Insert(row);
            Console.WriteLine($"Inserted:  ({tables[tableName].PrimaryKeyName}={pk})");
            foreach (var attr in row)
                Console.WriteLine($"    {attr.Key} = {attr.Value}");
            return pk;
        }

        public static void Update(string tableName, object primaryKey, IDictionary<string, object> row)
        {
            Console.WriteLine($"Updated: ({tables[tableName].PrimaryKeyName}={primaryKey})");
            foreach (var attr in row)
                Console.WriteLine($"    {attr.Key} = {attr.Value}");
            tables[tableName].Update(primaryKey, row);
        }

        public static IDictionary<string, object> Select(string tableName, object id)
        {
            Console.WriteLine($"select * from {tableName} where {tables[tableName].PrimaryKeyName} = {Convert.ToInt32(id)}");
            return tables[tableName].Find(id);
        }

        public static IDictionary<string, object> Select(string tableName, string columnName, object value)
        {
            Console.WriteLine($"select * from {tableName} where {columnName} = {Convert.ToInt32(value)}");
            return tables[tableName].Find(columnName, value);
        }
    }
}
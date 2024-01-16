using com.Instant.Mishor.Program.Enums;

using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using System.Data;

namespace com.Instant.Mishor.Program
{
    internal class Importer
    {
        private const int BatchSize = 100000;
        private const string Schema = "Mishor";
        private const string FileExtension = "csv";

        private readonly string[] _tables = new string[4] { "Hotels", "Destinations", "Countries", "HotelDestinations" };

        private readonly string? _connectionString = @"Data Source=.\SQLEXPRESS;Database=Medici;User Id=sa;Password=12345;TrustServerCertificate=true;";
        private readonly SqlConnection _connection;

        private ObjectType _objectType;

        private string? _tableName;
        private string? _filename;
        private string? _filePath;

        private readonly SqlBulkCopy? _bulkCopy;

        private DataTable? _dataTable;

        private readonly Dictionary<string, Type> _fields = new();
        private readonly Dictionary<string, string> _dictionary = new();
        private readonly Dictionary<string, object> _data = new();

        private string? _line;
        private string[]? _values;
        private string[]? _columns;

        private StreamReader? _streamReader;

        private int _index = 0;

        public Importer()
        {
            _connection = new SqlConnection(_connectionString);

            _bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.TableLock)
            {
                BatchSize = BatchSize
            };
        }

        private void Validate()
        {
            if (!File.Exists(_filePath)) throw new InvalidOperationException($"File not found: {_filePath}");
        }

        public void Run(ObjectType objectType)
        {
            Console.WriteLine($"Initializing: {objectType}");

            _objectType = objectType;

            InitializeVariables();

            Validate();

            InitializeFields();
            InitializeDataTable();

            _index = 0;

            _streamReader = new StreamReader(_filePath!);

            while ((_line = _streamReader.ReadLine()) != null)
            {
                //Console.WriteLine(_line);

                try
                {
                    InitializeValues();

                    if (_index == 0)
                    {
                        InitializeColumns();
                    }
                    else
                    {
                        InitializeDictionary();
                        InitializeData();

                        _dataTable?.Rows.Add(_data.Values.ToArray());

                        if ((_index % 100000 == 0) || (_streamReader.EndOfStream))
                        {
                            Save();

                            _dataTable?.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _index++;
                }
            }

            _streamReader.Close();

            Console.WriteLine($"Completed: {objectType}");
        }

        public void Clear()
        {
            _connection.Open();

            foreach (var table in _tables)
            {
                using var command = new SqlCommand($"DELETE FROM [{Schema}].[{table}]", _connection)
                {
                    CommandType = CommandType.Text
                };

                command.ExecuteNonQuery();
            }

            _connection.Close();
        }

        private void InitializeVariables()
        {
            _tableName = _objectType.ToString();

            InitializeFilename();

            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", $"{_filename}.{FileExtension}");

            _bulkCopy!.DestinationTableName = $"[{Schema}].[{_tableName}]";
        }

        private void InitializeFilename()
        {
            switch (_objectType)
            {
                case ObjectType.Countries:
                    _filename = "countries";
                    break;
                case ObjectType.Destinations:
                    _filename = "destinations";
                    break;
                case ObjectType.Hotels:
                    _filename = "hotels";
                    break;
                case ObjectType.HotelDestinations:
                    _filename = "hotel_destinations";
                    break;
                default:
                    _filename = "countries";
                    break;
            }
        }

        private void InitializeFields()
        {
            _fields.Clear();


            switch (_objectType)
            {
                case ObjectType.Countries:
                    _fields["Id"] = typeof(string);
                    _fields["Name"] = typeof(string);
                    _fields["Continent"] = typeof(string);
                    _fields["Region"] = typeof(string);
                    _fields["Currency"] = typeof(string);
                    break;
                case ObjectType.Destinations:
                    _fields["Id"] = typeof(string);
                    _fields["Name"] = typeof(string);
                    _fields["Type"] = typeof(string);
                    _fields["Latitude"] = typeof(decimal);
                    _fields["Longitude"] = typeof(decimal);
                    _fields["CountryId"] = typeof(string);
                    _fields["Searchable"] = typeof(bool);
                    _fields["SeoName"] = typeof(string);
                    _fields["State"] = typeof(string);
                    _fields["Contains"] = typeof(string);
                    break;
                case ObjectType.Hotels:
                    _fields["Id"] = typeof(int);
                    _fields["Name"] = typeof(string);
                    _fields["Address"] = typeof(string);
                    _fields["Status"] = typeof(int);
                    _fields["ZipCode"] = typeof(string);
                    _fields["Phone"] = typeof(string);
                    _fields["Fax"] = typeof(string);
                    _fields["Latitude"] = typeof(decimal);
                    _fields["Longitude"] = typeof(decimal);
                    _fields["Stars"] = typeof(int);
                    _fields["SeoName"] = typeof(string);
                    break;
                case ObjectType.HotelDestinations:
                    _fields["Id"] = typeof(int);
                    _fields["HotelId"] = typeof(int);
                    _fields["DestinationId"] = typeof(int);
                    _fields["Surroundings"] = typeof(int);
                    break;
            }
        }

        private void InitializeDataTable()
        {
            _dataTable = new DataTable(_objectType.ToString());

            _dataTable.Columns.AddRange(_fields.Select(x => new DataColumn()
            {
                ColumnName = x.Key,
                DataType = x.Value
            }).ToArray());
        }

        private void InitializeValues()
        {
            _values = null;

            if (_line == null) return;

            using var parser = new TextFieldParser(new StringReader(_line))
            {
                HasFieldsEnclosedInQuotes = true
            };

            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                _values = parser.ReadFields();
            }

            parser.Close();
        }

        private void InitializeColumns()
        {
            _columns = _values;
        }

        private void InitializeDictionary()
        {
            _dictionary.Clear();

            if (_columns == null || _values == null) return;

            for (var i = 0; i < _columns.Length; i++)
            {
                _dictionary.Add(_columns[i], _values[i]);
            }
        }

        private void InitializeData()
        {
            _data.Clear();

            switch (_objectType)
            {
                case ObjectType.Countries:
                    _data["Id"] = _dictionary["id"];
                    _data["Name"] = _dictionary["name"];
                    _data["Continent"] = _dictionary["continent"];
                    _data["Region"] = _dictionary["region"];
                    _data["Currency"] = _dictionary["currency"];
                    break;
                case ObjectType.Destinations:
                    _data["Id"] = _dictionary["id"];
                    _data["Name"] = _dictionary["name"];
                    _data["Type"] = _dictionary["type"];
                    _data["Latitude"] = GetDecimal(_dictionary["lat"]);
                    _data["Longitude"] = GetDecimal(_dictionary["lon"]);
                    _data["CountryId"] = _dictionary["countryid"];
                    _data["Searchable"] = GetBoolean(_dictionary["searchable"]);
                    _data["SeoName"] = _dictionary["seoname"];
                    _data["State"] = _dictionary["state"];
                    _data["Contains"] = _dictionary["contains"];
                    break;
                case ObjectType.Hotels:
                    _data["Id"] = GetInteger(_dictionary["id"]);
                    _data["Name"] = _dictionary["name"];
                    _data["Address"] = _dictionary["address"];
                    _data["Status"] = GetInteger(_dictionary["status"]);
                    _data["ZipCode"] = _dictionary["zip"];
                    _data["Phone"] = _dictionary["phone"];
                    _data["Fax"] = _dictionary["fax"];
                    _data["Latitude"] = GetDecimal(_dictionary["lat"]);
                    _data["Longitude"] = GetDecimal(_dictionary["lon"]);
                    _data["Stars"] = GetInteger(_dictionary["stars"]);
                    _data["SeoName"] = _dictionary["seoname"];
                    break;
                case ObjectType.HotelDestinations:
                    _data["Id"] = _index;
                    _data["HotelId"] = GetInteger(_dictionary["hotel_id"]);
                    _data["DestinationId"] = GetInteger(_dictionary["destination_id"]);
                    _data["Surroundings"] = GetInteger(_dictionary["surroundings"]);
                    break;
            }
        }

        private static int GetInteger(string value)
        {
            return int.TryParse(value, out int result) ? result : 0;
        }

        private static decimal GetDecimal(string value)
        {
            return decimal.TryParse(value, out decimal result) ? result : 0;
        }

        private static bool GetBoolean(string value)
        {
            return bool.TryParse(value, out bool result) ? result : false;
        }

        private void Save()
        {
            Console.WriteLine("Saving Started");

            _bulkCopy?.WriteToServer(_dataTable);

            Console.WriteLine("Saving Complete");

        }
    }
}

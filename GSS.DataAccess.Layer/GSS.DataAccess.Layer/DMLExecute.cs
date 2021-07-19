using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GSS.DataAccess.Layer
{
    public sealed class DMLExecute
    {
        public static string con = System.Configuration.ConfigurationManager.ConnectionStrings["StoreSoftConnection"].ToString();
        //"Data Source=.;Initial Catalog=StoreSoftware;User ID=sa;Password=jathin2008";

        string _sql;
        List<MapFileds> _mapFields = new List<MapFileds>();

        public void AddFields(string _fieldName, string _fieldValue)
        {
            MapFileds _mapField = new MapFileds();
            _mapField.FieldName = _fieldName;
            _mapField.FieldValue = _fieldValue;
            _mapFields.Add(_mapField);
        }

        public int ExecuteInsert(string _TableName, SqlConnection _conn, SqlTransaction _conTran = null)
        {
            try
            {
                _sql = @"INSERT INTO " + _TableName + "(";
                foreach (MapFileds map in _mapFields)
                {
                    _sql = _sql + map.FieldName + ",";
                }
                _sql = _sql.Substring(0, _sql.Length - 1);

                _sql = _sql + ") VALUES (";

                foreach (MapFileds map in _mapFields)
                {
                    _sql = _sql + "@" + map.FieldName + ",";
                }
                _sql = _sql.Substring(0, _sql.Length - 1);
                _sql = _sql + ")" ;

                SqlCommand _sqlcmd = new SqlCommand(_sql, _conn, _conTran);
                //Adding values

                foreach (MapFileds map in _mapFields)
                {
                    if (map.FieldValue == null)
                        map.FieldValue = "";

                    _sqlcmd.Parameters.AddWithValue("@" + map.FieldName, map.FieldValue);
                }

                _mapFields.Clear();
                return _sqlcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteUpdate(string _TableName, SqlConnection _conn, string[] _keyFieldNames, SqlTransaction _conTran = null)
        {
            string _whereClause = "";
            try
            {
                _sql = @"UPDATE " + _TableName + " SET ";
                foreach (MapFileds map in _mapFields)
                {
                    if (!_keyFieldNames.Contains(map.FieldName))
                    {
                        // will not update created by and created timestamp for update transaction
                        if ((map.FieldName != "CREATED_BY") && (map.FieldName != "CREATED_TIMESTAMP"))
                            _sql = _sql + map.FieldName + " = @" + map.FieldName + ",";
                    }
                    else
                        _whereClause = _whereClause + map.FieldName + " = @" + map.FieldName + " AND "; 
                }

                _sql = _sql.Substring(0, _sql.Length - 1);

                if (_whereClause.Length > 0)
                    _whereClause = " WHERE " + _whereClause.Substring(0, _whereClause.Length - 4);

                _sql += _whereClause;

                SqlCommand _sqlcmd = new SqlCommand(_sql, _conn, _conTran);
                foreach (MapFileds map in _mapFields)
                {
                    if ((map.FieldName != "CREATED_BY") &&  (map.FieldName != "CREATED_TIMESTAMP"))
                        _sqlcmd.Parameters.AddWithValue("@" + map.FieldName, map.FieldValue);
                }

                _mapFields.Clear();
                return _sqlcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteDMLSQL(string _Sql, SqlConnection _conn, SqlTransaction _conTran = null)
        {
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_Sql, _conn, _conTran);
                _sqlcmd.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public sealed class MapFileds
    {
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}

using Dapper;
using Npgsql;
using Serilog;
using System;

namespace DataImporter
{
    class DBUtilities
    {
        string _connstring;
        ILogger _logger;

        public DBUtilities(string connstring, ILogger logger)
        {
            _connstring = connstring;
            _logger = logger;

        }

        public int GetRecordCount(string table_name)
        {
            int res = 0;
            string sql_string = @"select count(*) from sd." + table_name;
            using (var conn = new NpgsqlConnection(_connstring))
            {
                res = conn.ExecuteScalar<int>(sql_string);
            }
            return res;
        }

        public int ExecuteSQL(string sql_string)
        {
            using (var conn = new NpgsqlConnection(_connstring))
            {
                return conn.Execute(sql_string);
            }
        }


        public void ExecuteDandI(string sql_string1, string sql_string2, string table_name)
        {
            using (var conn = new NpgsqlConnection(_connstring))
            {
                conn.Execute(sql_string1);
                ExecuteTransferSQL(sql_string2, table_name, "Editing");
            }
        }

        // The T String identifies study / hash type combinations where ANY
        // type of change has occured. This allows all attributes of that 
        // type to be updated - i.e. deleted and then added back in revised form.
        // The status of the att (composite hash) record does not affect the process
        // if atts of a certain type are deleted (status = 4) they are not added back,
        // if atts are new (status = 1) they are simply added. More often, with  change
        // of some form in an att type, (status = 2) a new set of atts replaces the old.

        public string GetStudyTString(int type_id)
        {
            return @"with t as (
               SELECT sd_sid from 
               sd.to_ad_study_atts 
               WHERE hash_type_id = " + type_id.ToString() + ") ";
        }


        public string GetStudyDeleteString(string table_name)
        {
            return @" DELETE FROM ad." + table_name + @" a
            USING t
            WHERE a.sd_sid = t.sd_sid; ";
        }


        public string GetObjectTString(int type_id)
        {
            return @"with t as (
               SELECT sd_oid from 
               sd.to_ad_object_atts 
               WHERE hash_type_id = " + type_id.ToString() + ") ";
        }


        public string GetObjectDeleteString(string table_name)
        {
            return @" DELETE from ad." + table_name + @" a
            USING t
            WHERE a.sd_oid = t.sd_oid; ";
        }


        public void ExecuteTransferSQL(string sql_string, string table_name, string context)
        {
            try
            {
                int rec_count = GetRecordCount(table_name);
                int rec_batch = 500000;
                // int rec_batch = 10000;  // for testing 
                if (rec_count > rec_batch)
                {
                    for (int r = 1; r <= rec_count; r += rec_batch)
                    {
                        string batch_sql_string = sql_string + " and s.id >= " + r.ToString() + " and s.id < " + (r + rec_batch).ToString();
                        ExecuteSQL(batch_sql_string);

                        string feedback = context + " " + table_name + " data, " + r.ToString() + " to ";
                        feedback += (r + rec_batch < rec_count) ? (r + rec_batch - 1).ToString() : rec_count.ToString();
                        _logger.Information(feedback);
                    }
                }
                else
                {
                    ExecuteSQL(sql_string);
                    _logger.Information(context + " " + table_name + " data, as a single batch");
                }
            }
            catch (Exception e)
            {
                string res = e.Message;
                _logger.Error("In data transfer (" + table_name + ") to ad table: " + res);
            }
        }


        public void UpdateLastImportedDate(string table_name, string top_sql, string base_sql)
        {
            try
            {
                string sql_string = top_sql;
                string feedbackA = "Updating last imported dates and import ids, (" + table_name + "), ";
                int rec_count = GetRecordCount(table_name);
                int rec_batch = 100000;
                if (rec_count > rec_batch)
                {
                    for (int r = 1; r <= rec_count; r += rec_batch)
                    {
                        sql_string += " and so.id >= " + r.ToString() + " and so.id < " + (r + rec_batch).ToString();
                        ExecuteSQL(sql_string + base_sql);
                        
                        string feedback = feedbackA + r.ToString() + " to ";
                        feedback += (r + rec_batch < rec_count) ? (r + rec_batch - 1).ToString() : rec_count.ToString();
                        _logger.Information(feedback);
                    }
                }
                else
                { 
                    ExecuteSQL(sql_string + base_sql);
                    _logger.Information(feedbackA + " as a single batch");
                }
            }
            catch (Exception e)
            {
                string res = e.Message;
                _logger.Error("In update last imported date (" + table_name + "): " + res);
            }
        }


        public void UpdateDateOfData(string table_name, string top_sql, string base_sql)
        {
            try
            {
                int rec_count = GetRecordCount(table_name);
                int rec_batch = 100000;

                if (rec_count > rec_batch)
                {
                    for (int r = 1; r <= rec_count; r += rec_batch)
                    {
                        string batch_sql_string = top_sql + " and so.id >= " + r.ToString() + " and so.id < " + 
                                                  (r + rec_batch).ToString() + base_sql;
                        ExecuteSQL(batch_sql_string);

                        string feedback = "Updating date of data for " + table_name + ", " + r.ToString() + " to ";
                        feedback += (r + rec_batch < rec_count) ? (r + rec_batch - 1).ToString() : rec_count.ToString();
                        _logger.Information(feedback);
                    }
                }
                else
                {
                    ExecuteSQL(top_sql + base_sql);
                    _logger.Information("Updating date of data for " + table_name + ", as a single batch");
                }
            }
            catch (Exception e)
            {
                string res = e.Message;
                _logger.Error("In " + table_name + " date of data update: " + res);
            }
        }


        public void EditEntityRecords(string topstring, string basestring, string table_name)
        {
            try
            {
                int rec_count = GetRecordCount(table_name);
                int rec_batch = 100000;

                if (rec_count > rec_batch)
                {
                    for (int r = 1; r <= rec_count; r += rec_batch)
                    {
                        string batch_sql_string = topstring + " and so.id >= " + r.ToString() + " and so.id < " 
                                                + (r + rec_batch).ToString() + basestring;
                        ExecuteSQL(batch_sql_string);

                        string feedback = "Updating entity records for " + table_name + ", " + r.ToString() + " to ";
                        feedback += (r + rec_batch < rec_count) ? (r + rec_batch - 1).ToString() : rec_count.ToString();
                        _logger.Information(feedback);
                    }
                }
                else
                {
                    ExecuteSQL(topstring + basestring);
                    _logger.Information("Updating entity records for " + table_name + ", as a single batch");
                }
            }
            catch (Exception e)
            {
                string res = e.Message;
                _logger.Error("In updating entity records for " + table_name + ", sd to ad table: " + res);
            }
        }


        public void EditStudyHashes(string table_name, string top_sql)
        {
            // Need to ensure that the hashes themselves are all up to date (for the next comparison)
            // Change the ones that have been changed in sd
            // if a very large data objects (and therefore hash) table may need to chunk using a link to the 
            // sd.studies table....

            try
            {
                int rec_count = GetRecordCount(table_name);
                int rec_batch = 100000;

                if (rec_count > rec_batch)
                {
                    for (int r = 1; r <= rec_count; r += rec_batch)
                    {
                        string batch_sql_string = top_sql + " and so.id >= " + r.ToString() + " and so.id < " + (r + rec_batch).ToString();
                        ExecuteSQL(batch_sql_string);

                        string feedback = "Updating changed composite hashes for " + table_name + ", " + r.ToString() + " to ";
                        feedback += (r + rec_batch < rec_count) ? (r + rec_batch - 1).ToString() : rec_count.ToString();
                        _logger.Information(feedback);
                    }
                }
                else
                {
                    ExecuteSQL(top_sql);
                    _logger.Information("Updating changed composite hashes for " + table_name + ", as a single batch");
                }
            }
            catch (Exception e)
            {
                string res = e.Message;
                _logger.Error("In " + table_name + " data edits (composite hashes), sd to ad table: " + res);
            }
        }


        public void UpdateFullHashes(string table_name, string top_sql)
        {
            // Ensure object_full_hash is updated to reflect new value
            // The object record itself may not have changed, so the object
            // record update above cannot be used to make the edit 
            try
            {
                string sql_string = top_sql;
                int rec_count = GetRecordCount(table_name);
                int rec_batch = 100000;

                if (rec_count > rec_batch)
                {
                    for (int r = 1; r <= rec_count; r += rec_batch)
                    {
                        string batch_sql_string = sql_string + " and so.id >= " + r.ToString() + " and so.id < " + (r + rec_batch).ToString();
                        ExecuteSQL(batch_sql_string);

                        string feedback = "Updating full hashes for " + table_name + ", " + r.ToString() + " to ";
                        feedback += (r + rec_batch < rec_count) ? (r + rec_batch - 1).ToString() : rec_count.ToString();
                        _logger.Information(feedback);
                    }
                }
                else
                {
                    ExecuteSQL(sql_string);
                    _logger.Information("Updating full hashes for " + table_name + ", as a single batch");
                }
            }
            catch (Exception e)
            {
                string res = e.Message;
                _logger.Error("In  " + table_name + " full hash updates: " + res);
            }
        }
    }
}

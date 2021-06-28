﻿namespace DataImporter
{
    class ADBuilder
    {
        private string _db_conn;
        private Source _source;
        StudyTableBuilders _study_builder;
        ObjectTableBuilders _object_builder;
        LoggingDataLayer _logging_repo;

        public ADBuilder(string db_conn, Source source, LoggingDataLayer logging_repo)
        {
            _db_conn = db_conn;
            _source = source;
            _study_builder = new StudyTableBuilders(_db_conn);
            _object_builder = new ObjectTableBuilders(_db_conn);
            _logging_repo = logging_repo;
        }


        public void BuildNewADTables()
        {
            if (_source.has_study_tables)
            {
                // these common to all databases

                _study_builder.create_ad_schema();
                _study_builder.create_table_studies();
                _study_builder.create_table_study_identifiers();
                _study_builder.create_table_study_titles();
                _study_builder.create_table_study_hashes();

                // these are database dependent
                if (_source.has_study_topics) _study_builder.create_table_study_topics();
                if (_source.has_study_features) _study_builder.create_table_study_features();
                if (_source.has_study_contributors) _study_builder.create_table_study_contributors();
                if (_source.has_study_references) _study_builder.create_table_study_references();
                if (_source.has_study_relationships) _study_builder.create_table_study_relationships();
                if (_source.has_study_links) _study_builder.create_table_study_links();
                if (_source.has_study_ipd_available) _study_builder.create_table_ipd_available();

                _logging_repo.LogLine("Rebuilt AD study tables");
            }

            // object tables - these common to all databases

            _object_builder.create_table_data_objects();
            _object_builder.create_table_object_instances();
            _object_builder.create_table_object_titles();
            _object_builder.create_table_object_hashes();

            // these are database dependent		

            if (_source.has_object_datasets) _object_builder.create_table_object_datasets();
            if (_source.has_object_dates) _object_builder.create_table_object_dates();
            if (_source.has_object_relationships) _object_builder.create_table_object_relationships();
            if (_source.has_object_rights) _object_builder.create_table_object_rights();
            if (_source.has_object_pubmed_set)
            {
                _object_builder.create_table_object_contributors();
                _object_builder.create_table_object_topics();
                _object_builder.create_table_object_comments();
                _object_builder.create_table_object_descriptions();
                _object_builder.create_table_object_identifiers();
                _object_builder.create_table_object_db_links();
                _object_builder.create_table_object_publication_types();
            }

            _logging_repo.LogLine("Rebuilt AD Object tables");
        }
    }
}

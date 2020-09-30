﻿using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataImporter
{
	class DataObjectDataAdder
	{
		string connstring;
		DBUtilities dbu;

		public DataObjectDataAdder(string _connstring)
		{
			connstring = _connstring;
			dbu = new DBUtilities(connstring);
		}


		#region Table data transfer

		public void TransferDataObjects()
		{
			string sql_string = @"INSERT INTO ad.data_objects(sd_oid, sd_sid, 
            display_title, version, doi, doi_status_id, publication_year,
            object_class_id, object_type_id, managing_org_id, managing_org, lang_code, access_type_id,
            access_details, access_details_url, url_last_checked, eosc_category, add_study_contribs,
            add_study_topics, datetime_of_data_fetch, record_hash, object_full_hash)
            SELECT d.sd_oid, d.sd_sid, 
            display_title, version, doi, doi_status_id, publication_year,
            object_class_id, object_type_id, managing_org_id, managing_org, lang_code, access_type_id,
            access_details, access_details_url, url_last_checked, eosc_category, add_study_contribs,
            add_study_topics, datetime_of_data_fetch, record_hash, object_full_hash
            FROM sd.data_objects d
            INNER JOIN ad.import_object_recs nd
            ON d.sd_oid = nd.sd_oid
            WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "data_objects", "adding");
		}


		public void TransferDataSetProperties()
		{
			string sql_string = @"INSERT INTO ad.dataset_properties(sd_oid, 
            record_keys_type_id, record_keys_details, 
            deident_type_id, deident_direct, deident_hipaa,
            deident_dates, deident_nonarr, deident_kanon, deident_details,
			consent_type_id, consent_noncommercial, consent_geog_restrict,
			consent_research_type, consent_genetic_only, consent_no_methods, consent_details,
			record_hash)
            SELECT d.sd_oid, 
            record_keys_type_id, record_keys_details, 
            deident_type_id, deident_direct, deident_hipaa,
            deident_dates, deident_nonarr, deident_kanon, deident_details,
			consent_type_id, consent_noncommercial, consent_geog_restrict,
			consent_research_type, consent_genetic_only, consent_no_methods, consent_details,
			record_hash
            FROM sd.dataset_properties d
            INNER JOIN ad.import_object_recs nd
            ON d.sd_oid = nd.sd_oid
            WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "dataset_properties", "adding");
		}


		public void TransferObjectInstances()
		{
			string sql_string = @"INSERT INTO ad.object_instances(sd_oid, 
            instance_type_id, repository_org_id, repository_org,
            url, url_accessible, url_last_checked, resource_type_id,
            resource_size, resource_size_units, resource_comments, record_hash)
            SELECT d.sd_oid, 
            instance_type_id, repository_org_id, repository_org,
            url, url_accessible, url_last_checked, resource_type_id,
            resource_size, resource_size_units, resource_comments, record_hash
            FROM sd.object_instances d
            INNER JOIN ad.import_object_recs nd
            ON d.sd_oid = nd.sd_oid
            WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_instances", "adding");
		}

		public void TransferObjectTitles()
		{
			string sql_string = @"INSERT INTO ad.object_titles(sd_oid, 
            title_type_id, title_text, lang_code,
            lang_usage_id, is_default, comments, comparison_text, record_hash)
            SELECT d.sd_oid, 
            title_type_id, title_text, lang_code,
            lang_usage_id, is_default, comments, comparison_text, record_hash
            FROM sd.object_titles d
            INNER JOIN ad.import_object_recs nd
            ON d.sd_oid = nd.sd_oid
            WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_titles", "adding");
		}
		

		public void TransferObjectDates()
		{
			string sql_string = @"INSERT INTO ad.object_dates(sd_oid, 
            date_type_id, is_date_range, date_as_string, start_year, 
            start_month, start_day, end_year, end_month, end_day, details, record_hash)
            SELECT d.sd_oid, 
            date_type_id, is_date_range, date_as_string, start_year, 
            start_month, start_day, end_year, end_month, end_day, details, record_hash
            FROM sd.object_dates d
            INNER JOIN ad.import_object_recs nd
            ON d.sd_oid = nd.sd_oid
            WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_dates", "adding");
		}

		public void TransferObjectContributors()
		{
			string sql_string = @"INSERT INTO ad.object_contributors(sd_oid, 
            contrib_type_id, is_individual, organisation_id, organisation_name,
            person_id, person_given_name, person_family_name, person_full_name,
            person_identifier, identifier_type, person_affiliation, affil_org_id,
            affil_org_id_type, record_hash)
            SELECT d.sd_oid, 
            contrib_type_id, is_individual, organisation_id, organisation_name,
            person_id, person_given_name, person_family_name, person_full_name,
            person_identifier, identifier_type, person_affiliation, affil_org_id,
            affil_org_id_type, record_hash
            FROM sd.object_contributors d
            INNER JOIN ad.import_object_recs nd
            ON d.sd_oid = nd.sd_oid
            WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_contributors", "adding");
		}

		public void TransferObjectTopics()
		{
			string sql_string = @"INSERT INTO ad.object_topics(sd_oid, 
            topic_type_id, mesh_coded, topic_code, topic_value, 
            topic_qualcode, topic_qualvalue, original_ct_id, original_ct_code,
            original_value, comments, record_hash)
            SELECT d.sd_oid,  
            topic_type_id, mesh_coded, topic_code, topic_value, 
            topic_qualcode, topic_qualvalue, original_ct_id, original_ct_code,
            original_value, comments, record_hash
            FROM sd.object_topics d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_topics", "adding");
		}


		public void TransferObjectComments()
		{
			string sql_string = @"INSERT INTO ad.object_comments(sd_oid, 
            ref_type, ref_source, pmid, pmid_version, notes, record_hash)
            SELECT d.sd_oid,  
            ref_type, ref_source, pmid, pmid_version, notes, record_hash
            FROM sd.object_comments d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_comments", "adding");
		}


		public void TransferObjectDescriptions()
		{
			string sql_string = @"INSERT INTO ad.object_descriptions(sd_oid, 
            description_type_id, label, description_text, lang_code, 
            contains_html, record_hash)
            SELECT d.sd_oid, 
            description_type_id, label, description_text, lang_code, 
            contains_html, record_hash
            FROM sd.object_descriptions d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_descriptions", "adding");
		}

		public void TransferObjectIdentifiers()
		{
			string sql_string = @"INSERT INTO ad.object_identifiers(sd_oid, 
            identifier_value, identifier_type_id, identifier_org_id, identifier_org,
            identifier_date, record_hash)
            SELECT d.sd_oid, 
            identifier_value, identifier_type_id, identifier_org_id, identifier_org,
            identifier_date, record_hash
            FROM sd.object_identifiers d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_identifiers", "adding");
		}

		public void TransferObjectDBLinks()
		{
			string sql_string = @"INSERT INTO ad.object_db_links(sd_oid, 
            db_sequence, db_name, id_in_db, record_hash)
            SELECT d.sd_oid, 
            db_sequence, db_name, id_in_db, record_hash
            FROM sd.object_db_links d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_db_links", "adding");
		}

		public void TransferObjectPublicationTypes()
		{
			string sql_string = @"INSERT INTO ad.object_publication_types(sd_oid, 
            type_name, record_hash)
            SELECT d.sd_oid, 
            type_name, record_hash
            FROM sd.object_publication_types d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_publication_types", "adding");
		}


		public void TransferObjectRights()
		{
			string sql_string = @"INSERT INTO ad.object_rights(sd_oid, 
            rights_name, rights_uri, comments, record_hash)
            SELECT d.sd_oid, 
            rights_name, rights_uri, comments, record_hash
            FROM sd.object_rights d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_rights", "adding");
		}


		public void TransferObjectRelationships()
		{
			string sql_string = @"INSERT INTO ad.object_relationships(sd_oid, 
            relationship_type_id, target_sd_oid, record_hash)
            SELECT d.sd_oid, 
            relationship_type_id, target_sd_oid, record_hash
            FROM sd.object_relationships d
			INNER JOIN ad.import_object_recs nd
			ON d.sd_oid = nd.sd_oid
			WHERE nd.status = 1";

			dbu.ExecuteTransferSQL(sql_string, "object_relationships", "adding");
		}

		#endregion


		public void UpdateObjectsLastImportedDate(int import_id, int source_id)
		{
			string top_string = @"UPDATE mon_sf.source_data_objects src
                          set last_import_id = " + import_id.ToString() + @", 
                          last_imported = current_timestamp
                          from 
                             (select so.id, so.sd_oid 
                              FROM sd.data_objects so
                              INNER JOIN ad.import_object_recs ts
                              ON so.sd_oid = ts.sd_oid
                             ";
			string base_string = @" where s.sd_sid = src.sd_id and
                              src.source_id = " + source_id.ToString();

			dbu.UpdateLastImportedDate("data_objects", top_string, base_string, "adding");
		}


		public void TransferObjectHashes()
		{
			for (int n = 50; n < 64; n++)
			{
				string sql_string = @"INSERT INTO ad.object_hashes(sd_oid, 
                 hash_type_id, composite_hash)
                 SELECT d.sd_oid,  
                 hash_type_id, composite_hash
                 FROM sd.object_hashes d
                 INNER JOIN ad.import_object_recs nd
			     ON d.sd_oid = nd.sd_oid
			     WHERE nd.status = 1
			     and d.hash_type_id = " + n.ToString();

				dbu.ExecuteSQL(sql_string);
				StringHelpers.SendFeedback("Inserting object hashes - type " + n.ToString());
			}
		}

	}
}

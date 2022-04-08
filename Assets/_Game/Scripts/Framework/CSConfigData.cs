using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace  CodeStringers.Framework {
    public interface CSConfigDataInterface {
        void LoadFromJson(string json);
        void LoadFromTextAsset(TextAsset asset);
        void LoadFromAssetPath(string path, bool useAppDataPath = true);
    }

    public class CSConfigData<TDataRecord> : CSConfigDataInterface where TDataRecord : class, new() {
        public bool isLoaded ;
		List<TDataRecord> records = new List<TDataRecord> ();
		public class IndexField<TIndex> : Dictionary<TIndex, List<TDataRecord>> { };
		Dictionary<string, object> rebuildIndex = new Dictionary<string, object> ();

		List<FieldInfo> fields;
	
		public List<TDataRecord> Records{
			get { return records;}
		}
			
		public class RecordJsonData
		{
			public List<TDataRecord> data;
		}
		
		public CSConfigData()
		{
			ReadFieldInfo ();
		}
		
		void ReadFieldInfo()
		{
			Type type = typeof(TDataRecord);
			FieldInfo[] fieldArr  = type.GetFields ();
			fields = new List<FieldInfo> ();
			foreach (FieldInfo filedInfo in fieldArr)
				if (!filedInfo.IsPrivate && !filedInfo.IsNotSerialized)
					fields.Add (filedInfo);
		}
		
		public void LoadFromAssetPath(string path, bool useAppDataPath = true)
		{
			if (fields == null || fields.Count == 0)
				return;
			
			FileInfo theSourceFile = null;
			TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

			if (useAppDataPath) {
				theSourceFile = new FileInfo (Application.dataPath + "/" + path);
			}
			else
				theSourceFile = new FileInfo (path);
			
			if ( theSourceFile != null && theSourceFile.Exists ) {
				reader = theSourceFile.OpenText();  // returns StreamReader
			}
			else {
				TextAsset puzdata = (TextAsset)Resources.Load(path, typeof(TextAsset));
				reader = new StringReader(puzdata.text);  // returns StringReader
			}
			if ( reader == null ) {
				Debug.Log("not found or not readable");
			}
			else {
				var line = 0;
				var txt = reader.ReadLine();// bo dong dau
				while(true) {
					txt = reader.ReadLine();
					line++;
					if(txt == null) break;
					TDataRecord record =  new TDataRecord();//Activator.CreateInstance<TDataRecord>();
					
					string[] columns = txt.Split('\t');
					
					if(columns == null || columns.Length < fields.Count) {
						//Debug.LogError( "Load config " + path + " line " + line + " error " +columns.Length +"," +fields.Count) ;
						//Debug.LogError("Try splitting with ','");
						columns = txt.Split(',');
						if (columns == null || columns.Length < fields.Count)
						{
							Debug.LogError("Load config " + path + " line " + line + " error " + columns.Length + "," + fields.Count);
							Debug.LogError("still fail");
							continue;
						}
					}
					
					int i = 0;
					bool error =  false;
					
					foreach(FieldInfo field in fields) {
						object convert = null;
						if (field.FieldType == typeof(int) && string.IsNullOrEmpty(columns[i])) {
							convert = 0;
						} else {
							convert = ConvertData(columns[i], field.FieldType);
						}
						if(convert != null)
							field.SetValue(record, convert);
						else {
							error = true;
							break;
						}
						
						i++;
					}
					if(error) {
						Debug.LogError( "Load config " + path + " line " + line + " error");
						continue;
					}
					records.Add(record);
					
				}
				RebuildIndex();
				isLoaded = true;
			}
		}

		public void LoadFromTextAsset(TextAsset textAsset)
		{
	        TextReader reader = new StringReader(textAsset.text);  // returns StringReader
			if ( reader == null ) {
				Debug.Log("not found or not readable");
			}
			else {
				int line = 0;
				string txt = reader.ReadLine();// bo dong dau
				while(true) {
					txt = reader.ReadLine();
					line++;
					if(txt == null) break;
					TDataRecord record =  new TDataRecord();//Activator.CreateInstance<TDataRecord>();

					string[] columns = txt.Split('\t');

					if(columns == null || columns.Length < fields.Count) {
						Debug.LogError( "Load textasset " + textAsset.name + " line " + line + " error " +columns.Length +"," +fields.Count) ;
						continue;
					}

					int i = 0;
					bool error =  false;

					foreach(FieldInfo field in fields) {
						object convert = null;
						if (field.FieldType == typeof(int) && string.IsNullOrEmpty(columns[i])) {
							convert = 0;
						} else {
							convert = ConvertData(columns[i], field.FieldType);
						}					if(convert != null)
							field.SetValue(record, convert);
						else {
							error = true;
							break;
						}

						i++;
					}
					if(error) {
						Debug.LogError( "Load textasset " + textAsset.name + " line " + line + " error");
						continue;
					}
					records.Add(record);

				}
				RebuildIndex();
				isLoaded = true;
			}
		}

		public void LoadFromJson(string json)
		{
			try{
	            var parsed = JsonConvert.DeserializeObject<List<TDataRecord>> (json);
				if (parsed != null)
					records = parsed;
				RebuildIndex();
			}
			catch(Exception ex){
				Debug.LogErrorFormat ("{0} Load Json Failed!. Error: {1}", typeof(TDataRecord).Name, ex.Message);
			}
		}

		public string ConvertToJson()
		{
			try{
				RecordJsonData jsonRecord = new RecordJsonData ();
				jsonRecord.data = records;
				return JsonConvert.SerializeObject (jsonRecord);
			}
			catch(Exception ex) {
				Debug.LogError (ex.Message);
				return string.Empty;
			}
		}

		#if UNITY_EDITOR
		// public void WriteToAssetPath(string filePath)
		// {
		// 	if (fields == null || fields.Count == 0)
		// 		return;
		//
		// 	string extension 	= ".csv";
		// 	string fullPath 	= Application.dataPath + "/" + SourcePath.GAME_RESOURCES_PATH + filePath + extension;
		//
		// 	TextWriter writer 	= new StreamWriter(fullPath, false, Encoding.Unicode);
		// 	StringBuilder sb 	= new StringBuilder();
		//
		// 	//write header name
		// 	for (int i = 0; i < fields.Count; i++) {
		// 		string headerName = fields[i].Name;
		// 		if (i < fields.Count - 1)
		// 			sb.AppendFormat ("{0}\t", headerName);
		// 		else
		// 			sb.Append (headerName);
		// 	}
		//
		// 	writer.WriteLine (sb);
		//
		// 	//write each records
		// 	int idx = 0;
		// 	foreach (TDataRecord record in records){
		// 		sb.Clear ();
		// 		for (int i = 0; i < fields.Count; i++) {
		// 			FieldInfo fInfo = fields [i];
		// 			object val = fInfo.GetValue (record);
		//
		// 			if (i < fields.Count - 1)
		// 				sb.AppendFormat ("{0}\t", val.ToString());
		// 			else
		// 				sb.Append (val.ToString());
		// 		}
		// 		if(idx < records.Count - 1)
		// 			writer.WriteLine (sb);
		// 		else
		// 			writer.Write (sb);
		// 		idx++;
		// 	}
		//
		// 	writer.Close ();
		//
		// 	UnityEditor.AssetDatabase.Refresh(); 
		// 	Debug.Log ("DONE");
		// }
		#endif

		public void Clear()
		{
			records.Clear ();
			rebuildIndex.Clear ();
		}
		
		public virtual void RebuildIndex()
		{
			
		}
		
		protected void RebuildIndexByField<TIndex>(string fieldName)
		{
	//		object dic;
			Type recordType = typeof(TDataRecord);
			FieldInfo fieldInfo = recordType.GetField(fieldName);
			if (fieldInfo == null)
				throw new Exception("Field [" + fieldName + "] not found");
			IndexField<TIndex> indexField = new IndexField<TIndex>();
			rebuildIndex [fieldName] = indexField;
			foreach (TDataRecord record in records)
			{
				var fieldValue = (TIndex) fieldInfo.GetValue(record);
				
				List<TDataRecord> indexedValue;
				if (!indexField.TryGetValue(fieldValue, out indexedValue))
				{
					indexedValue = new List<TDataRecord>();
					indexField[fieldValue] = indexedValue;
				}
				indexedValue.Add(record);
				
			}
		}
		
		
		
		public TDataRecord GetRecordByIndex<TIndex>(string fieldName, TIndex compareValue )
		{
			object dic = null;
			if(rebuildIndex.TryGetValue(fieldName, out dic))
			{
				IndexField<TIndex> indexField = (IndexField<TIndex>)dic;
				List<TDataRecord> resultList = null;
				if(indexField.TryGetValue(compareValue, out resultList))
					if(resultList.Count > 0) return resultList[0];
				
				return null;
			}
			return null;
		}
		
		public List<TDataRecord> GetRecordsByIndex<TIndex>(string fieldName, TIndex compareValue )
		{
			object dic = null;
			if(rebuildIndex.TryGetValue(fieldName, out dic))
			{
				IndexField<TIndex> indexField = (IndexField<TIndex>)dic;
				List<TDataRecord> resultList = null;
				if(indexField.TryGetValue(compareValue, out resultList))
					if(resultList.Count > 0) return resultList;
				
				return null;
			}
			return null;
		}
		
		object ConvertData(string value, Type t)
		{
			if (t.IsEnum) {
				Array arr = Enum.GetValues (t);
				if(string.IsNullOrEmpty(value))
					return arr.GetValue(0);
				foreach (object item in arr) {
					if (item.ToString ().ToLower ().Equals (value.Trim ().ToLower ()))
						return item;
				}
			} else {
				TypeCode typeCode = Type.GetTypeCode (t);
				if (typeCode == TypeCode.Int32) {
					int result;
					if (int.TryParse (value, out result))
						return result;
					return null;
				} else if (typeCode == TypeCode.Int64) {
					long result;
					if (long.TryParse (value, out result))
						return result;
					return null;
				} else if (typeCode == TypeCode.Int16){
					short result;
					if(short.TryParse(value, out result))
						return result;
					return null;
				} else if (typeCode == TypeCode.Single || typeCode == TypeCode.Decimal) {
					float result;
					if (float.TryParse (value, out result))
						return result;
					return null;
				} else if (typeCode == TypeCode.String){
					//xu ly xuong dong
					string[] regex = { @"\n" };
					string[] temp2 = value.Split(regex, StringSplitOptions.None);
					value = "";
					for (int i = 0; i < temp2.Length; i++) {
						if (i == temp2.Length - 1) {
							if (!string.IsNullOrEmpty (temp2 [i])) {
								//remove some invalid char for json string
								temp2 [i] = temp2 [i].Replace (@"""""", @"""");//replace double " for json string in csv
								if (temp2 [i].IndexOf ('"') == 0)
									temp2 [i] = temp2 [i].Remove (0, 1);
							
								if (temp2 [i].LastIndexOf ('"') == temp2 [i].Length - 1)
									temp2 [i] = temp2 [i].Substring (0, temp2 [i].Length - 1);
							}
							value += temp2[i];
							break;
						}
						value += temp2[i] + "\n";
					}
					return value;
				}
				else if (typeCode == TypeCode.Boolean) {
					bool result;
					if (bool.TryParse (value, out result))
						return result;
					if (value == "0")
						return false;
					else if (value == "1")
						return true;
				} else if (typeCode == TypeCode.Double) {
					double result;
					if (double.TryParse (value, out result))
						return result;
					return null;
				} else if (typeCode == TypeCode.DateTime) {
					DateTime result;
					if (DateTime.TryParseExact (value, "yyyy-MM-dd HH:mm:ss,fff",
					                            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
						return result;
					return  null;
				}
			}
			return null;
		}

		public void CopyFrom(List<TDataRecord> desRecords)
		{
			if (records.GetType() != desRecords.GetType ()) {
				Debug.LogError ("Wrong DesType");
				return;
			}
			
			records.Clear ();
			for (int i = 0; i < desRecords.Count; i++)
				records.Add (desRecords [i]);
		}
			
		public void CopyTo(List<TDataRecord> newRecords)
		{
			newRecords.Clear ();
			for (int i = 0; i < records.Count; i++)
				newRecords.Add (records [i]);
		}
	}
}



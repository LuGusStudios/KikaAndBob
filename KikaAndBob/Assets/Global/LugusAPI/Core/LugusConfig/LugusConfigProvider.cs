using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public interface ILugusConfigProvider
{
	string URL { get; set; }

	Dictionary<string, string> Load(string name);

	void Store(Dictionary<string, string> data, string name);

	List<string> Scan();
}

public class LugusConfigProviderDefault : ILugusConfigProvider
{

	#region Properties
	public virtual string URL
	{
		get
		{
			return _url;
		}
		set
		{
			_url = value;
		}
	}
	public List<ILugusConfigDataHelper> Parsers
	{
		get
		{
			return _parsers;
		}
		set
		{
			_parsers = value;
		}
	}

	[SerializeField]
	protected string _url = "";

	protected List<ILugusConfigDataHelper> _parsers = null;
	#endregion

	// Adds an XML parser as standard parser.
	public LugusConfigProviderDefault(string url)
	{
		_url = url;

		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(new LugusConfigDataHelperXML());
		//_parsers.Add(new LugusConfigDataHelperJSON());
	}

	// Use the given parser to parse data loaded by the provider.
	public LugusConfigProviderDefault(string url, ILugusConfigDataHelper parser)
	{
		_url = url;
		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(parser);

	}

	// Use the given list of parsers to parse data loaded by the provider.
	public LugusConfigProviderDefault(string url, List<ILugusConfigDataHelper> parsers)
	{
		_url = url;
		_parsers = parsers;
	}

	public Dictionary<string, string> Load(string name)
	{
		// Find config files that can be parsed by the parsers in the list.
		// Data parsed by multiple parsers will be merged.
		// Data with the same key from different parsers will result in the latest parsed value with that key.
		// So the order in which the parsers are added is important!

		Dictionary<string, string> data = new Dictionary<string, string>();

		if (!Directory.Exists(URL))
		{
			Debug.LogError("LugusConfigProviderDefault.Load(): The following directory does not exist: " + URL);
			return data;
		}

		foreach (ILugusConfigDataHelper parser in _parsers)
		{
			string fullpath = URL + name + parser.FileExtension;

			// Read the raw data out of the file
			if (File.Exists(fullpath))
			{
				StreamReader reader = new StreamReader(fullpath, Encoding.Default);
				string rawdata = reader.ReadToEnd();
				reader.Close();

				Dictionary<string, string> partialData = parser.ParseFrom(rawdata);

				// Merge with data and resolve duplicate keys
				foreach (KeyValuePair<string, string> entry in partialData)
				{
					if (data.ContainsKey(entry.Key))
						data[entry.Key] = entry.Value;
					else
						data.Add(entry.Key, entry.Value);
				}
			}
		}

		return data;
	}

	public void Store(Dictionary<string, string> data, string name)
	{
		if (!Directory.Exists(URL))
		{
			try
			{
				Directory.CreateDirectory(URL);
			}
			catch (System.Exception)
			{
				return;
			}
		}

		foreach (ILugusConfigDataHelper parser in _parsers)
		{
			string rawData = parser.ParseTo(data);

			// Compose the full path to write away to profile
			string fullpath = URL + name + parser.FileExtension;

			// Write the raw data out to a file
			StreamWriter writer = new StreamWriter(fullpath);
			writer.Write(rawData);
			writer.Close();
		}
	}

	public List<string> Scan()
	{
		List<string> names = new List<string>();

		if (!Directory.Exists(URL))
		{
			Debug.LogError("LugusConfigProviderDefault.Names(): The following directory does not exist: " + URL);
			return names;
		}

		// TODO: Currently very naive implementation and very inefficient when there are many files
		// Replace with a more efficient file accessing algorithm!
		DirectoryInfo directoryInfo = new DirectoryInfo(URL);
		foreach (ILugusConfigDataHelper parser in _parsers)
		{
			FileInfo[] files = directoryInfo.GetFiles("*" + parser.FileExtension);
			foreach (FileInfo file in files)
			{
				string name = file.Name.Remove(file.Name.LastIndexOf(parser.FileExtension));

				if (!names.Exists(n => n == name))
					names.Add(name);
			}
		}

		return names;
	}

}
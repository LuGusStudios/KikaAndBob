/*
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LugusConfig : LugusSingletonRuntime<LugusConfigDefault>
{
	
}

public class LugusConfigDefault : MonoBehaviour
{
	#region Properties
	public ILugusConfigProfile User
	{
		get
		{
			if (_currentUser == null)
				ReloadDefaultProfiles();

			return _currentUser;
		}
		set
		{
			_currentUser = value;
		}
	}
	public ILugusConfigProfile System
	{
		get
		{
			if (_systemProfile == null)
				ReloadDefaultProfiles();

			return _systemProfile;
		}
		set
		{
			_systemProfile = value;
		}
	}
	public List<ILugusConfigProfile> AllProfiles
	{
		get
		{
			return _profiles;
		}
		set
		{
			_profiles = value;
		}
	}
	#endregion

	#region Protected
	protected ILugusConfigProfile _systemProfile = null;								// Profile holding system variables, i.e. graphics and sound options.
	protected ILugusConfigProfile _currentUser = null;									// Profile holding user specific variables, i.e. character health and strength.
	protected List<ILugusConfigProfile> _profiles = new List<ILugusConfigProfile>();	// All profiles registered in this configuration, incl. system profile.
	#endregion

	// Reload all profiles found in the Config folder with an .xml-extension.
	public void ReloadDefaultProfiles()
	{
		ReloadProfiles(new LugusConfigDataHelperXML());
	}

	// Reload all profiles found in the Config folder that have a specific extension.
	public void ReloadProfiles(ILugusConfigDataHelper dataHelper)
	{
		List<ILugusConfigDataHelper> dataHelpers = new List<ILugusConfigDataHelper>();
		dataHelpers.Add(dataHelper);
		ReloadProfiles(dataHelpers);
	}

	// Reload all profiles found in the Config folder that have one of the file extensions in the list.
	public void ReloadProfiles(List<ILugusConfigDataHelper> dataHelpers)
	{
		LugusConfigProviderDefault provider = new LugusConfigProviderDefault(Application.dataPath + "/Config/", dataHelpers);
		ReloadProfiles(provider);
	}

	// Reload all profiles that can be found by the provider.
	public void ReloadProfiles(ILugusConfigProvider provider)
	{
		List<ILugusConfigProvider> providers = new List<ILugusConfigProvider>();
		providers.Add(provider);
		ReloadProfiles(providers);
	}

	// Reload all profiles that can be found by the list of providers.
	public void ReloadProfiles(List<ILugusConfigProvider> providers)
	{
		ClearProfiles();

		// Find all profiles that can be found by the providers, and load them
		foreach (ILugusConfigProvider provider in providers)
		{
			List<string> names = provider.Scan();
			foreach (string name in names)
			{
				LugusConfigProfileDefault profile = new LugusConfigProfileDefault(name, provider);
				profile.Load();

				// Ensure that a profile with a duplicate name is overwritten
				if (_profiles.Exists(p => p.Name == name))
					_profiles[_profiles.FindIndex(p => p.Name == name)] = profile;
				else
					_profiles.Add(profile);
			}
		}

		// Find the system profile, if there is one, and find the latest user
		ILugusConfigProfile system = FindProfile("System");
		if (system == null)
		{
			system = new LugusConfigProfileDefault("System");
			_systemProfile = system;
			_profiles.Add(system);
		}
		else
		{
			_systemProfile = system;
			string lastestUser = _systemProfile.GetString("User.Latest", string.Empty);
			if (!string.IsNullOrEmpty(lastestUser))
				_currentUser = _profiles.Find(profile => profile.Name == lastestUser);
		}

		if (_currentUser == null)
		{
			_currentUser = new LugusConfigProfileDefault("Player");
			_profiles.Add(_currentUser);
		}

	}

	public void SaveProfiles()
	{

		if ((_systemProfile != null) && (_currentUser != null))
			_systemProfile.SetString("User.Latest", _currentUser.Name, true);

		foreach (ILugusConfigProfile profile in _profiles)
			profile.Store();
	}

	public ILugusConfigProfile FindProfile(string name)
	{
		return _profiles.Find(profile => profile.Name == name);
	}

	public void ClearProfiles()
	{
		_profiles = new List<ILugusConfigProfile>();
		_systemProfile = null;
		_currentUser = null;
	}

}*/
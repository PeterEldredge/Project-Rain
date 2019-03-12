public interface ISaveable
{
	string SAVE_FILE
	{
		get;
	}

	void Save();
	void Load();
}

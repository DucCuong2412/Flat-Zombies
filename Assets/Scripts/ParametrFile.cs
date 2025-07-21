public struct ParametrFile
{
	public static bool makeSaveFile = false;

	public static ParametrFile[] variables = new ParametrFile[200];

	public static readonly string StringTrue = "true";

	public static readonly string StringFalse = "false";

	public string name;

	public string value;

	public bool IsEmpty()
	{
		return string.IsNullOrEmpty(name);
	}

	public override string ToString()
	{
		return name + ":" + value;
	}
}

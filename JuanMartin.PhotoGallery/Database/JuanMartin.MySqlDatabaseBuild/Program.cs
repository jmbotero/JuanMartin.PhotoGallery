using System.Diagnostics;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
const string databasePath = @"C:\GitHub\JuanMartin.PhotoGallery\JuanMartin.PhotoGallery\Database";

//AlterMySqlScripts("Tables", databasePath);
//AlterMySqlScripts("Functions", databasePath);
//AlterMySqlScripts("Views", databasePath);
//AlterMySqlScripts("Procedures", databasePath, "USE Photo_Gallery\r\nDELIMITER //\r\n\r\n");
//AlterMySqlScripts("Data", databasePath);

LoadMySqlObjects("Tables", databasePath, "local");
LoadMySqlObjects("Functions", databasePath, "local");
LoadMySqlObjects("Views", databasePath, "local");
LoadMySqlObjects("Procedures", databasePath, "local");
LoadMySqlObjects("Data", databasePath, "local");

static void AlterMySqlScripts(string objectName, string databaseObjectsPath, string header = "")
{
	string path = @$"{databaseObjectsPath}\{objectName}";

	if (!Directory.Exists(path))
		throw new FileNotFoundException(path);

	if (string.IsNullOrEmpty(header)) header = "USE Photo_Gallery\r\n\r\n";

	var files = Directory.EnumerateFiles(path);
	foreach (var file in files)
	{
		AddHeaderAndFooterToTextFile(file, header);
	}
}


static void AddHeaderAndFooterToTextFile(string textFile, string header, string footer = "")
{
	char[] buffer = new char[10000];

	string renamedFile = textFile + ".orig";
	File.Move(textFile, renamedFile);

	using (StreamReader sr = new StreamReader(renamedFile))
	using (StreamWriter sw = new StreamWriter(textFile, false))
	{
		if(!string.IsNullOrEmpty(header)) sw.Write(header);

		int read;
		while ((read = sr.Read(buffer, 0, buffer.Length)) > 0)
			sw.Write(buffer, 0, read);

		 if(!string.IsNullOrEmpty(footer)) sw.Write(footer);
	}

	File.Delete(renamedFile);
}

static void LoadMySqlObjects(string objectName, string databaseObjectsPath, string mySqlLoginPath)
{
	string systemMySqlPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin";
	if (mySqlLoginPath == null)
		throw new ArgumentException("Login path not specified");

	string path = @$"{databaseObjectsPath}\{objectName}";

	if (!Directory.Exists(path))
		throw new FileNotFoundException(path);

	ProcessStartInfo startInfo = new()
	{
		FileName = "cmd.exe",
		ErrorDialog = false,
		UseShellExecute = false,
		WindowStyle = ProcessWindowStyle.Hidden,
		CreateNoWindow = true,
		RedirectStandardInput = true,
		RedirectStandardOutput= true,
		RedirectStandardError = true
	};

	var files = Directory.EnumerateFiles(path);
	foreach (var file in files)
	{
		var command = @"""" + systemMySqlPath + @"\mysql"" --login-path=" + mySqlLoginPath + @" < """ + file + @"""";
		//command = @"echo " + command;

 		Process process = new()
		{
			StartInfo = startInfo
		};
		process.Start();
		try
		{
			process.StandardInput.WriteLine(command);
			process.StandardInput.Flush();
			process.StandardInput.Close();
			process.WaitForExit();

			var output = process.StandardOutput.ReadToEnd();
			var error = process.StandardError.ReadToEnd();
			if (error != null && error.Length > 0)
			{
				throw new FileLoadException(error);
			}
		}
		catch (Exception err)
		{
			Console.WriteLine(err.Message);
		}
	}
}
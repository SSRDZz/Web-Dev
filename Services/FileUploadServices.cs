namespace KMITL_WebDev_MiniProject.Services;
public class FileUploadServcies(IWebHostEnvironment Env)
{
	private IWebHostEnvironment Env {get; init;} = Env;
	public string LastExt {get; set;}

	public async Task Upload(IFormFile File, string Name)
	{
		if(!FileIsExist(File) || string.IsNullOrEmpty(Name))
			return ;
			
		LastExt = GetExt(File);
		string FilePath = Path.Combine(Env.WebRootPath, "image", "UserProfile", Name + LastExt);

		using (var stream = System.IO.File.Create(FilePath))
			await File.CopyToAsync(stream);
	}

	public bool FileIsExist(IFormFile File)
	{
		return File != null && File.Length > 0;
	}

	private string? GetExt(IFormFile File)
	{
		if(!FileIsExist(File))
			return null;

		string str = File.ContentType.ToString(), ext = ".";
		int i = 0;
		for(; i < str.Length;)
			if(str[i++] == '/') 
				break;

		for(; i < str.Length; i++)
			ext += str[i];

		return ext;
	}
}
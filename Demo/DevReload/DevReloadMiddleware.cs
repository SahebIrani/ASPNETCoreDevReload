using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Abiosoft.DotNet.DevReload
{

	public class DevReloadMiddleware
	{

		private string _time;

		private readonly RequestDelegate _next;
		private FileSystemWatcher _watcher;
		private DevReloadOptions _options;

		public DevReloadMiddleware(
			RequestDelegate next,
			IOptions<DevReloadOptions> options)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			_time = System.DateTime.Now.ToString();
			_watcher = new FileSystemWatcher();
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_options = options.Value;

			Task.Run(Watch);
		}

		public Task Invoke(HttpContext c)
		{
			if (c.Request.Path.StartsWithSegments("/__DevReload"))
			{
				if (c.Request.Headers.ContainsKey("ping"))
					return c.Response.WriteAsync(_time);

				c.Response.ContentType = "application/javascript";
				return c.Response.WriteAsync(Js.Script);
			}
			return _next(c);
		}

		private void Watch()
		{

			_watcher.Path = _options.Directory;

			_watcher.NotifyFilter = NotifyFilters.LastWrite
								 | NotifyFilters.FileName
								 | NotifyFilters.DirectoryName;

			_watcher.Changed += OnChanged;
			_watcher.Created += OnChanged;
			_watcher.Deleted += OnChanged;
			_watcher.IncludeSubdirectories = true;

			_watcher.EnableRaisingEvents = true;
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			foreach (string ignoredDirectory in _options.IgnoredSubDirectories)
			{
				var sep = Path.DirectorySeparatorChar;
				if (e.FullPath.Contains($"{sep}{ignoredDirectory}{sep}")) return;
			}

			FileInfo fileInfo = new FileInfo(e.FullPath);
			if (_options.StaticFileExtensions.Length > 0)
			{
				foreach (string extension in _options.StaticFileExtensions)
				{
					if (fileInfo.Extension.Equals($".{extension.TrimStart('.')}"))
					{
						_time = System.DateTime.Now.ToString();
						break;
					}
				}
			}
			else
			{
				_time = System.DateTime.Now.ToString();
			}
			Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
		}
	}
}

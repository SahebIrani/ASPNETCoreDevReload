using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

using System;

namespace Abiosoft.DotNet.DevReload
{
	public class DevReloadOptions
	{
		public string Directory { get; set; }
		public string[] IgnoredSubDirectories { get; set; }
		public string[] StaticFileExtensions { get; set; }
	}

	public static class MiddlewareHelpers
	{
		public static IApplicationBuilder UseDevReload(this IApplicationBuilder app)
		{
			if (app == null)
				throw new ArgumentNullException(nameof(app));

			return app.UseDevReload(new DevReloadOptions
			{
				Directory = "./wwwroot",
				IgnoredSubDirectories = new string[] { ".git", ".node_modules" },
				StaticFileExtensions = new string[] { "js", "html", "css", }
			});
		}
		public static IApplicationBuilder UseDevReload(this IApplicationBuilder app, DevReloadOptions options)
		{
			if (app == null)
				throw new ArgumentNullException(nameof(app));

			if (options == null)
				throw new ArgumentNullException(nameof(options));

			return app.UseMiddleware<DevReloadMiddleware>(Options.Create(options));
		}
	}
}

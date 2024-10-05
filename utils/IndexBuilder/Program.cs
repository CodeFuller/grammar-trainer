using System.Threading.Tasks;
using CodeFuller.Library.Bootstrap;

namespace IndexBuilder
{
	public static class Program
	{
		public static async Task<int> Main(string[] args)
		{
			using var bootstrapper = new ApplicationBootstrapper();
			var application = new ConsoleApplication(bootstrapper);

			return await application.Run(args).ConfigureAwait(false);
		}
	}
}

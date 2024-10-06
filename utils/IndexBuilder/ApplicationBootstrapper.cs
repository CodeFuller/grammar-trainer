using CodeFuller.Library.Bootstrap;
using CodeFuller.Library.Logging;
using IndexBuilder.Interfaces;
using IndexBuilder.Internal;
using IndexBuilder.WikitextParsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IndexBuilder
{
	internal sealed class ApplicationBootstrapper : BasicApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<IWiktionaryPageParser, WiktionaryPageParser>();
			services.AddSingleton<IFormValuesParser, PidginFormValuesParser>();

			services.AddSingleton<IWordDefinitionsSerializer, JsonWordDefinitionsSerializer>();

			services.AddSingleton<IApplicationLogic, ApplicationLogic>();
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.AddLogging(settings => configuration.Bind("logging", settings));
		}
	}
}

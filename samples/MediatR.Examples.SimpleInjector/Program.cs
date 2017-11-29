using System.Threading.Tasks;
using MediatR.Pipeline;

namespace MediatR.Examples.SimpleInjector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::SimpleInjector;

    internal class Program
    {
        private static Task Main(string[] args)
        {
            var mediator = BuildMediator();

            return Runner.Run(mediator, Console.Out, "SimpleInjector");
        }

        private static IMediator BuildMediator()
        {
            var container = new Container();
            var assemblies = GetAssemblies().ToArray();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);
			container.Register(typeof(IRequestHandler<>), assemblies);
            container.RegisterCollection(typeof(INotificationHandler<>), assemblies);
            container.RegisterSingleton(Console.Out);

            //Pipeline
            container.RegisterCollection(typeof(IPipelineBehavior<,>), new []
            {
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>),
                typeof(GenericPipelineBehavior<,>)
            });
            container.RegisterCollection(typeof(IRequestPreProcessor<>), new [] { typeof(GenericRequestPreProcessor<>)});
            container.RegisterCollection(typeof(IRequestPostProcessor<,>), new[] { typeof(GenericRequestPostProcessor<,>) });

            container.RegisterSingleton(new SingleInstanceFactory(container.GetInstance));
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));

            container.Verify();

            var mediator = container.GetInstance<IMediator>();

            return mediator;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(Ping).GetTypeInfo().Assembly;
        }
    }
}

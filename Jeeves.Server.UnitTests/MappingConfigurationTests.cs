using System.Reflection;
using AutoMapper;
using NUnit.Framework;

namespace Jeeves.Server.UnitTests
{
    [TestFixture]
    public class MappingConfigurationsTests
    {
        [Test] 
        public void MappingConfigurationShouldBeValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(expression =>
            {
                expression.AddMaps(Assembly.GetAssembly(typeof(Startup)));
            });

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
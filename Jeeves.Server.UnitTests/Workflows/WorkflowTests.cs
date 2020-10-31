using FakeItEasy;
using FluentAssertions;
using Jeeves.Server.Challenges;
using Jeeves.Server.Challenges.Steps;
using NUnit.Framework;

namespace Jeeves.Server.UnitTests.Workflows
{
    [TestFixture]
    public class WorkflowTests
    {
        [TestCase("hyphenated-name")]
        [TestCase("$special%^^&characters!@#name")]
        [TestCase("PascalCaseName")]
        [TestCase("camelCaseName")]
        [TestCase(null)]
        public void ShouldParseWorkflowName(string name)
        {
            //Arrange
            var yaml = $"name: {name}";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.Name.Should().Be(name);
        }
        
        [Test]
        public void ShouldParseWorkflowHostType()
        {
            //Arrange
            var yaml = $"runs-on: windows-latest";
            var parser = new WorkflowParser(A.Dummy<IWorkflowStepResolver>());
            
            //Act
            var workflow = parser.Parse(yaml);
            
            //Assert
            workflow.HostType.Should().Be("windows-latest");
        }
    }
}
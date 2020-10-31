using System.Collections;
using System.Linq;
using FluentAssertions;
using Jeeves.Server.Challenges;
using Jeeves.Server.Challenges.Steps;
using Jeeves.Server.IntegrationTests.Tests.WorkflowParser.Samples;
using NUnit.Framework;

namespace Jeeves.Server.IntegrationTests.Tests.WorkflowParser
{
    [TestFixture]
    public class WorkflowTests
    {
        public static IEnumerable Samples
        {
            get
            {
                yield return new TestCaseData(SimpleWorkflow.Yaml, SimpleWorkflow.Workflow);
            }
        }
        
        [TestCaseSource(nameof(Samples))]
        public void ShouldParseWorkflow(Yaml yaml, Workflow workflow)
        {
            //Arrange
            var expectedWorkflow = workflow;
            TestContext.Out.WriteLine("YAML");
            TestContext.Out.WriteLine(yaml.Content);
            var stepContract = typeof(IWorkflowStep);
            var stepTypes = stepContract.Assembly.GetTypes().Where(t => stepContract.IsAssignableFrom(t)).ToArray();
            var resolver = new WorkflowStepResolver(stepTypes);
            var parser = new Challenges.WorkflowParser(resolver);
            
            //Act
            var actualWorkflow = parser.Parse(yaml.Content);

            //Assert
            actualWorkflow.Should().BeEquivalentTo(expectedWorkflow);
        }
    }
}
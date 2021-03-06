using Microsoft.CodeAnalysis;
using APIView;
using Xunit;

namespace APIViewTest
{
    public class AssemblyTests
    {
        [Fact]
        public void AssemblyTestAssemblyFromSymbol()
        {
            IAssemblySymbol assemblySymbol = TestResource.GetAssemblySymbol();
            AssemblyAPIV assembly = new AssemblyAPIV(assemblySymbol);
            
            Assert.Equal("TestLibrary", assembly.Name);

            Assert.Single(assembly.GlobalNamespace.Namespaces);
        }

        [Fact]
        public void AssemblyTestAssembliesFromFile()
        {
            AssemblyAPIV assembly = AssemblyAPIV.AssemblyFromFile("TestLibrary.dll");
            Assert.Equal("TestLibrary", assembly.Name);

            NamespaceAPIV globalNamespace = assembly.GlobalNamespace;
            Assert.Single(globalNamespace.Namespaces);
        }
    }
}

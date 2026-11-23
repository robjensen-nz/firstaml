using System.Reflection;

namespace FirstAML.Tests;

public class LibrarySmokeTests
{
    [Fact]
    public void LibraryAssemblyCanBeLoaded()
    {
        var assembly = Assembly.Load("FirstAML");

        Assert.Equal("FirstAML", assembly.GetName().Name);
    }
}

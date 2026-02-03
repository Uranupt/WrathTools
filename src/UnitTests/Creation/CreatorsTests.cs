using Xunit;


namespace WrathTools.UnitTests
{
  public class CreatorsTests
  {

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    public void SelfCreator_UncastArityTest(int arity)
    {
      Type[] types = new Type[arity];
      object[] inputs = new object[arity];
      for(int i = 0; i < arity; i++)
      {
        types[i] = typeof(int);
        inputs[i] = 0;
      }
      ICreator creator = Creators.GetCreator(typeof(SelfCreator), types);
      Assert.NotNull(creator);
      SelfCreator instance = (SelfCreator)creator.Create(inputs);
      Assert.NotNull(instance);
    }

    [Fact]
    public void SelfCreator_ArityTestN0()
    {
      Creator<SelfCreator> creator
        = (Creator<SelfCreator>)Creators.GetCreator(typeof(SelfCreator));
      Assert.NotNull(creator);
      Assert.Equal(Creators.DefaultCreatorName, creator.Name);
      Assert.NotNull(creator.Create());
    }

    [Fact]
    public void SelfCreator_ArityTestN1()
    {
      Creator<int, SelfCreator> creator
        = (Creator<int, SelfCreator>)Creators.GetCreator(typeof(SelfCreator), typeof(int));
      Assert.NotNull(creator);
      Assert.NotNull(creator.Create(0));
    }


    [Fact]
    public void SelfCreator_ArityTestN2()
    {
      Creator<int, int, SelfCreator> creator
        = (Creator<int, int, SelfCreator>)Creators.GetCreator(typeof(SelfCreator), typeof(int), typeof(int));
      Assert.NotNull(creator);
      Assert.NotNull(creator.Create(0, 0));
    }

    [Fact]
    public void SelfCreator_ArityTestN15()
    {
      Type t = typeof(int);
      Creator<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, SelfCreator> creator
        = (Creator<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, SelfCreator>)Creators
        .GetCreator(typeof(SelfCreator), t, t, t, t, t, t, t, t, t, t, t, t, t, t, t);
      Assert.NotNull(creator);
      Assert.NotNull(creator.Create(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
    }

    [Fact]
    public void SelfCreatorInt_GenericFactoryTest()
    {
      Creator<SelfCreator<int>> creator = (Creator<SelfCreator<int>>)Creators.GetCreator(typeof(SelfCreator<int>));
      Assert.NotNull(creator);
      Assert.NotNull(creator.Create());
    }

    [Fact]
    public void SelfCreatorString_GenericFactoryTest()
    {
      Creator<SelfCreator<string>> creator = (Creator<SelfCreator<string>>)Creators.GetCreator(typeof(SelfCreator<string>));
      Assert.NotNull(creator);
      Assert.NotNull(creator.Create());
    }

    [Theory]
    [InlineData("unit_test")]
    [InlineData("unit_test_alt")]
    public void NamedCreator_TestN0(string name)
    {
      Creator<NoCreator> creator = (Creator<NoCreator>)Creators.GetCreator(typeof(NoCreator), name);
      Assert.Equal(name, creator.Name);
      Assert.NotNull(creator.Create());
    }

    [Theory]
    [InlineData("unit_test")]
    [InlineData("unit_test_alt")]
    public void NamedCreator_TestN1(string name)
    {
      Creator<int, NoCreator> creator = (Creator<int, NoCreator>)Creators.GetCreator(typeof(NoCreator), name, typeof(int));
      Assert.Equal(name, creator.Name);
      Assert.NotNull(creator.Create(0));
    }

    [Fact]
    public void ConstructorPreferenceTest()
    {
      Creator<NoCreator> creator = (Creator<NoCreator>)Creators.GetCreator(typeof(NoCreator));
      Assert.Equal(Creators.ConstructorName, creator.Name);
    }

    [Fact]
    public void NamedCreator_GenericIntTestN0()
    {
      Creator<NoCreator<int>> creator = (Creator<NoCreator<int>>)Creators.GetCreator(typeof(NoCreator<int>), false);
      Assert.Equal("unit_test", creator.Name);
      Assert.NotNull(creator.Create());
    }

    [Fact]
    public void NamedCreator_GenericIntTestN1()
    {
      Creator<int, NoCreator<int>> creator = (Creator<int, NoCreator<int>>)Creators.GetCreator(typeof(NoCreator<int>), false, typeof(int));
      Assert.Equal("unit_test", creator.Name);
      Assert.NotNull(creator.Create(0));
    }

    [Fact]
    public void NamedCreator_GenericStringTestN0()
    {
      Creator<NoCreator<string>> creator = (Creator<NoCreator<string>>)Creators.GetCreator(typeof(NoCreator<string>), false);
      Assert.Equal("unit_test", creator.Name);
      Assert.NotNull(creator.Create());
    }

    [Fact]
    public void NamedCreator_GenericStringTestN1()
    {
      Creator<int, NoCreator<string>> creator = (Creator<int, NoCreator<string>>)Creators.GetCreator(typeof(NoCreator<string>), false, typeof(int));
      Assert.Equal("unit_test", creator.Name);
      Assert.NotNull(creator.Create(0));
    }

    [Fact]
    public void ConstructorsAsCreators_TestN0()
    {
      Creator<ConstructorsCreators> creator = (Creator<ConstructorsCreators>)Creators.GetCreator(typeof(ConstructorsCreators), false);
      Assert.Equal(Creators.ConstructorName, creator.Name);
      Assert.NotNull(creator.Create());
    }

    [Fact]
    public void ConstructorsAsCreators_TestN1()
    {
      Creator<int, ConstructorsCreators> creator = (Creator<int, ConstructorsCreators>)Creators.GetCreator(typeof(ConstructorsCreators), false, typeof(int));
      Assert.Equal(Creators.ConstructorName, creator.Name);
      Assert.NotNull(creator.Create(0));
    }

  }
}
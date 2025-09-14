namespace PinionCore.Showcases.Protocol.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var protocol = Protocol.ProtocolCreater.Create();
            Assert.NotNull(protocol);
        }
    }
}
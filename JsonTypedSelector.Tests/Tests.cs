using JsonTypedSelector.Extensions;
using JsonTypedSelector.Model;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonTypedSelector.Tests
{
    [TestClass]
    public sealed class Tests
    {
        public static Person person;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            person = new Person()
            {
                FirstName = "Test",
                CardNumber = 12345,
                Addresses = new List<Address>()
                {
                    new Address() { City = "Gdansk", CountryCode = CountryCode.PL },
                    new Address() { City = "New York", CountryCode = CountryCode.USA },
                    new Address() { City = "London", CountryCode = CountryCode.GB }
                }
            };
        }

        [TestMethod]
        public void SelectPropertyWithoutJasonPropertyNameAttribute()
        {
            JsonObject jsonObj = JsonSerializer.SerializeToNode(person) as JsonObject;
            var expected = person.FirstName;
            var actual = jsonObj.SelectNode<Person>(p => p.FirstName).GetValue<string>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelectPropertyWithJasonPropertyNameAttribute()
        {
            JsonObject jsonObj = JsonSerializer.SerializeToNode(person) as JsonObject;
            var expected = person.CardNumber;
            var actual = jsonObj.SelectNode<Person>(p => p.CardNumber).GetValue<int>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelectUsingIndex()
        {
            JsonObject jsonObj = JsonSerializer.SerializeToNode(person) as JsonObject;
            var expected = person.Addresses[0];
            var actual = jsonObj.SelectNode<Person>(p => p.Addresses[0]).Deserialize<Address>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelectUsingIndexAsVariable()
        {
            JsonObject jsonObj = JsonSerializer.SerializeToNode(person) as JsonObject;
            var index = 1;
            var expected = person.Addresses[index];
            var actual = jsonObj.SelectNode<Person>(p => p.Addresses[index]).Deserialize<Address>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelectPropertyOfObjectFromArray()
        {
            JsonObject jsonObj = JsonSerializer.SerializeToNode(person) as JsonObject;
            var expected = person.Addresses[0].City;
            var actual = jsonObj.SelectNode<Person>(p => p.Addresses[0].City).GetValue<string>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NegativeSelectWithStringIndexThrowsException()
        {
            JsonObject jsonObj = JsonSerializer.SerializeToNode(person) as JsonObject;
            var expected = person.Addresses[0].City;
            var actual = jsonObj.SelectNode<Person>(p => p.Addresses[0].City).GetValue<string>();
            Assert.AreEqual(expected, actual);
        }
    }
}

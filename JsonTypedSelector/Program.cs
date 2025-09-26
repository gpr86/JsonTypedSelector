using JsonTypedSelector;
using JsonTypedSelector.Extensions;
using JsonTypedSelector.Model;
using System.Text.Json;
using System.Text.Json.Nodes;

// Example 1: Set
//  - country code of first address to "ABC"
//  - non nullable int card number to null.
// Please note that this is proposed solution for negative scenario when wee need incorrect payload.
// First create model object, serialize to JsonObject
// Then set required fields into values that can't be set using class object
// e.g. int as null or enum as string that is not definied in this enum

// The service that prints to the console
var service = new PrintPersonService();

var person = new Person()
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

var jsonObject = JsonSerializer.SerializeToNode(person) as JsonObject;
var jsonObject11 = jsonObject.DeepClone() as JsonObject;
var jsonObject12 = jsonObject.DeepClone() as JsonObject;

// 1.1 Normal way
jsonObject11["addresses"][0]["country_code"] = "ABC";
jsonObject11["card_number"] = null;

// 1.2 Proposed selecor
// we can build access path based on Linq Expression because we know the type

jsonObject12.SetNodeValue<Person>(p => p.Addresses[0].CountryCode, "ABC");
jsonObject12.SetNodeValue<Person>(p => p.CardNumber, null);

// print
service.PrintPerson(jsonObject11, "Example 1.1");
service.PrintPerson(jsonObject12, "Example 1.2");




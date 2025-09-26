using JsonTypedSelector.Model;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonTypedSelector
{
    public class PrintPersonService
    {
        public void PrintPerson(Person person, string message) =>
            PrintPerson(JsonSerializer.SerializeToNode(person) as JsonObject, message);

        public void PrintPerson(JsonObject obj, string message) =>
            Console.WriteLine($"{message}\n{obj.ToString()}\n\n");

    }
}

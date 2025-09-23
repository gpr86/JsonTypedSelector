using JsonTypedSelector.Model;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonTypedSelector
{
    public class DummyService
    {
        public void Send(Person person) => Send(JsonSerializer.SerializeToNode(person) as JsonObject);
        public void Send(JsonObject person) { }

    }
}

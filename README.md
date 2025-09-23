**The Problem:**
1. I want to keep all model's enums as c# enums
2.  I want to be able to pass incorrect or null value for such field for negative scnerios

**Solution:** Create additional clients method that accept JsonObject instead of model object.

3. The above solves problem but introduce new problems
4. The node selector will look like `jsonObject["Adressess"][0]["City"]`. A lot of hardcoded strings
5. Additionally if JsonPropertyAttribute is used then it will look like `jsonObject["adressess"][0]["country_code"]`.
6. Mybe point 4 can be solved with ExpandoObject but still we need to use Json property names.

**Solution:** Write simple selector for json.
- Use linq expression to select JsonNode from JsonObject based on typed selector written for model
- Take name from JsonPropertyNameAttribute if set
- Return always a JsonNode
- Accept only memberaccess and indexer[int index] (this is very simple solution)
- With the above JsonNode can be selected as `jsonObj.SelectNode<Person>(p => p.Addresses[0].City)`
that returns JsonNode that can be later modified.
- What can be misleading here is that generic parameter `<Person>` is not a type o returned object but the type of parameter p.
- Also I we want to use indexes then we have to use IList, List, Array in model. ICollection does not support index access. 

I wrote this selector and access tests for it. I won't deny that copilot wrote the very first version of the code to see how complex it would be. The idea is to use it for negative scenarios while keep models as clean as possible.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Script.Serialization;

public class JSONHelper
{
	public static string Render(JSONValue value)
	{
		StringBuilder sb = new StringBuilder();
		value.RenderValue(sb);
		return sb.ToString();
	}

	public static string EscapeString(string value)
	{
		return value;
	}

	public static void RenderDelimited<TValue>(IEnumerable<TValue> values, Action<TValue> RenderItem, Action SeparateItems)
	{
		bool first = true;
		foreach (TValue value in values)
		{
			if (first)
				first = false;
			else
				SeparateItems();

			RenderItem(value);
		}
	}

	public static T Deserialise<T>(string json)
	{
		JavaScriptSerializer js = new JavaScriptSerializer();
		T obj = js.Deserialize<T>(json);
		return obj;
	}

	public static string Serialize(Object obj)
	{
		JavaScriptSerializer js = new JavaScriptSerializer();
		string retVal = js.Serialize(obj);
		return retVal;
	}
}

public abstract class JSONValue
{
	public abstract void RenderValue(StringBuilder sb);

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		this.RenderValue(sb);
		return sb.ToString();
	}
}

public abstract class JSONValue<TValue> : JSONValue
{
	public TValue Value { get; protected set; }
}

public class JSONNull : JSONValue<object>
{
	private JSONNull() { Value = null; }

	public static readonly JSONNull Instance = new JSONNull();

	public override void RenderValue(StringBuilder sb)
	{
		sb.Append("null");
	}
}

public class JSONBoolean : JSONValue<bool>
{
	private JSONBoolean(bool value) { Value = value; }

	private static JSONBoolean FalseInstance = new JSONBoolean(false);
	private static JSONBoolean TrueInstance = new JSONBoolean(true);

	public static JSONBoolean Get(bool value)
	{
		return value ? TrueInstance : FalseInstance;
	}

	public override void RenderValue(StringBuilder sb)
	{
		sb.Append(Value.ToString().ToLower());
	}
}

public abstract class JSONNumber<TNum> : JSONValue<TNum>
	where TNum : struct
{
	public override void RenderValue(StringBuilder sb)
	{
		sb.Append(Value.ToString());
	}
}

public class JSONInteger : JSONNumber<int>
{
	public JSONInteger(int value) { Value = value; }
}

public class JSONDouble : JSONNumber<double>
{
	public JSONDouble(double value) { Value = value; }
}

public class JSONString : JSONValue<string>
{
	private const string DOUBLE_QUOTE = "\"";

	public JSONString(string value) { Value = value; }

	public override void RenderValue(StringBuilder sb)
	{
		sb.Append(DOUBLE_QUOTE);
		sb.Append(JSONHelper.EscapeString(Value));
		sb.Append(DOUBLE_QUOTE);
	}
}

public class JSONArray : JSONValue<List<JSONValue>>
{
	private const string ARRAY_OPEN = "[ ";
	private const string ARRAY_CLOSE = " ]";
	private const string MEMBER_SEPARATOR = ", ";

	public JSONArray() { Value = new List<JSONValue>(); }
	public JSONArray(List<JSONValue> members) { Value = members; }

	public override void RenderValue(StringBuilder sb)
	{
		sb.Append(ARRAY_OPEN);
		JSONHelper.RenderDelimited(
			Value,
			member => member.RenderValue(sb),
			() => sb.Append(MEMBER_SEPARATOR)
		);
		sb.Append(ARRAY_CLOSE);
	}

	public void AddMember(JSONValue member) { Value.Add(member); }
}

public class JSONObject : JSONValue<Dictionary<string, JSONValue>>
{
	private const string OBJECT_OPEN = "{ ";
	private const string OBJECT_CLOSE = " }";
	private const string DOUBLE_QUOTE = "\"";
	private const string PROPERTY_SEPARATOR = ", ";
	private const string PROPERTY_ID_SEPARATOR = " : ";

	public JSONObject() { Value = new Dictionary<string, JSONValue>(); }
	public JSONObject(Dictionary<string, JSONValue> properties) { Value = properties; }

	public override void RenderValue(StringBuilder sb)
	{
		sb.Append(OBJECT_OPEN);
		JSONHelper.RenderDelimited(
			Value,
			propertyPair => DisplayProperty(sb, propertyPair),
			() => sb.Append(PROPERTY_SEPARATOR)
		);
		sb.Append(OBJECT_CLOSE);
	}

	private void DisplayProperty(StringBuilder sb, KeyValuePair<string, JSONValue> propertyPair)
	{
		sb.AppendFormat("{0}{1}{0}", DOUBLE_QUOTE, propertyPair.Key);
		sb.Append(PROPERTY_ID_SEPARATOR);
		propertyPair.Value.RenderValue(sb);
	}

	public void AddProperty(string propertyID, JSONValue value)
	{
		Value.Add(propertyID, value);
	}
}

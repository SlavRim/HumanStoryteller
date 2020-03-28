using Newtonsoft.Json.Utilities;
using System;
using HumanStoryteller.NewtonsoftShell.Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Linq
{
	
	
	public class JTokenReader : JsonReader, IJsonLineInfo
	{
		private readonly JToken _root;

		
		private string _initialPath;

		
		private JToken _parent;

		
		private JToken _current;

		
		public JToken CurrentToken
		{
			
			get
			{
				return _current;
			}
		}

		int IJsonLineInfo.LineNumber
		{
			get
			{
				if (base.CurrentState == State.Start)
				{
					return 0;
				}
				return ((IJsonLineInfo)_current)?.LineNumber ?? 0;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				if (base.CurrentState == State.Start)
				{
					return 0;
				}
				return ((IJsonLineInfo)_current)?.LinePosition ?? 0;
			}
		}

		public override string Path
		{
			get
			{
				string text = base.Path;
				if (_initialPath == null)
				{
					_initialPath = _root.Path;
				}
				if (!StringUtils.IsNullOrEmpty(_initialPath))
				{
					if (StringUtils.IsNullOrEmpty(text))
					{
						return _initialPath;
					}
					text = ((!text.StartsWith('[')) ? (_initialPath + "." + text) : (_initialPath + text));
				}
				return text;
			}
		}

		public JTokenReader(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");
			_root = token;
		}

		public JTokenReader(JToken token, string initialPath)
			: this(token)
		{
			_initialPath = initialPath;
		}

		public override bool Read()
		{
			if (base.CurrentState != 0)
			{
				if (_current == null)
				{
					return false;
				}
				JContainer jContainer = _current as JContainer;
				if (jContainer != null && _parent != jContainer)
				{
					return ReadInto(jContainer);
				}
				return ReadOver(_current);
			}
			if (_current == _root)
			{
				return false;
			}
			_current = _root;
			SetToken(_current);
			return true;
		}

		private bool ReadOver(JToken t)
		{
			if (t == _root)
			{
				return ReadToEnd();
			}
			JToken next = t.Next;
			if (next == null || next == t || t == t.Parent.Last)
			{
				if (t.Parent == null)
				{
					return ReadToEnd();
				}
				return SetEnd(t.Parent);
			}
			_current = next;
			SetToken(_current);
			return true;
		}

		private bool ReadToEnd()
		{
			_current = null;
			SetToken(JsonToken.None);
			return false;
		}

		private JsonToken? GetEndToken(JContainer c)
		{
			switch (c.Type)
			{
			case JTokenType.Object:
				return JsonToken.EndObject;
			case JTokenType.Array:
				return JsonToken.EndArray;
			case JTokenType.Constructor:
				return JsonToken.EndConstructor;
			case JTokenType.Property:
				return null;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
			}
		}

		private bool ReadInto(JContainer c)
		{
			JToken first = c.First;
			if (first == null)
			{
				return SetEnd(c);
			}
			SetToken(first);
			_current = first;
			_parent = c;
			return true;
		}

		private bool SetEnd(JContainer c)
		{
			JsonToken? endToken = GetEndToken(c);
			if (endToken.HasValue)
			{
				SetToken(endToken.GetValueOrDefault());
				_current = c;
				_parent = c;
				return true;
			}
			return ReadOver(c);
		}

		private void SetToken(JToken token)
		{
			switch (token.Type)
			{
			case JTokenType.Object:
				SetToken(JsonToken.StartObject);
				break;
			case JTokenType.Array:
				SetToken(JsonToken.StartArray);
				break;
			case JTokenType.Constructor:
				SetToken(JsonToken.StartConstructor, ((JConstructor)token).Name);
				break;
			case JTokenType.Property:
				SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
				break;
			case JTokenType.Comment:
				SetToken(JsonToken.Comment, ((JValue)token).Value);
				break;
			case JTokenType.Integer:
				SetToken(JsonToken.Integer, ((JValue)token).Value);
				break;
			case JTokenType.Float:
				SetToken(JsonToken.Float, ((JValue)token).Value);
				break;
			case JTokenType.String:
				SetToken(JsonToken.String, ((JValue)token).Value);
				break;
			case JTokenType.Boolean:
				SetToken(JsonToken.Boolean, ((JValue)token).Value);
				break;
			case JTokenType.Null:
				SetToken(JsonToken.Null, ((JValue)token).Value);
				break;
			case JTokenType.Undefined:
				SetToken(JsonToken.Undefined, ((JValue)token).Value);
				break;
			case JTokenType.Date:
			{
				object obj = ((JValue)token).Value;
				if (obj is DateTime)
				{
					DateTime value2 = (DateTime)obj;
					obj = DateTimeUtils.EnsureDateTime(value2, base.DateTimeZoneHandling);
				}
				SetToken(JsonToken.Date, obj);
				break;
			}
			case JTokenType.Raw:
				SetToken(JsonToken.Raw, ((JValue)token).Value);
				break;
			case JTokenType.Bytes:
				SetToken(JsonToken.Bytes, ((JValue)token).Value);
				break;
			case JTokenType.Guid:
				SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
				break;
			case JTokenType.Uri:
			{
				object value = ((JValue)token).Value;
				Uri uri = value as Uri;
				SetToken(JsonToken.String, ((object)uri != null) ? uri.OriginalString : SafeToString(value));
				break;
			}
			case JTokenType.TimeSpan:
				SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
				break;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
			}
		}

		
		private string SafeToString(object value)
		{
			return value?.ToString();
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			if (base.CurrentState == State.Start)
			{
				return false;
			}
			return ((IJsonLineInfo)_current)?.HasLineInfo() ?? false;
		}
	}
}

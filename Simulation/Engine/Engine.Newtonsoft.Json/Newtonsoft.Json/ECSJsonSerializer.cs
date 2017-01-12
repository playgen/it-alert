using System;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Engine.Serialization.Newtonsoft.Json
{
	internal class ECSJsonSerializer : JsonSerializer
	{
		internal override object DeserializeInternal(JsonReader reader, Type objectType)
		{
			// skip internal to use custom reader
			//return base.DeserializeInternal(reader, objectType);

			ValidationUtils.ArgumentNotNull(reader, nameof(reader));

			// set serialization options onto reader
			CultureInfo previousCulture;
			DateTimeZoneHandling? previousDateTimeZoneHandling;
			DateParseHandling? previousDateParseHandling;
			FloatParseHandling? previousFloatParseHandling;
			int? previousMaxDepth;
			string previousDateFormatString;
			// TODO: reimplement
			SetupReader(reader, out previousCulture, out previousDateTimeZoneHandling, out previousDateParseHandling, out previousFloatParseHandling, out previousMaxDepth, out previousDateFormatString);

			var traceJsonReader = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
				? new TraceJsonReader(reader)
				: null;

			var serializerReader = new ECSJsonReader(this);
			var value = serializerReader.Deserialize(traceJsonReader ?? reader, objectType, CheckAdditionalContent);

			if (traceJsonReader != null)
			{
				TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), null);
			}

			ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);

			return value;
		}
	}
}

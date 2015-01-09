using System;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace ManUnmanInterop
{
	internal interface ISettingsProvider
	{
		string Get(string key);

		T Get<T>(string key);
	}

	class AppSettingsProvider : ISettingsProvider
	{
		public string Get(string key) {
			Contract.Ensures(!String.IsNullOrEmpty(key));
			Contract.Ensures(Contract.Result<String>() != null);

			return ConfigurationManager.AppSettings[key];
		}

		public T Get<T>(string key) {
			String value = Get(key);

			return ConvertFromString<T>(value);
		}

		private T ConvertFromString<T>(String value) {
			var resType = typeof (T);
			T result = default(T);
			var converted = false;
			try {
				if (resType.IsPrimitive) {
					result = (T) Convert.ChangeType(value, resType);
					converted = true;
				} else if (resType.IsEnum) {
					result = (T) Enum.Parse(resType, value);
					converted = true;
				}
			} catch (InvalidCastException) {
				converted = false;
			}

			if (converted) {
				return result;
			}

			dynamic dynValue = value;
			return (T) dynValue;
		}
	}
}
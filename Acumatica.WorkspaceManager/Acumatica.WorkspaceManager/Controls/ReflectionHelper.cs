using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ConfigCore
{
	public static class ReflectionHelper
	{
		public static IEnumerable<T> GetFieldValuesOfType<T>(Object source) {
			return source.GetType()
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(fi => fi.FieldType == typeof (T) || fi.FieldType.IsSubclassOf(typeof (T)))
					.Select(fi => (T)fi.GetValue(source));
		}

		public static Type GetSingleChildType(Type parentType, Assembly assembly = null) {
			List<Type> res = GetChildTypes(parentType, assembly);
			if (res.Count > 1) throw new Exception(String.Format("Assembly contains multiple child classes for '{0}'.", parentType.Name));
			return res.Count == 1 ? res[0] : null;
		}

		public static List<Type> GetChildTypes(Type parentType, Assembly assembly = null) {
			if (null == assembly)
				assembly = Assembly.GetExecutingAssembly();

			return assembly.GetTypes().Where(t => t.IsClass && parentType.IsAssignableFrom(t) && !t.IsAbstract && t != parentType).ToList();
		}

		public static T CreateInstance<T>(Type t = null) {
			if (t == null) t = typeof(T);

			if (t.IsAbstract)
				throw new Exception(String.Format("Cant create instance of abstract type '{0}'.", typeof(T).Name));

			ConstructorInfo ci = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
			return (T)ci.Invoke(new object[] { });
		}

	}
}

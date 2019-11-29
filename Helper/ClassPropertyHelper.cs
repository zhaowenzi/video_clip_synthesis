using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace 素材合成.Helper
{
	/// <summary>
	/// 属性操作的类
	/// </summary>
	public class ClassPropertyHelper {
		/// <summary>
		/// 获取属性名称和值  
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetPropertyNameAndValue<T>(T obj) {
			if (obj != null) {
				Dictionary<string, string> propertyValue = new Dictionary<string, string>();
				Type type = obj.GetType();
				PropertyInfo[] propertyInfos = type.GetProperties();

				foreach (PropertyInfo item in propertyInfos) {
					propertyValue.Add(item.Name, (item.GetValue(obj, null) == null ? "" : item.GetValue(obj, null)).ToString());
				}

				return propertyValue;
			}
			return null;
		}
		/// <summary>
		/// 获取属性名称和类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetPropertyNameAndType<T>(T obj) {
			if (obj != null) {
				Dictionary<string, string> propertyValue = new Dictionary<string, string>();
				Type type = obj.GetType();
				PropertyInfo[] propertyInfos = type.GetProperties();

				foreach (PropertyInfo item in propertyInfos) {
					propertyValue.Add(item.Name, item.PropertyType.Name);
				}

				return propertyValue;
			}
			return null;
		}
		/// <summary>
		/// 获取类名
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetClassName<T>(T obj) {
			if (obj != null) {
				Type type = obj.GetType();
				return type.Name;
			}
			return null;
		}  
	}
}

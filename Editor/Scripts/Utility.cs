using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    public static class Utility
    {

        private readonly static Dictionary<MemberInfo, object[]> m_MemberAttributeLookup;
        private static readonly Dictionary<Type, FieldInfo[]> m_SerializedFieldInfoLookup;
        private static readonly Dictionary<Type, bool> m_CustomPropertyDrawerLookup;

        static Utility()
        {
            m_SerializedFieldInfoLookup = new Dictionary<Type, FieldInfo[]>();
            m_MemberAttributeLookup = new Dictionary<MemberInfo, object[]>();
            m_CustomPropertyDrawerLookup = new Dictionary<Type, bool>();
        }

        public static Type GetElementType(Type type)
        {
            Type[] interfaces = type.GetInterfaces();

            return (from i in interfaces
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select i.GetGenericArguments()[0]).FirstOrDefault();
        }

        public static bool HasCustomPropertyDrawer(Type type)
        {
            if (m_CustomPropertyDrawerLookup.ContainsKey(type)) {
                return m_CustomPropertyDrawerLookup[type];
            }

            foreach (Type typesDerivedFrom in TypeCache.GetTypesDerivedFrom<GUIDrawer>())
            {
                object[] customAttributes = typesDerivedFrom.GetCustomAttributes<CustomPropertyDrawer>();
                for (int i = 0; i < (int)customAttributes.Length; i++)
                {
                    CustomPropertyDrawer customPropertyDrawer = (CustomPropertyDrawer)customAttributes[i];

                    FieldInfo field = customPropertyDrawer.GetType().GetField("m_Type", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Type type1 = (Type)field.GetValue(customPropertyDrawer);
                    if (type == type1)
                    {
                        m_CustomPropertyDrawerLookup.Add(type, true);
                        return true;
                    }
                }
            }
            m_CustomPropertyDrawerLookup.Add(type, false);
            return false;
        }

        public static IEnumerable<SerializedProperty> EnumerateChildProperties(this SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            if (iterator.NextVisible(enterChildren: true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        yield break;

                    yield return iterator;
                }
                while (iterator.NextVisible(enterChildren: false));
            }
        }

        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo)
		{
			object[] objArray = GetCustomAttributes(memberInfo, true);
			List<T> list = new List<T>();
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				if (objArray[i].GetType() == typeof(T) || objArray[i].GetType().IsSubclassOf(typeof(T)))
				{
					list.Add((T)objArray[i]);
				}
			}
			return list.ToArray();
		}

        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
        {
            object[] objArray = GetCustomAttributes(memberInfo, true);
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(T) || objArray[i].GetType().IsSubclassOf(typeof(T)))
                {
                    return (T)objArray[i];
                }
            }
            return default(T);
        }

        public static object[] GetCustomAttributes(MemberInfo memberInfo, bool inherit)
        {
            if (!m_MemberAttributeLookup.TryGetValue(memberInfo, out object[] customAttributes))
            {
                customAttributes = memberInfo.GetCustomAttributes(inherit);
                m_MemberAttributeLookup.Add(memberInfo, customAttributes);
            }
            return customAttributes;
        }

        public static FieldInfo GetSerializedField(this Type type, string name)
        {
            return type.GetAllSerializedFields().Where(x => x.Name == name).FirstOrDefault();
        }

        public static FieldInfo[] GetAllSerializedFields(this Type type)
        {
            if (type == null)
            {
                return new FieldInfo[0];
            }
            FieldInfo[] fields = GetSerializedFields(type).Concat(GetAllSerializedFields(type.BaseType)).ToArray();
            fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
            return fields;
        }

        public static FieldInfo[] GetSerializedFields(this Type type)
        {
            if (!m_SerializedFieldInfoLookup.TryGetValue(type, out FieldInfo[] fields))
            {
                fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.IsPublic && !x.HasAttribute(typeof(NonSerializedAttribute)) || x.HasAttribute(typeof(SerializeField)) || x.HasAttribute(typeof(SerializeReference))).ToArray();
                fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
                m_SerializedFieldInfoLookup.Add(type, fields);
            }
            return fields;
        }

        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

        public static bool HasAttribute<T>(this MemberInfo memberInfo)
        {
            return memberInfo.HasAttribute(typeof(T));
        }

        public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            object[] objArray = GetCustomAttributes(memberInfo, true);
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i].GetType() == attributeType || objArray[i].GetType().IsSubclassOf(attributeType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
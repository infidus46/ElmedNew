using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Yamed.Core
{
    class DynamicFactory
    {
        private string[] _propertyNames;
        private Type[] _propertyTypes;
        private string _codeString;

        public string NamespaceName { get; private set; }
        public string ClassName { get; private set; }
        public string FullyQualifiedName { get { return string.Format("{0}.{1}", NamespaceName, ClassName); } }

        public DynamicFactory(string namespaceName, string className, string[] propertyNames, Type[] propertyTypes)
        {
            NamespaceName = namespaceName;
            ClassName = className;
            _propertyNames = propertyNames;
            _propertyTypes = propertyTypes;
            _codeString = CodeGenerationHelper.GetCodeString(NamespaceName, ClassName, _propertyNames, _propertyTypes);
        }

        public void AddPropertyChangedEventHandler(object objectInstance, PropertyChangedEventHandler handler)
        {
            EventInfo eventInfo = (from eventHandler in objectInstance.GetType().GetEvents()
                                   where eventHandler.EventHandlerType == typeof(PropertyChangedEventHandler)
                                   select eventHandler).First();
            eventInfo.AddEventHandler(objectInstance, handler);
        }

        public object Create()
        {
            return CodeGenerationHelper.InstantiateClass(_codeString, FullyQualifiedName);
        }

        public object GetProperty(object objectInstance, string propertyName)
        {
            PropertyInfo propertyInfo = FindPropertyInfo(objectInstance, propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' does not exist.", propertyName));
            }

            return propertyInfo.GetValue(objectInstance, null);
        }

        public string[] GetPropertyNames()
        {
            return (string[])_propertyNames.Clone();
        }

        public Type[] GetPropertyTypes()
        {
            return (Type[])_propertyTypes.Clone();
        }

        public PropertyInfo FindPropertyInfo(object objectInstance, string propertyName)
        {
            return (from propertyInfo in objectInstance.GetType().GetProperties()
                    where propertyInfo.Name == propertyName
                    select propertyInfo).First();
        }

        public void RemovePropertyChangedEventHandler(object objectInstance, PropertyChangedEventHandler handler)
        {
            EventInfo eventInfo = (from eventHandler in objectInstance.GetType().GetEvents()
                                   where eventHandler.EventHandlerType == typeof(PropertyChangedEventHandler)
                                   select eventHandler).First();
            eventInfo.RemoveEventHandler(objectInstance, handler);
        }

        public void SetProperty(object objectInstance, string propertyName, object propertyValue)
        {
            PropertyInfo propertyInfo = FindPropertyInfo(objectInstance, propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' does not exist.", propertyName));
            }

            propertyInfo.SetValue(objectInstance, propertyValue, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SeriesUI.Configuration
{
    [ConfigurationCollection(typeof(SeriesConfigElement), AddItemName = "Series",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class SeriesConfigCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Required to do series[0]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SeriesConfigElement this[int index]
        {
            get => (SeriesConfigElement)BaseGet(index);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SeriesConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SeriesConfigElement) element).Name ?? throw new InvalidOperationException();
        }

        public IEnumerable<SeriesConfigElement> GetEnumerable()
        {
            var count = Count;
            for (var i = 0; i < count; i++) yield return BaseGet(i) as SeriesConfigElement;
        }
    }
}
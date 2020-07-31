using System;
using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class ComponentBreadCrumbsModel
    {
        public Element[] Elements;

        public class Element
        {
            public Guid Id;

            public string Name;
        }

        public static ComponentBreadCrumbsModel Create(Guid componentId, IStorage storage)
        {
            var elements = new List<Element>();
            Guid? id = componentId;
            do
            {
                var component = storage.Components.GetOneById(id.Value);
                elements.Add(new Element()
                {
                    Id = component.Id,
                    Name = component.DisplayName
                });
                id = component.ParentId;
            } while (id != null);
            elements.Reverse();

            return new ComponentBreadCrumbsModel()
            {
                Elements = elements.ToArray()
            };
        }
    }
}
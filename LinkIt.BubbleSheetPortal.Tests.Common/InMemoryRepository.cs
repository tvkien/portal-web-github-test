using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Tests.Common
{
    public class InMemoryRepository<T> : IRepository<T> where T : class, IIdentifiable
    {
        protected readonly static List<T> data = new List<T>();

        public IQueryable<T> Select()
        {
            return data.Select(CreateCopy).ToList().AsQueryable();
        }

        public virtual void Save(T item)
        {
            T oldItem = data.FirstOrDefault(IdMatches(item));
            if (!oldItem.IsNull())
            {
                data.Remove(oldItem);
            }
            else
            {
                item.Id = GenerateNextID();
            }
            var copy = CreateCopy(item);
            CheckValidCopy(item, copy);
            data.Add(copy);
        }

        public virtual void Delete(T item)
        {
            T oldItem = data.FirstOrDefault(IdMatches(item));
            if (!oldItem.IsNull())
            {
                data.Remove(oldItem);
            }
        }

        public int GenerateNextID()
        {
            if (Select().Any())
            {
                return Select().Max(x => x.Id) + 1;
            }
            return 1;
        }

        protected void CheckValidCopy(T item, T copy)
        {
            if (ReferenceEquals(copy, item))
            {
                var type = typeof(T);
                var itemType = item.GetType();
                var thisType = this.GetType();
                throw new AutoMapperMappingException(string.Format("Type: '{0}' in {1} was not mapped to '{2}' and thus not copied correctly.", type.FullName, thisType.FullName, itemType.FullName));
            }
        }

        protected T CreateCopy(T x)
        {
            return (T)Mapper.Map(x, x.GetType(), x.GetType());
        }

        private Func<T, bool> IdMatches(T item)
        {
            return x => x.Id.Equals(item.Id);
        }
    }
}

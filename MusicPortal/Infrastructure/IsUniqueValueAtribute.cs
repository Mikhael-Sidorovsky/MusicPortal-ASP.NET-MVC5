using MusicPortal.Models.Repository;
using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Infrastructure
{
    public class IsUniqueValueAttribute : ValidationAttribute
    {
        IRepository repository;
        public IsUniqueValueAttribute(){ repository = new MusicPortalRepository(); }
        public IsUniqueValueAttribute(IRepository repository)
        {
            this.repository = repository;
        }
        public override bool IsValid(object value)
        {
            bool result = repository.IsUniqueUserValue(value.ToString());
            return result;
        }
    }
}
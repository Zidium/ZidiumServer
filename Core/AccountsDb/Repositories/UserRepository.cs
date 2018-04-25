using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public class UserRepository : AccountBasedRepository<User>, IUserRepository
    {
        public UserRepository(AccountDbContext context) : base(context) { }

        public User Add(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
                user.CreateDate = DateTime.Now;
            }
            Context.Users.Add(user);
            Context.SaveChanges();
            return user;
        }

        public User GetById(Guid id)
        {
            var result = Context.Users.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.User);
            return result;
        }

        public User GetByIdOrNull(Guid id)
        {
            return Context.Users.Find(id);
        }

        public User Update(User entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(User entity)
        {
            entity.InArchive = true;
            Update(entity);
        }

        public User GetOneOrNullByLogin(string login)
        {
            return QueryAll().SingleOrDefault(x => x.Login == login);
        }

        public IQueryable<User> QueryAll()
        {
            return Context.Users.Where(x => x.InArchive == false);
        }

        public void Remove(Guid userId)
        {
            var user = GetById(userId);
            Remove(user);
        }

        public UserContact GetContactById(Guid id)
        {
            var result = Context.UserContacts.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.UserContact);

            return result;
        }

        public UserContact AddContactToUser(Guid userId, UserContactType type, string value, DateTime createDate)
        {
            var contact = new UserContact()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Value = value,
                CreateDate = createDate
            };
            Context.UserContacts.Add(contact);
            Context.SaveChanges();
            return contact;
        }

        public UserContact EditContactById(Guid id, UserContactType type, string value)
        {
            var contact = GetContactById(id);
            if (contact != null)
            {
                contact.Type = type;
                contact.Value = value;
                Context.SaveChanges();
                return contact;
            }
            return null;
        }

        public void DeleteContactById(Guid id)
        {
            var contact = GetContactById(id);
            if (contact != null)
            {
                Context.UserContacts.Remove(contact);
                Context.SaveChanges();
            }
        }

        public User GetAccountAdmin()
        {
            return QueryAll().First(x => x.Roles.Any(y => y.RoleId == RoleId.AccountAdministrators));
        }

    }

}

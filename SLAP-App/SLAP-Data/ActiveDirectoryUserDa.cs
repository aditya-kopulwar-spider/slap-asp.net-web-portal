using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
  public class ActiveDirectoryUserDa
    {
        private slap_dbEntities _slapDbEntities;

        public ActiveDirectoryUserDa()
        {
            this._slapDbEntities=new slap_dbEntities();
        }

        public List<ActiveDirectoryUser> GetActiveDirectoryUsers()
        {
            return _slapDbEntities.ActiveDirectoryUsers.ToList();
        }
        public ActiveDirectoryUser GetActiveDirectoryUser(int activeDirectoryUserId)
        {
            return _slapDbEntities.ActiveDirectoryUsers.Find(activeDirectoryUserId);
        }
        public ActiveDirectoryUser GetActiveDirectoryUserByEmailId(string userPrincipalName)
        {
            return _slapDbEntities.ActiveDirectoryUsers.First(p=>p.userPrincipalName==userPrincipalName);
        }
        public ActiveDirectoryUser EditActiveDirectoryUser(ActiveDirectoryUser activeDirectoryUser)
        {
            _slapDbEntities.Entry(activeDirectoryUser).State = EntityState.Modified;
            _slapDbEntities.SaveChanges();
            return activeDirectoryUser;
        }

        public bool AddActiveDirectoryUsers(List<ActiveDirectoryUser> activeDirectoryUsers)
        {
            try
            {
                Dictionary<Guid,ActiveDirectoryUser> mapActiveDirectoryUsers=new Dictionary<Guid, ActiveDirectoryUser>();
                mapActiveDirectoryUsers= activeDirectoryUsers.ToDictionary(key => key.Id, value => value.ActiveDirectoryUser2);
                activeDirectoryUsers.ForEach(p=>p.ActiveDirectoryUser2=null);
                _slapDbEntities.ActiveDirectoryUsers.AddRange(activeDirectoryUsers);
                _slapDbEntities.SaveChanges();

                var directoryUsers = GetActiveDirectoryUsers();
                foreach (var directoryUser in directoryUsers)
                {
                    var activeDirectoryUser2 = mapActiveDirectoryUsers[directoryUser.Id];
                    if (activeDirectoryUser2 == null) continue;
                    {
                        directoryUser.ManagerId = directoryUsers.First(p => p.Id == activeDirectoryUser2.Id)
                            .ActiveDirectoryUserId;
                        EditActiveDirectoryUser(directoryUser);
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        public bool RemoveAllActiveDirectoryUsers()
        {
            _slapDbEntities.ActiveDirectoryUsers.RemoveRange(_slapDbEntities.ActiveDirectoryUsers.ToList());
            _slapDbEntities.SaveChanges();
            return true;
        }
    }
}

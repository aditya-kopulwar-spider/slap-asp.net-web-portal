using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
    public class UserRolesDA
    {
        private const string PC = "PC";
        private slap_dbEntities _dbEntities;
        private List<UserRole> _userRoleMapping = null;
        private List<Role> _roles;

        public UserRolesDA()
        {
            _dbEntities = new slap_dbEntities();
            _roles = _dbEntities.Roles.ToList();
        }

        public bool IsAdmin()
        {
            return true;
        }

        public bool IsUserPC(Guid userId)
        {
            List<UserRole> userRoles = GetAllUserRoles();
            return userRoles.Any(x => x.UserId == userId && x.Role.RoleName == PC);
        }

        public List<UserRole> GetAllUserRoles(bool refresh = false)
        {
            if (_userRoleMapping == null || refresh)
            {
                return _dbEntities.UserRoles.ToList();
            }
            else
                return _userRoleMapping;
        }

        public bool MakeUserPC(Guid userId)
        {
            int pcRoleId = _roles.First(x => x.RoleName == PC).RoleId;

            UserRole newPC = new UserRole
                { UserId = userId, RoleId = pcRoleId };

            _dbEntities.UserRoles.Add(newPC);
            _dbEntities.SaveChanges();
            GetAllUserRoles(true);
            return true;
        }

        public bool RemoveUserFromPCRole(Guid userId)
        {
            int pcRoleId = _roles.First(x => x.RoleName == PC).RoleId;
            UserRole pcUser = _dbEntities.UserRoles.Find(pcRoleId, userId);
            _dbEntities.UserRoles.Remove(pcUser);
            _dbEntities.SaveChanges();
            //removing entries from pcAssociate table having given userId as PC
            new PCAssociatesDA().RemoveAllAssociatesForGivenPC(userId);
            GetAllUserRoles(true);
            return true;
        }
    }
}

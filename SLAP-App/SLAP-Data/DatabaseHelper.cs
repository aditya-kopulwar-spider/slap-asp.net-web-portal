﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
    public class DatabaseHelper
    {
        private const string PC = "PC";
        private slap_dbEntities db = new slap_dbEntities();
        private List<UserRole> _userRoleMapping = null;
        private List<Role> _roles = null;


        public DatabaseHelper()
        {
            _roles = db.Roles.ToList();
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
                return db.UserRoles.ToList();
            }
            else
                return _userRoleMapping;
        }

        public bool MakeUserPC(Guid userId)
        {
            int pcRoleId = _roles.First(x => x.RoleName == PC).RoleId;

            UserRole newPC = new UserRole
            { UserId = userId, RoleId = pcRoleId };

            db.UserRoles.Add(newPC);
            db.SaveChanges();

            GetAllUserRoles(true);

            return true;
        }

        public bool RemoveUserFromPCRole(Guid userId)
        {
            int pcRoleId = _roles.First(x => x.RoleName == PC).RoleId;

            UserRole pcUser = db.UserRoles.Find(pcRoleId, userId);
            db.UserRoles.Remove(pcUser);
            db.SaveChanges();

            GetAllUserRoles(true);

            return true;
        }
        
    }
}

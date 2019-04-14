using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tangy.Data;
using Tangy.Models.AccountModels;

namespace Tangy.Extensions
{
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Extension method that takes in an updated string for the user's First Name and saves it with the given DbContext
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="firstName"></param>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool UpdateUserFirstName(this UserManager<AppUser> manager,
            string firstName,
            ApplicationDbContext db,
            string id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            if (user.FirstName != firstName)
            {
                user.FirstName = firstName;
                return db.SaveChanges() == 1;
            }
            else return true;
        }

        /// <summary>
        /// Extension method that takes in an updated string for the user's First Name and saves it with the given DbContext
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="lastName"></param>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool UpdateUserLastName(this UserManager<AppUser> manager,
            string lastName,
            ApplicationDbContext db,
            string id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            if (user.LastName != lastName)
            {
                user.LastName = lastName;
                return db.SaveChanges() == 1;
            }
            else return true;
        }
    }
}

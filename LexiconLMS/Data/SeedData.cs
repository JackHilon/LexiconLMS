using LexiconLMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Data
{
    public static class SeedData
    {
        internal static async Task InitializeAsync(IServiceProvider services, List<string> users)
        {
            var options = services.GetRequiredService<DbContextOptions<ApplicationDbContext>>();

            using(var context = new ApplicationDbContext(options))
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                //Create Roles ("Teacher" and "Student")
                var rolenames = new[] { "Teacher", "Student" };

                foreach (var name in rolenames)
                {
                    if (await roleManager.RoleExistsAsync(name)) continue;

                    var role = new IdentityRole { Name = name };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                //Create Users and Setting Roles
                var teacherEmails = new[] { "Teacher1@lms.se", "Teacher2@lms.se", "Teacher3@lms.se", "Teacher4@lms.se" };
                var studentEmails = new[] { "Student1@lms.se", "Student2@lms.se", "Student3@lms.se", "Student4@lms.se", "Student5@lms.se", "Student6@lms.se", "Student7@lms.se", "Student8@lms.se", "Student9@lms.se", "Student10@lms.se" };
                var teacherCount = 0;
                var studentCount = 4;

                foreach (var email in teacherEmails)
                {
                     var foundUser = await userManager.FindByEmailAsync(email);

                    if (foundUser != null)
                    {
                        teacherCount++;
                        continue;
                    }

                    var user = new ApplicationUser { UserName = email, Email = email };
                    var userResult = await userManager.CreateAsync(user, users[teacherCount]);
                    teacherCount++;

                    if (!userResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", userResult.Errors));
                    }

                    if (await userManager.IsInRoleAsync(user, rolenames[0])) continue;

                    var addTeacherRole = await userManager.AddToRoleAsync(user, rolenames[0]);

                    if (!addTeacherRole.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addTeacherRole.Errors));
                    }

                }

                foreach (var email in studentEmails)
                {

                    var foundUser = await userManager.FindByEmailAsync(email);

                    if (foundUser != null)
                    {
                        studentCount++;
                        continue;
                    }

                    var user = new ApplicationUser { UserName = email, Email = email };
                    var userResult = await userManager.CreateAsync(user, users[studentCount]);
                    studentCount++;

                    if (!userResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", userResult.Errors));
                    }

                    if (await userManager.IsInRoleAsync(user, rolenames[1])) continue;

                    var addStudentRole = await userManager.AddToRoleAsync(user, rolenames[1]);

                    if (!addStudentRole.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addStudentRole.Errors));
                    }

                }

                context.SaveChanges();
            }
        }
    }
}

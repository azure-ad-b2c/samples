using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace TaskService.Controllers
{
    [Authorize]
    public class TasksController : ApiController
    {
        // In this service we're using an in-memory list to store tasks, just to keep things simple.
        // All of your tasks will be lost each time you run the service
        private static List<Models.Task> db = new List<Models.Task>();
        private static int taskId;

        // OWIN auth middleware constants -> These claims must match what's in your JWT, like for like. Click the 'claims' tab to check.
        public const string scopeElement = "http://schemas.microsoft.com/identity/claims/scope";
        public const string objectIdElement = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        // API Scopes
        public static string ReadPermission = ConfigurationManager.AppSettings["api:ReadScope"];
        public static string WritePermission = ConfigurationManager.AppSettings["api:WriteScope"];

        /*
         * GET all tasks for user
         */
        public IEnumerable<Models.Task> Get()
        {
            HasRequiredScopes(ReadPermission);

            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);

            IEnumerable<Models.Task> userTasks = db.Where(t => t.Owner == owner);
            return userTasks;
        }

        /*
        * POST a new task for user
        */
        public void Post(Models.Task task)
        {
            HasRequiredScopes(WritePermission);

            if (String.IsNullOrEmpty(task.Text))
                throw new WebException("Please provide a task description");

            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);

            task.Id = taskId++;
            task.Owner = owner;
            task.Completed = false;
            task.DateModified = DateTime.UtcNow;
            db.Add(task);
        }

        /*
         * DELETE a task for user
         */
        public void Delete(int id)
        {
            HasRequiredScopes(WritePermission);

            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);

            Models.Task task = db.Where(t => t.Owner.Equals(owner) && t.Id.Equals(id)).FirstOrDefault();
            db.Remove(task);
        }

        /*
         * Check user claims match task details
         */
        private string CheckClaimMatch(string claim)
        {
            try
            {
                return ClaimsPrincipal.Current.FindFirst(claim).Value;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = $"Unable to match claim '{claim}' against user claims; click the 'claims' tab to double-check. {e.Message}"
                });                
            }
        }

        // Validate to ensure the necessary scopes are present.
        private void HasRequiredScopes(String permission)
        {
            if (!ClaimsPrincipal.Current.FindFirst(scopeElement).Value.Contains(permission))
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = $"The Scope claim does not contain the {permission} permission."
                });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DugoutDigits.Utilities;
using DugoutDigits.Objects;
using System.Web.Security;
using DugoutDigits.Models;

namespace DugoutDigits.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Person user = dba.GetPersonInformation(User.Identity.Name);

                ViewBag.FirstName = user.firstName;
                ViewBag.LastName = user.lastName;
                ViewBag.ImageURL = user.imageURL;
                //List<string> teams = dba.GetTeamListCoach(User.Identity.Name);
                //ViewBag.TeamNames = teams;
                //ViewBag.TeamCount = teams.Count;
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult AccountSummary() {
            // Get the user information from the database
            DBAccessor dba = new DBAccessor();
            Person user = dba.GetPersonInformation(User.Identity.Name);

            String summaryMessage = "<h3>Account Summary</h3>";
            summaryMessage += "<p>" + user.firstName + " " + user.lastName + "</p>";
            summaryMessage += "<img src='" + user.imageURL + "' alt='profile image' />";
            summaryMessage += "</div>";

            List<Team> teamNames = dba.GetTeamListCoach(User.Identity.Name);
            summaryMessage += "<div id='sidebar-teamlist'><ul>";
            foreach (Team team in teamNames) {
                summaryMessage += "<li>" + team.name + "</li>";
            }
            summaryMessage += "</ul></div>";


            return Json(
                new { message = summaryMessage },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult TeamList() {
            DBAccessor dba = new DBAccessor();
            List<Team> teamNames = dba.GetTeamListCoach(User.Identity.Name);
            
            String summaryMessage = "<ul>";
            foreach (Team team in teamNames) {
                summaryMessage += "<li>" + team.name + "</li>";
            }
            summaryMessage += "</ul>";

            return Json(
                new { message = summaryMessage },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult TestAJAX() {
            // TODO: do something with the arguments

            return Json(
                new { message = "Thanks for sending info" },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}

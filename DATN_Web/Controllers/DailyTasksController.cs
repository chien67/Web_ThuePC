using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Filters;

namespace DATN_Web.Controllers
{
    public class DailyTasksController : Controller
    {
        private readonly DailyTaskBLL _dailyTaskbll;

        // GET: Customer
        public DailyTasksController(DailyTaskBLL dailyTaskbll)
        {
            _dailyTaskbll = dailyTaskbll;
        }
        public ActionResult Index(DateTime? date)
        {
            var d = (date ?? DateTime.Today).Date;
            var list = _dailyTaskbll.GetDailyTasks(d);
            ViewBag.Date = d;
            return View(list);
        }

        // Manager thêm task
        [ManagerOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string title, string description, DateTime dueDate, byte priority = 2)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            _dailyTaskbll.AddManualTask(title, description, dueDate, priority, userId);
            return RedirectToAction("Index", new { date = dueDate.ToString("yyyy-MM-dd") });
        }

        // (tuỳ) tick done cho manual
        [ManagerOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Done(int taskId, bool isDone, DateTime date)
        {
            _dailyTaskbll.SetDone(taskId, isDone);
            return RedirectToAction("Index", new { date = date.ToString("yyyy-MM-dd") });
        }
    }
}
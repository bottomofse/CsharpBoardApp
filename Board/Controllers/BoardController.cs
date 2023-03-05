using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Board.Models;

namespace Board.Controllers
{
    public class BoardController : Controller
    {
        private BoardDbContext db_;

        public BoardController(): this(null)
        {
        }

        public BoardController(BoardDbContext db)
        {
            db_ = db ?? new BoardDbContext();
        }

        // GET: Board
        public ActionResult Index()
        {
            var model = new BoardListModel(db_);
            return View(model);
        }

        // GET: Board/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Board/Create
        [HttpPost]
        public ActionResult Create(BoardCreateModel data)
        {
            db_.Boards.Add(new BoardEntity
            {
                Title = data.Title,
                Text = data.Text
            });
            db_.SaveChanges();
            return View();
        }
    }
}
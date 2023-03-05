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

        // GET: Board/Create
        [HttpPost]
        public ActionResult Create(BoardCreateModel data)
        {
            var result = db_.Boards.Add(new BoardEntity
            {
                Title = data.Title,
                Text = data.Text
            });

            db_.SaveChanges();
            return Redirect("/Board/Show/" + result.Id);
        }

        // GET: Board/Show/{ID}
        public ActionResult Show(int id)
        {
            var board = (from o in db_.Boards where o.Id == id select o).DefaultIfEmpty(null).Single();
            return View(board);
        }

        // POST: Board/PostResponse/{id}
        [HttpPost]
        public ActionResult PostResponse(int id, BoardPostModel data)
        {
            var board = (from o in db_.Boards where o.Id == id select o).DefaultIfEmpty(null).Single();
            if(board != null)
            {
                board.Posts.Add(new BoardPostEntity
                {
                    Text = data.Text
                });
                db_.SaveChanges();
            }
            return Redirect("/Board/Show/" + id);
        }

    }
}
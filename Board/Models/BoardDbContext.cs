using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Board.Models {

    public class BoardEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        public virtual ICollection<BoardPostEntity> Posts { get; set; }

    }

    public class BoardPostEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
    }

    public class BoardListModel
    {
        public List<BoardEntity> Boards { get; private set; }

        public BoardListModel(BoardDbContext db)
        {
            Boards = db.Boards.ToList();
        }
    }


    public class BoardDbContext:DbContext
    {
        public BoardDbContext(): base("DefaultConnection")
        {

        }

        public virtual DbSet<BoardEntity> Boards { get; set; }
    }
}
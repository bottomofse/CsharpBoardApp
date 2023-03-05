using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

using Board.Controllers;
using Board.Models;

using Moq;

namespace Board.Tests.Controllers
{
    [TestClass]
    public class BoardControllerTest
    {
        [TestMethod]
        public void Index()
        {
            //DBのモックを用意
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            var originalData = new List<BoardEntity>
            {
                new BoardEntity{Id = 1,Title = "A", Text = "a"},
                new BoardEntity{Id = 2,Title = "B", Text = "b"},
                new BoardEntity{Id = 3,Title = "C", Text = "c"},
            };
            var data = originalData.AsQueryable();

            //返り値の設定
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockcontext.Setup(m => m.Boards).Returns(mockset.Object);


            var controller = new BoardController(mockcontext.Object);
            var result = controller.Index() as ViewResult;

            //データ取得の検証
            var model = result.Model as BoardListModel;
            Assert.AreSame(originalData[0], model.Boards[0]);
            Assert.AreSame(originalData[1], model.Boards[1]);
            Assert.AreSame(originalData[2], model.Boards[2]);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
        {
            var controller = new BoardController();
            var result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PostCreate()
        {
            //モック用意
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            mockcontext.Setup(m => m.Boards).Returns(mockset.Object);

            var model = new BoardCreateModel()
            {
                Title = "題名",
                Text = "本文"
            };
            var controller = new BoardController(mockcontext.Object);
            var result = controller.Create(model) as ViewResult;
            Assert.IsNotNull(result);

            //Addが呼ばれたかチェック
            mockset.Verify(m => m.Add(It.Is<BoardEntity>(o => o.Title == model.Title && o.Text == model.Text)), Times.Once);

            //saveChangesが呼ばれたかチェック
            mockcontext.Verify(m => m.SaveChanges(), Times.Once);

        }
    }
}

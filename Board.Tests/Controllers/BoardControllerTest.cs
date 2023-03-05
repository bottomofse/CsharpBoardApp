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

            var model = new BoardCreateModel()
            {
                Title = "題名",
                Text = "本文"
            };

            var dummy = new BoardEntity { Id = 1, Title = model.Title, Text = model.Text };
            mockset.As<IDbSet<BoardEntity>>().Setup(m => m.Add(It.IsAny<BoardEntity>())).Returns(dummy);

            mockcontext.Setup(m => m.Boards).Returns(mockset.Object);


            var controller = new BoardController(mockcontext.Object);
            var result = controller.Create(model) as RedirectResult;
            Assert.IsNotNull(result);

            //Addが呼ばれたかチェック
            mockset.Verify(m => m.Add(It.Is<BoardEntity>(o => o.Title == model.Title && o.Text == model.Text)), Times.Once);

            //saveChangesが呼ばれたかチェック
            mockcontext.Verify(m => m.SaveChanges(), Times.Once);

            Assert.AreEqual(result.Url, "/Board/Show/1");
        }

        [TestMethod]
        public void Show()
        {
            //DBのモック用意
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            //掲示板の情報
            var postOriginalData = new List<BoardPostEntity>
            {
                new BoardPostEntity { Text = "投稿1"},
                new BoardPostEntity { Text = "投稿2"}
            };

            //レスの情報
            var originalData = new List<BoardEntity>
            {
                new BoardEntity{ Id = 1, Title = "A", Text="a", Posts=postOriginalData}
            };
            var data = originalData.AsQueryable();

            //メソッドの戻り値を設定
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockcontext.Setup(m => m.Boards).Returns(mockset.Object);

            var controller = new BoardController(mockcontext.Object);
            ViewResult result = controller.Show(1) as ViewResult;
            
            //モデルのデータがDBのデータを取得できているか
            var model = result.Model as BoardEntity;
            Assert.AreSame(originalData[0], model);
            Assert.AreSame(postOriginalData[0], model.Posts.ToArray()[0]);
            Assert.AreSame(postOriginalData[1], model.Posts.ToArray()[1]);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PostResponse()
        {
            //DBのモック用意
            var mockposts = new Mock<ICollection<BoardPostEntity>>();
            var mockset = new Mock<DbSet<BoardEntity>>();
            var mockcontext = new Mock<BoardDbContext>();

            var originalData = new List<BoardEntity>
            {
                new BoardEntity{ Id=1, Title= "A", Text="a", Posts=mockposts.Object}
            };
            var data = originalData.AsQueryable();

            //メソッドのの返り値をモックに差し替え
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockset.As<IQueryable<BoardEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockcontext.Setup(m => m.Boards).Returns(mockset.Object);

            var postData = new BoardPostModel { Text = "投稿内容" };

            var controller = new BoardController(mockcontext.Object);
            var result = controller.PostResponse(1, postData) as RedirectResult;

            //データ追加されているか確認
            mockposts.Verify(m => m.Add(It.Is<BoardPostEntity>(o => o.Text == postData.Text)), Times.Once);
            mockcontext.Verify(m => m.SaveChanges(), Times.Once);
        }


    }
}

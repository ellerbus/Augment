using System;
using System.Collections.Generic;
using System.Linq;
using Augment.Mailing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.Mailing
{
    [TestClass]
    public class MailerTests
    {
        #region Members

        public class MessageContainer<T>
        {
            public string Subject { get; set; }
            public string Message { get; set; }
            public T Item { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }
        }

        #endregion

        #region Templating

        [TestMethod]
        public void Mailer_Should_RenderWithAnonymousObjectProperty()
        {
            var list = new List<int> { 1, 2, 3 };

            var item = new { numbers = list };

            var mc = GetMessageContainer(item);

            var template = "{{ subject }} {{ message }} {% for x in item.numbers %}{{ x }}{% endfor %}";

            var msg = Mailer.Create()
                .From("a@b.c")
                .To("d@e.f")
                .Subject("Subject")
                .RenderBodyWith(mc, template)
                .ToMessage();

            msg.Body.Should().Be("Hello Ya'll Alot of Stuff 123");
        }

        [TestMethod]
        public void Mailer_Should_RenderWithAnonymousObjectProperty_AndGenericList()
        {
            var list = new[] { new Person() { Name = "bob" } };

            var item = new { people = list };

            var mc = GetMessageContainer(item);

            var template = "{{ subject }} {{ message }} {% for x in item.people %}{{ x.name }}{% endfor %}";

            var msg = Mailer.Create()
                .From("a@b.c")
                .To("d@e.f")
                .Subject("Subject")
                .RenderBodyWith(mc, template)
                .ToMessage();

            msg.Body.Should().Be("Hello Ya'll Alot of Stuff bob");
        }

        private MessageContainer<T> GetMessageContainer<T>(T item)
        {
            var mc = new MessageContainer<T>()
            {
                Subject = "Hello Ya'll",
                Message = "Alot of Stuff",
                Item = item
            };

            return mc;
        }

        #endregion

        #region Addressing

        [TestMethod]
        public void Mailer_ShouldHave_Subject()
        {
            var subject = "Hello Ya'll";

            var mailer = Mailer.Create()
                .From("a@b.c")
                .To("d@e.f")
                .Subject(subject)
                .Body("Hello")
                .ToMessage();

            mailer.Subject.Should().Be(subject);
        }

        [TestMethod]
        public void Mailer_ShouldHave_From()
        {
            var from = "a@b.c";

            var mailer = Mailer.Create()
                .From(from)
                .To("d@e.f")
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.From.Should().Be(from);
        }

        [TestMethod]
        public void Mailer_ShouldHave_To()
        {
            var to = "a@b.c";

            var mailer = Mailer.Create()
                .To(to)
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.To.Count.Should().Be(1);

            mailer.To.Any(x => x.Address.IsSameAs(to)).Should().BeTrue();
        }

        [TestMethod]
        public void Mailer_ShouldHave_AllTo()
        {
            var to = new[] { "a@b.c", "d@f.g" };

            var mailer = Mailer.Create()
                .To(to)
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.To.Count.Should().Be(2);

            mailer.To.ElementAt(0).Should().Be(to[0]);
            mailer.To.ElementAt(1).Should().Be(to[1]);
        }

        [TestMethod]
        public void Mailer_ShouldHave_Cc()
        {
            var cc = "a@b.c";

            var mailer = Mailer.Create()
                .Cc(cc)
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.CC.Count.Should().Be(1);

            mailer.CC.Any(x => x.Address.IsSameAs(cc)).Should().BeTrue();
        }

        [TestMethod]
        public void Mailer_ShouldHave_AllCc()
        {
            var cc = new[] { "a@b.c", "d@f.g" };

            var mailer = Mailer.Create()
                .Cc(cc)
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.CC.Count.Should().Be(2);

            mailer.CC.ElementAt(0).Should().Be(cc[0]);
            mailer.CC.ElementAt(1).Should().Be(cc[1]);
        }

        [TestMethod]
        public void Mailer_ShouldHave_Bcc()
        {
            var bcc = "a@b.c";

            var mailer = Mailer.Create()
                .Bcc(bcc)
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.Bcc.Count.Should().Be(1);

            mailer.Bcc.Any(x => x.Address.IsSameAs(bcc)).Should().BeTrue();
        }

        [TestMethod]
        public void Mailer_ShouldHave_AllBcc()
        {
            var bcc = new[] { "a@b.c", "d@f.g" };

            var mailer = Mailer.Create()
                .Bcc(bcc)
                .Subject("Hello World!")
                .Body("How ya doing?")
                .ToMessage();

            mailer.Bcc.Count.Should().Be(2);

            mailer.Bcc.ElementAt(0).Should().Be(bcc[0]);
            mailer.Bcc.ElementAt(1).Should().Be(bcc[1]);
        }

        #endregion

        #region Exceptions

        [TestMethod]
        public void Mailer_ShouldThrow_SubjectException()
        {
            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .From("a@b.c")
                 .To("d@e.f")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_BodyException()
        {
            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .From("a@b.c")
                 .To("d@e.f")
                 .Subject("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_FromArgumentNullException()
        {
            var from = null as string;

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .From(from)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldNotThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_FromArgumentException()
        {
            var from = "";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .From(from)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldNotThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_FromFormatException()
        {
            var from = "a";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .From(from)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_ToArgumentNullException()
        {
            var to = null as string;

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .To(to)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_ToArgumentException()
        {
            var to = "";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .To(to)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_ToFormatException()
        {
            var to = "a";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .To(to)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_AllTo()
        {
            var to = new[] { "a", "d@f.g" };

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .To(to)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_CcArgumentNullException()
        {
            var cc = null as string;

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Cc(cc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_CcArgumentException()
        {
            var cc = "";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Cc(cc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_CcFormatException()
        {
            var cc = "a";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Cc(cc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_AllCc()
        {
            var cc = new[] { "a", "d@f.g" };

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Cc(cc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_BccArgumentNullException()
        {
            var bcc = null as string;

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Bcc(bcc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_BccArgumentException()
        {
            var bcc = "";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Bcc(bcc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_BccFormatException()
        {
            var bcc = "a";

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Bcc(bcc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Mailer_ShouldThrow_AllBcc()
        {
            var bcc = new[] { "a", "d@f.g" };

            var action = new Action(() =>
            {
                var mailer = Mailer.Create()
                 .Bcc(bcc)
                 .Subject("Hello")
                 .Body("Hello")
                 .ToMessage();
            });

            action.ShouldThrow<FormatException>();
        }

        #endregion
    }
}

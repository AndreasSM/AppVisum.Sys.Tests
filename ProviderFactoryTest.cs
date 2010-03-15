using AppVisum.Sys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AppVisum.Sys.EventTypes;
using System.Linq;

namespace AppVisum.Sys.Tests
{
    #region TestBound classes and interfaces
    interface INotProvider
    {
        void DoSomething();
    }

    [ProviderType("ProviderType 1")]
    interface IProvider1
    {
        void DoSomething();
    }

    [ProviderType("ProviderType 2")]
    interface IProvider2
    {
        void DoSomething();
    }

    [ProviderType("ProviderType 1")]
    interface IProvider3
    {
        void DoSomething();
    }

    class Provider1 : ProviderBase, IProvider1
    {
        public void DoSomething() { }

        public override string Name
        {
            get { return "Provider 1"; }
        }
    }

    class Provider2 : ProviderBase,  IProvider1
    {
        public Provider2(string minstr) { }

        public void DoSomething() { }

        public override string Name
        {
            get { return "Provider 2"; }
        }
    }

    class Provider3 : ProviderBase, IProvider1
    {
        public Provider3(string minstr = "test") { }

        public void DoSomething() { }

        public override string Name
        {
            get { return "Provider 3"; }
        }
    }

    class Provider4 : IProvider1
    {
        public Provider4() { }

        public void DoSomething() { }
    }

    class Provider5 : ProviderBase, IProvider1
    {
        public Provider5() { }

        public void DoSomething() { }

        public override string Name
        {
            get { return "Provider 1"; }
        }
    }

    class Provider6 : ProviderBase, IProvider2
    {
        private ProviderFactory pf;

        public Provider6(ProviderFactory pf)
        {
            this.pf = pf;
        }

        public void DoSomething() { }

        public override string Name
        {
            get { return "Provider 6"; }
        }

        public override bool CanUse
        {
            get { try { return pf.Instance<IProvider1>().GetType() == typeof(Provider1); } catch { return false; } }
        }
    }

    class Provider7 : ProviderBase, IProvider2
    {
        public Provider7() { }

        public void DoSomething() { }

        public override string Name
        {
            get { return "Provider 7"; }
        }
    }

    #endregion

    /// <summary>
    ///This is a test class for ProviderFactoryTest and is intended
    ///to contain all ProviderFactoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class ProviderFactoryTest
    {


        private TestContext testContextInstance;
        private ProviderFactory providerFactory;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            providerFactory = new ProviderFactory();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            providerFactory = null;
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ProviderFactory Constructor
        ///</summary>
        [TestMethod]
        public void ProviderFactoryConstructorTest()
        {
            Assert.IsNotNull(providerFactory);
        }

        #region ProviderFactory.RegisterType

        [TestMethod]
        public void ProviderFactory_RegisterType_ThrowsArgumentNullException_OnEmpty()
        {
            try
            {
                providerFactory.RegisterType(null);
                Assert.Fail("ProviderFactory did not throw ArgumentNullException.");
            }
            catch (ArgumentNullException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
                Assert.AreEqual("type", e.ParamName);
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentNullException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_RegisterType_ThrowsArgumentException_OnNonInterface()
        {
            try
            {
                providerFactory.RegisterType(this.GetType());
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("type", e.ParamName);
                Assert.IsTrue(e.Message.Contains("Interface"), "The error was not related to the type not beeing an interface.");
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_RegisterType_ThrowsArgumentException_OnNonProvider()
        {
            try
            {
                providerFactory.RegisterType(typeof(INotProvider));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("type", e.ParamName);
                Assert.IsTrue(e.Message.Contains("ProviderTypeAttribute"), "The error was not related to the type not beeing an provider.");
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_RegisterType_ThrowsArgumentException_OnDoubleProvider()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.RegisterType(typeof(IProvider1));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("type", e.ParamName);
                Assert.IsTrue(e.Message.Contains("registered"), "The error was not related to the type already beeing registered.");
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_RegisterType_ThrowsArgumentException_OnDoupleProvidername()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.RegisterType(typeof(IProvider3));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("type", e.ParamName);
                Assert.IsTrue(e.Message.Contains("registered"), "The error was not related to the type already beeing registered.");
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_RegisterType_Succeeds_OnProvider()
        {
            providerFactory.RegisterType(typeof(IProvider1));
        }

        #endregion

        #region ProviderFactory.Register

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnProviderDoesntInherit_ProviderBase()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(typeof(Provider4));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("provider", e.ParamName);
                Assert.IsTrue(e.Message.Contains("ProviderBase"));
            }
            catch
            {
                Assert.Fail("Exception wasn't an ArgumentException");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnProviderNameDuplicate()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(typeof(Provider1));
                providerFactory.Register(typeof(Provider5));
                Assert.Fail("ProviderFactory did not thow ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("provider", e.ParamName);
            }
            catch
            {
                Assert.Fail("Exception wasn't an ArgumentException");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentNullException_OnEmpty()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(null);
                Assert.Fail("ProviderFactory did not throw ArgumentNullException.");
            }
            catch (ArgumentNullException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
                Assert.AreEqual("provider", e.ParamName);
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentNullException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnInterface()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(typeof(INotProvider));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("provider", e.ParamName);
                Assert.IsTrue(e.Message.Contains("Interface"), "The error was not related to the type beeing an interface.");
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnNotRegisteredType()
        {
            try
            {
                providerFactory.Register(typeof(Provider));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("provider", e.ParamName);
                Assert.IsTrue(e.Message.Contains("does not have a registered type"), "The error was not related to the type not beeing registered.");
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnDuplicates()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(typeof(Provider1));
                providerFactory.Register(typeof(Provider1));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("provider", e.ParamName);
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnConstructorlessWithoutInstance()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(typeof(Provider2));
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_ThrowsArgumentException_OnWrongInstance()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.Register(typeof(Provider2), new Provider1());
                Assert.Fail("ProviderFactory did not throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
            catch
            {
                Assert.Fail("The exception thrown was not an ArgumentException.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Register_Succeeds_OnValidProviderAndInstance()
        {
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.RegisterType(typeof(IProvider2));
            providerFactory.Register(typeof(Provider1));
            providerFactory.Register(typeof(Provider2), new Provider2("tester"));
            providerFactory.Register(typeof(Provider3));
            providerFactory.Register(typeof(Provider6));
        }

        #endregion

        #region ProviderFactory.SetCurrent

        [TestMethod]
        public void ProviderFactory_SetCurrent_ThrowsArgumentException_OnUnregisteredType()
        {
            try
            {
                providerFactory.SetCurrent<IProvider1>("Provider1");
                Assert.Fail("ProviderFactory didn't throw exception.");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("T", e.ParamName);
            }
            catch
            {
                Assert.Fail("Wrong exception thrown.");
            }
        }

        [TestMethod]
        public void ProviderFactory_SetCurrent_ThrowsArgumentException_OnUnregisteredProvider()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.SetCurrent<IProvider1>("Provider1");
                Assert.Fail("ProviderFactory didn't throw exception.");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("providername", e.ParamName);
            }
            catch
            {
                Assert.Fail("Wrong exception thrown.");
            }
        }

        [TestMethod]
        public void ProviderFactory_SetCurrent_Succeeds_OnValidProviderAndType()
        {
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
            providerFactory.SetCurrent<IProvider1>("provider 1");
            providerFactory.SetCurrent<IProvider1>("Provider 1");
            providerFactory.SetCurrent<IProvider1>("PROVIDER 1");
        }

        #endregion

        #region ProviderFactory.Instance<T>()

        [TestMethod]
        public void ProviderFactory_Instance_ThrowsException_OnSelectedProviderUnusable()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                providerFactory.RegisterType(typeof(IProvider2));
                providerFactory.Register(typeof(Provider3));
                providerFactory.Register(typeof(Provider6));
                providerFactory.SetCurrent<IProvider1>("Provider 3");
                providerFactory.SetCurrent<IProvider2>("Provider 6");
                IProvider2 provider = providerFactory.Instance<IProvider2>();
                Assert.Fail("Did not throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Provided provider can't be used"));
            }
        }

        [TestMethod]
        public void ProviderFactory_Instance_ThrowsArgumentException_OnUnregisteredType()
        {
            try
            {
                IProvider1 provider = providerFactory.Instance<IProvider1>();
                Assert.Fail("ProviderFactory didn't throw exception.");
            }
            catch (ArgumentException e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
                Assert.AreEqual("T", e.ParamName);
            }
            catch
            {
                Assert.Fail("Wrong exception thrown.");
            }
        }

        [TestMethod]
        public void ProviderFactory_Instance_ThrowsArgumentException_OnNoProvidersRegisterd()
        {
            try
            {
                providerFactory.RegisterType(typeof(IProvider1));
                IProvider1 provider = providerFactory.Instance<IProvider1>();
                Assert.Fail("ProviderFactory didn't throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("No providers that can be used was found for that type"));
            }
        }

        [TestMethod]
        public void ProviderFactory_Instance_ReturnsInstance_OnValidTypes()
        {
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
            IProvider1 provider = providerFactory.Instance<IProvider1>();

            Assert.IsNotNull(provider);
            Assert.IsInstanceOfType(provider, typeof(IProvider1));
        }

        [TestMethod]
        public void ProviderFactory_Instance_ReturnsSameInstanceEveryTime()
        {
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
            IProvider1 provider = providerFactory.Instance<IProvider1>();

            for (int i = 0; i < 10; i++)
                Assert.ReferenceEquals(provider, providerFactory.Instance<IProvider1>());
        }

        #endregion

        #region ProviderFactory.SetCurrent<T> -> ProviderFactory.Instance<T>

        [TestMethod]
        public void ProviderFactory_Instance_ReturnsTypeSetBy_SetCurrent()
        {
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
            providerFactory.Register(typeof(Provider3));
            providerFactory.SetCurrent<IProvider1>("Provider 1");

            Assert.IsInstanceOfType(providerFactory.Instance<IProvider1>(), typeof(Provider1));

            providerFactory.SetCurrent<IProvider1>("Provider 3");

            Assert.IsInstanceOfType(providerFactory.Instance<IProvider1>(), typeof(Provider3));
        }

        #endregion

        #region Events

        [TestMethod]
        public void ProviderFactory_RegisterType_Fires_ProviderTypeRegistered()
        {
            providerFactory.TypeRegistered += (Object sender, ProviderTypeEventArgs args) =>
            {
                Assert.ReferenceEquals(sender, providerFactory);
                Assert.AreEqual(typeof(IProvider1), args.ProviderType.Type);
                Assert.AreEqual("ProviderType 1", args.ProviderType.Name);
            };
            providerFactory.RegisterType(typeof(IProvider1));
        }

        [TestMethod]
        public void ProviderFactory_Register_Fires_ProviderRegistered()
        {
            providerFactory.ProviderRegistered += (Object sender, ProviderEventArgs args) =>
            {
                Assert.ReferenceEquals(sender, providerFactory);
                Assert.AreEqual(typeof(IProvider1), args.ProviderType.Type);
                Assert.AreEqual("ProviderType 1", args.ProviderType.Name);
                Assert.AreEqual(typeof(Provider1), args.Provider.Type);
                Assert.IsNotNull(args.Provider.Instance);
            };
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
        }

        [TestMethod]
        public void ProviderFactory_SetCurrent_Fires_ProviderSelected()
        {
            providerFactory.ProviderSelected += (Object sender, ProviderEventArgs args) =>
            {
                Assert.ReferenceEquals(sender, providerFactory);
                Assert.AreEqual(typeof(IProvider1), args.ProviderType.Type);
                Assert.AreEqual("ProviderType 1", args.ProviderType.Name);
                Assert.AreEqual(typeof(Provider1), args.Provider.Type);
                Assert.IsNotNull(args.Provider.Instance);
            };
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
            providerFactory.Register(typeof(Provider2), new Provider2("test"));
            providerFactory.SetCurrent<IProvider1>("Provider 1");
        }

        [TestMethod]
        public void ProviderFactory_Instance_Fires_ProviderInstanceCreated_OnNewInstance()
        {
            providerFactory.ProviderInstanceCreated += (Object sender, ProviderEventArgs args) =>
            {
                Assert.ReferenceEquals(sender, providerFactory);
                Assert.AreEqual(typeof(IProvider1), args.ProviderType);
                Assert.AreEqual("ProviderType 1", args.ProviderType.Name);
                Assert.AreEqual(typeof(Provider1), args.Provider);
            };
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.Register(typeof(Provider1));
            providerFactory.Register(typeof(Provider2), new Provider2("test"));
            providerFactory.SetCurrent<IProvider1>("Provider 1");
            Assert.IsInstanceOfType(providerFactory.Instance<IProvider1>(), typeof(Provider1));
            providerFactory.SetCurrent<IProvider1>("Provider 2");
            Assert.IsInstanceOfType(providerFactory.Instance<IProvider1>(), typeof(Provider2));
        }

        #endregion

        #region ProviderFactory.GetRegisteredProviderTypes

        [TestMethod]
        public void ProviderFactory_GetRegisteredProviderTypes()
        {
            providerFactory.RegisterType(typeof(IProvider1));
            providerFactory.RegisterType(typeof(IProvider2));

            ProviderType[] types = providerFactory.GetRegisteredProviderTypes();

            Assert.AreEqual(2, types.Length);
        }

        #endregion

        #region ProviderFactory.GetRegisteredProviders<T>

        [TestMethod]
        public void ProviderFactory_GetRegisteredProviders()
        {
            providerFactory.RegisterType(typeof(IProvider2));
            providerFactory.Register(typeof(Provider6));
            providerFactory.Register(typeof(Provider7));

            Provider[] usable = providerFactory.GetRegisteredProviders<IProvider2>();
            Provider[] all = providerFactory.GetRegisteredProviders<IProvider2>(false);

            Assert.AreEqual(1, usable.Length);
            Assert.AreEqual(2, all.Length);
        }

        #endregion
    }
}

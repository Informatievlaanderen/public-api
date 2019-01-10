namespace Public.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class IsSubClassOfGenericExtension
    {
        public static bool IsSubClassOfGeneric(this Type child, Type parent)
        {
            if (child == parent)
                return false;

            if (child.IsSubclassOf(parent))
                return true;

            var parameters = parent.GetGenericArguments();

            var isParameterLessGeneric =
                !(parameters != null &&
                  parameters.Length > 0 &&
                  (parameters[0].Attributes & TypeAttributes.BeforeFieldInit) == TypeAttributes.BeforeFieldInit);

            while (child != null && child != typeof(object))
            {
                var cur = GetFullTypeDefinition(child);
                if (parent == cur || (isParameterLessGeneric && cur.GetInterfaces().Select(GetFullTypeDefinition).Contains(GetFullTypeDefinition(parent))))
                    return true;

                if (!isParameterLessGeneric)
                {
                    if (GetFullTypeDefinition(parent) == cur && !cur.IsInterface)
                    {
                        if (VerifyGenericArguments(GetFullTypeDefinition(parent), cur))
                            if (VerifyGenericArguments(parent, child))
                                return true;
                    }
                    else
                    {
                        if (child
                            .GetInterfaces()
                            .Where(i => GetFullTypeDefinition(parent) == GetFullTypeDefinition(i))
                            .Any(item => VerifyGenericArguments(parent, item)))
                            return true;
                    }
                }

                child = child.BaseType;
            }

            return false;
        }

        private static Type GetFullTypeDefinition(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        private static bool VerifyGenericArguments(Type parent, Type child)
        {
            var childArguments = child.GetGenericArguments();
            var parentArguments = parent.GetGenericArguments();
            if (childArguments.Length != parentArguments.Length)
                return true;

            return !childArguments
                .Where((t, i) => (t.Assembly != parentArguments[i].Assembly || t.Name != parentArguments[i].Name || t.Namespace != parentArguments[i].Namespace) && !t.IsSubclassOf(parentArguments[i]))
                .Any();
        }
    }
}

//[TestMethod]
//public void IsSubClassOfGenericTest()
//{
//    Assert.IsTrue(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(BaseGeneric<>)), " 1");
//    Assert.IsFalse(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(WrongBaseGeneric<>)), " 2");
//    Assert.IsTrue(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), " 3");
//    Assert.IsFalse(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IWrongBaseGeneric<>)), " 4");
//    Assert.IsTrue(typeof(IChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), " 5");
//    Assert.IsFalse(typeof(IWrongBaseGeneric<>).IsSubClassOfGeneric(typeof(ChildGeneric2<>)), " 6");
//    Assert.IsTrue(typeof(ChildGeneric2<>).IsSubClassOfGeneric(typeof(BaseGeneric<>)), " 7");
//    Assert.IsTrue(typeof(ChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(BaseGeneric<>)), " 8");
//    Assert.IsTrue(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(BaseGeneric<Class1>)), " 9");
//    Assert.IsFalse(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(WrongBaseGeneric<Class1>)), "10");
//    Assert.IsTrue(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "11");
//    Assert.IsFalse(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IWrongBaseGeneric<Class1>)), "12");
//    Assert.IsTrue(typeof(IChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "13");
//    Assert.IsFalse(typeof(BaseGeneric<Class1>).IsSubClassOfGeneric(typeof(ChildGeneric2<Class1>)), "14");
//    Assert.IsTrue(typeof(ChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(BaseGeneric<Class1>)), "15");
//    Assert.IsFalse(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(ChildGeneric)), "16");
//    Assert.IsFalse(typeof(IChildGeneric).IsSubClassOfGeneric(typeof(IChildGeneric)), "17");
//    Assert.IsFalse(typeof(IBaseGeneric<>).IsSubClassOfGeneric(typeof(IChildGeneric2<>)), "18");
//    Assert.IsTrue(typeof(IChildGeneric2<>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "19");
//    Assert.IsTrue(typeof(IChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "20");
//    Assert.IsFalse(typeof(IBaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IChildGeneric2<Class1>)), "21");
//    Assert.IsTrue(typeof(IChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "22");
//    Assert.IsFalse(typeof(IBaseGeneric<Class1>).IsSubClassOfGeneric(typeof(BaseGeneric<Class1>)), "23");
//    Assert.IsTrue(typeof(BaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "24");
//    Assert.IsFalse(typeof(IBaseGeneric<>).IsSubClassOfGeneric(typeof(BaseGeneric<>)), "25");
//    Assert.IsTrue(typeof(BaseGeneric<>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "26");
//    Assert.IsTrue(typeof(BaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "27");
//    Assert.IsFalse(typeof(IBaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "28");
//    Assert.IsTrue(typeof(BaseGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "29");
//    Assert.IsFalse(typeof(IBaseGeneric<>).IsSubClassOfGeneric(typeof(BaseGeneric2<>)), "30");
//    Assert.IsTrue(typeof(BaseGeneric2<>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "31");
//    Assert.IsTrue(typeof(BaseGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "32");
//    Assert.IsTrue(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "33");
//    Assert.IsFalse(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(WrongBaseGenericA<,>)), "34");
//    Assert.IsTrue(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "35");
//    Assert.IsFalse(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IWrongBaseGenericA<,>)), "36");
//    Assert.IsTrue(typeof(IChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "37");
//    Assert.IsFalse(typeof(IWrongBaseGenericA<,>).IsSubClassOfGeneric(typeof(ChildGenericA2<,>)), "38");
//    Assert.IsTrue(typeof(ChildGenericA2<,>).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "39");
//    Assert.IsTrue(typeof(ChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "40");
//    Assert.IsTrue(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "41");
//    Assert.IsFalse(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(WrongBaseGenericA<ClassA, ClassB>)), "42");
//    Assert.IsTrue(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "43");
//    Assert.IsFalse(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IWrongBaseGenericA<ClassA, ClassB>)), "44");
//    Assert.IsTrue(typeof(IChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "45");
//    Assert.IsFalse(typeof(BaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(ChildGenericA2<ClassA, ClassB>)), "46");
//    Assert.IsTrue(typeof(ChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "47");
//    Assert.IsFalse(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(ChildGenericA)), "48");
//    Assert.IsFalse(typeof(IChildGenericA).IsSubClassOfGeneric(typeof(IChildGenericA)), "49");
//    Assert.IsFalse(typeof(IBaseGenericA<,>).IsSubClassOfGeneric(typeof(IChildGenericA2<,>)), "50");
//    Assert.IsTrue(typeof(IChildGenericA2<,>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "51");
//    Assert.IsTrue(typeof(IChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "52");
//    Assert.IsFalse(typeof(IBaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IChildGenericA2<ClassA, ClassB>)), "53");
//    Assert.IsTrue(typeof(IChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "54");
//    Assert.IsFalse(typeof(IBaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "55");
//    Assert.IsTrue(typeof(BaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "56");
//    Assert.IsFalse(typeof(IBaseGenericA<,>).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "57");
//    Assert.IsTrue(typeof(BaseGenericA<,>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "58");
//    Assert.IsTrue(typeof(BaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "59");
//    Assert.IsFalse(typeof(IBaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "60");
//    Assert.IsTrue(typeof(BaseGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "61");
//    Assert.IsFalse(typeof(IBaseGenericA<,>).IsSubClassOfGeneric(typeof(BaseGenericA2<,>)), "62");
//    Assert.IsTrue(typeof(BaseGenericA2<,>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "63");
//    Assert.IsTrue(typeof(BaseGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "64");
//    Assert.IsFalse(typeof(BaseGenericA2<ClassB, ClassA>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "65");
//    Assert.IsFalse(typeof(BaseGenericA<ClassB, ClassA>).IsSubClassOfGeneric(typeof(ChildGenericA2<ClassA, ClassB>)), "66");
//    Assert.IsFalse(typeof(BaseGenericA2<ClassB, ClassA>).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "67");
//    Assert.IsTrue(typeof(ChildGenericA3<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericB<ClassA, ClassB, ClassC>)), "68");
//    Assert.IsTrue(typeof(ChildGenericA4<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericB<ClassA, ClassB, ClassC>)), "69");
//    Assert.IsFalse(typeof(ChildGenericA3<ClassB, ClassA>).IsSubClassOfGeneric(typeof(BaseGenericB<ClassA, ClassB, ClassC>)), "68-2");
//    Assert.IsTrue(typeof(ChildGenericA3<ClassA, ClassB2>).IsSubClassOfGeneric(typeof(BaseGenericB<ClassA, ClassB, ClassC>)), "68-3");
//    Assert.IsFalse(typeof(ChildGenericA3<ClassB2, ClassA>).IsSubClassOfGeneric(typeof(BaseGenericB<ClassA, ClassB, ClassC>)), "68-4");
//    Assert.IsFalse(typeof(ChildGenericA4<ClassB, ClassA>).IsSubClassOfGeneric(typeof(IBaseGenericB<ClassA, ClassB, ClassC>)), "69-2");
//    Assert.IsTrue(typeof(ChildGenericA4<ClassA, ClassB2>).IsSubClassOfGeneric(typeof(IBaseGenericB<ClassA, ClassB, ClassC>)), "69-3");
//    Assert.IsFalse(typeof(ChildGenericA4<ClassB2, ClassA>).IsSubClassOfGeneric(typeof(IBaseGenericB<ClassA, ClassB, ClassC>)), "69-4");
//    Assert.IsFalse(typeof(bool).IsSubClassOfGeneric(typeof(IBaseGenericB<ClassA, ClassB, ClassC>)), "70");
//}

//public class Class1 { }
//public class BaseGeneric<T> : IBaseGeneric<T> { }
//public class BaseGeneric2<T> : IBaseGeneric<T>, IInterfaceBidon { }
//public interface IBaseGeneric<T> { }
//public class ChildGeneric : BaseGeneric<Class1> { }
//public interface IChildGeneric : IBaseGeneric<Class1> { }
//public class ChildGeneric2<Class1> : BaseGeneric<Class1> { }
//public interface IChildGeneric2<Class1> : IBaseGeneric<Class1> { }

//public class WrongBaseGeneric<T> { }
//public interface IWrongBaseGeneric<T> { }

//public interface IInterfaceBidon { }

//public class ClassA { }
//public class ClassB { }
//public class ClassC { }
//public class ClassB2 : ClassB { }
//public class BaseGenericA<T, U> : IBaseGenericA<T, U> { }
//public class BaseGenericB<T, U, V> { }
//public interface IBaseGenericB<ClassA, ClassB, ClassC> { }
//public class BaseGenericA2<T, U> : IBaseGenericA<T, U>, IInterfaceBidonA { }
//public interface IBaseGenericA<T, U> { }
//public class ChildGenericA : BaseGenericA<ClassA, ClassB> { }
//public interface IChildGenericA : IBaseGenericA<ClassA, ClassB> { }
//public class ChildGenericA2<ClassA, ClassB> : BaseGenericA<ClassA, ClassB> { }
//public class ChildGenericA3<ClassA, ClassB> : BaseGenericB<ClassA, ClassB, ClassC> { }
//public class ChildGenericA4<ClassA, ClassB> : IBaseGenericB<ClassA, ClassB, ClassC> { }
//public interface IChildGenericA2<ClassA, ClassB> : IBaseGenericA<ClassA, ClassB> { }

//public class WrongBaseGenericA<T, U> { }
//public interface IWrongBaseGenericA<T, U> { }

//public interface IInterfaceBidonA { }
